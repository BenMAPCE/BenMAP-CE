using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotSpatial.Data;
using GeoAPI.Geometries;

namespace BenMAP.Crosswalks
{
    public static class CrosswalksCalculator
    {
        /// <summary>
        /// Computes crosswalks between grid and a list of features.
        /// Grid assumptions:
        /// 1. Has much more cells than features list count.
        /// 2. Every cell is convex polygon.
        /// 3. There are no intersections between cells. (only common borders are allowed) 
        /// </summary>
        /// <param name="grid">Grid with cells.</param>
        /// <param name="features">List of features.</param>
        /// <param name="cancellationToken">Cancellation token for cancellation support.</param>
        /// <param name="progress">Instance of <see cref="IProgress"/> for progress indication.</param>
        /// <returns>List of crosswalk indexes with appropriate ratios.</returns>
        public static IList<Crosswalk> Calculate(IFeatureSet grid, IFeatureSet features, CancellationToken cancellationToken, IProgress progress)
        {
            if (grid == null) throw new ArgumentNullException("grid");
            if (features == null) throw new ArgumentNullException("features");
            if (progress == null) throw new ArgumentNullException("progress");

            var output = new List<Crosswalk>();
            var syncLock = new object();

            var featuresCount = !features.IndexMode ? features.Features.Count : features.ShapeIndices.Count;
            var cellsCount = !grid.IndexMode ? grid.Features.Count : grid.ShapeIndices.Count;

            progress.OnProgressChanged("Preparing data", 0);

            // Sort grid cells
            var cellsX = new List<GridCell>();
            var cellsY = new List<GridCell>();

            for (int i = 0; i < cellsCount; i++)
            {
                var cellGeometry = grid.GetShape(i, false).ToGeometry();
                var env = cellGeometry.EnvelopeInternal;

                cellsX.Add(new GridCell(cellGeometry, env.MinX, env.MaxX, i));
                cellsY.Add(new GridCell(cellGeometry, env.MinY, env.MaxY, i));
            }

            var sortedByX = cellsX.OrderBy(_ => _.Min).ToList();
            var sortedByY = cellsY.OrderBy(_ => _.Min).ToList();

            var po = new ParallelOptions {CancellationToken = cancellationToken, MaxDegreeOfParallelism = -1};
            var currentCount = 0;

            // Begin calculation
            progress.OnProgressChanged("Begin calculation", 0);

            // Determine whenever use nested parallelization (for iterating through interesection cells).
            // This will gain performance for border case when features contains just one feature.
            var nestedParallelization = featuresCount == 1;
            var nestedParallelizationStep = Math.Max(cellsCount / 1000, 1); // protect from zeros

            Parallel.For(0, featuresCount, po, delegate(int fi)
            {
                var featureGeometry = features.GetShape(fi, false).ToGeometry();
                var featureId = fi;
                var featureArea = featureGeometry.Area;

                var featureEnvelope = featureGeometry.EnvelopeInternal;
                var filterByX = Find(sortedByX, featureEnvelope.MinX, featureEnvelope.MaxX);
                var filterByY = Find(sortedByY, featureEnvelope.MinY, featureEnvelope.MaxY);
                var intersectionCells = filterByX.Intersect(filterByY, new GridCellEqualityComparer());

                var localList = new List<Crosswalk>();

                if (nestedParallelization)
                {
                    var currentCellCount = 0;
                    Parallel.ForEach(intersectionCells, po, delegate(GridCell cell)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var ci = FindCrosswalk(cell, featureGeometry, featureArea, featureId);
                        if (ci != null)
                        {
                            lock (syncLock)
                            {
                                localList.Add(ci);
                            }
                        }

                        var currentCell = Interlocked.Increment(ref currentCellCount);
                        if (currentCell % nestedParallelizationStep == 0)
                        {
                            progress.OnProgressChanged(string.Format("Cell {0}/{1} finished", currentCell, cellsCount),
                                currentCell / (cellsCount * 1.0f) * 100.0f);
                        }
                    });
                }
                else
                {
                    foreach (var cell in intersectionCells)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var ci = FindCrosswalk(cell, featureGeometry, featureArea, featureId);
                        if (ci != null)
                        {
                            localList.Add(ci);
                        }
                    }
                }

                lock (syncLock)
                {
                    output.AddRange(localList);
                }

                var current = Interlocked.Increment(ref currentCount);
                progress.OnProgressChanged(string.Format("Feature {0}/{1} finished", current, featuresCount),
                    current / (featuresCount * 1.0f) * 100.0f);
            });

            // Do post processing.
            // Find all cross-border cells and adjuts ratios.

            output = output.OrderBy(_ => _.FeatureId1).ToList();            // Sort them by featureId for fast processing
            var postProcessProgressStep = Math.Max(output.Count / 1000, 1);
            for (var i = 0; i < output.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var current = output[i];
                if (current.ForwardRatio == 1.0f) continue;

                var cellParts = new List<Crosswalk> {current};
                float ratiosSum = current.ForwardRatio;
                for (var j = i + 1; j < output.Count; j++)
                {
                    // Output is sorted by featureId
                    if (output[j].FeatureId1 != current.FeatureId1) break;

                    cellParts.Add(output[j]);
                    ratiosSum += output[j].ForwardRatio;

                    // Increase outer counter.
                    // Since output is sorted, we skip already processed crosswalks in outer loop.
                    i++;
                }
                
                if (ratiosSum != 1.0f)  // We don't need extra operations
                {
                    foreach (var crosswalk in cellParts)
                    {
                        crosswalk.ForwardRatio /= ratiosSum;
                    }
                }

                if (i % postProcessProgressStep == 0)
                {
                    progress.OnProgressChanged(
                        string.Format("Adjusting cross-border ratios. {0}/{1} finished", i + 1, output.Count),
                        (i + 1) / (1.0f * output.Count) * 100.0f);
                }
            }

            progress.OnProgressChanged(string.Format("Finished. Total crosswalks: {0}", output.Count), 100);
            return output;
        }

        private static Crosswalk FindCrosswalk(GridCell cell, IGeometry featureGeometry,
            double featureArea, int featureId)
        {
            var cellGeometry = cell.Geometry;

            if (cellGeometry.Intersects(featureGeometry))
            {
                double intersectionArea = 0;

                // Quick test: feature geometry covers entire cell.
                // In this case intersectionArea is entire cell area.
                if (featureGeometry.Covers(cellGeometry))
                {
                    intersectionArea = cell.Area;
                }
                else
                {
                    var intersection = cellGeometry.Intersection(featureGeometry);
                    if (!(intersection == null || intersection.IsEmpty))
                    {

                        intersectionArea = intersection.Area;
                    }
                }
                if (intersectionArea > 0)
                {
                    var cellArea = cell.Area;
                    var output = new Crosswalk
                    {
                        ForwardRatio = (float) (intersectionArea / cellArea),
                        BackwardRatio = (float) (intersectionArea / featureArea),
                        FeatureId1 = cell.Fid,
                        FeatureId2 = featureId
                    };
                    

                    // Grid cell fully in feature, exclude it from further calcuations
                    if (output.ForwardRatio == 1.0f)
                    {
                        cell.Used = true;
                    }
                    return output;
                }
            }

            return null;
        }

        private class GridCell
        {
            private bool _hasArea;
            private double _area;

            public GridCell(IGeometry geometry, double min, double max, int fid)
            {
                Geometry = geometry;
                Min = min;
                Max = max;
                Fid = fid;
            }

            public IGeometry Geometry { get; private set; }
            public int Fid { get; private set; }
            public double Min { get; private set; }
            public double Max { get; private set; }
            public bool Used { get; set; }

            public double Area
            {
                get
                {
                    if (!_hasArea)
                    {
                        _area = Geometry.Area;
                        _hasArea = true;
                    }
                    return _area;
                }
            }
        }

        private class GridCellEqualityComparer : IEqualityComparer<GridCell>
        {
            public bool Equals(GridCell x, GridCell y)
            {
                return x.Fid.Equals(y.Fid);
            }

            public int GetHashCode(GridCell obj)
            {
                return obj.Fid.GetHashCode();
            }
        }

        private static IEnumerable<GridCell> Find(IEnumerable<GridCell> list, double min, double max)
        {
            // List ordered by min
            foreach (var cell in list)
            {
                if (cell.Max < min) continue;
                if (cell.Min > max) break; // all other also can be ignored
                if (cell.Used) continue;
                yield return cell;
            }
        }

    }

    public interface IProgress
    {
        /// <summary>
        /// Reports progress.
        /// </summary>
        /// <param name="message">Progress message.</param>
        /// <param name="progress">Progress value (percents).</param>
        void OnProgressChanged(string message, float progress);
    }

    public class Crosswalk
    {
        /// <summary>
        /// Feature Index from Input1.
        /// </summary>
        public int FeatureId1 { get; set; }

        /// <summary>
        /// Feature Index from Input2.
        /// </summary>
        public int FeatureId2 { get; set; }

        /// <summary>
        /// Number in interval [0, 1.0]. Shows how much Feature1 contained in Feature2.
        /// </summary>
        public float ForwardRatio { get; set; }

        /// <summary>
        /// Number in interval [0, 1.0]. Shows how much Feature2 contained in Feature1.
        /// </summary>
        public float BackwardRatio { get; set; }
    }
}