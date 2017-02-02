using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotSpatial.Data;
using GeoAPI.Geometries;

namespace BenMAP
{
    public static class CrosswalksCalculator
    {
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

        class GridCellEqualityComparer : IEqualityComparer<GridCell>
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

        public static Dictionary<CrosswalkIndex, CrosswalkRatios> Calculate(IFeatureSet grid, IFeatureSet features,
            CancellationToken cancellationToken, IProgress progress)
        {

            if (grid == null) throw new ArgumentNullException("grid");
            if (features == null) throw new ArgumentNullException("features");
            if (progress == null) throw new ArgumentNullException("progress");

            var output = new Dictionary<CrosswalkIndex, CrosswalkRatios>();
            var sync_lock = new object();

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
            Parallel.ForEach(features.Features, po, delegate(IFeature feature)
            {
                var featureGeometry = feature.Geometry;
                var featureId = feature.Fid;
                var featureArea = featureGeometry.Area;

                var featureEnvelope = featureGeometry.EnvelopeInternal;
                var filterByX = Find(sortedByX, featureEnvelope.MinX, featureEnvelope.MaxX);
                var filterByY = Find(sortedByY, featureEnvelope.MinY, featureEnvelope.MaxY);
                var intersectionCells = filterByX.Intersect(filterByY, new GridCellEqualityComparer());

                var localList = new List<Tuple<CrosswalkIndex, CrosswalkRatios>>();
                foreach (var cell in intersectionCells)
                {
                    var cellId = cell.Fid;
                    var cellGeometry = cell.Geometry;

                    var intersection = cellGeometry.Intersection(featureGeometry);
                    if (!(intersection == null || intersection.IsEmpty))
                    {
                        var area1 = cell.Area;
                        var intersectionArea = intersection.Area;

                        var entry = new CrosswalkRatios
                        {
                            ForwardRatio = (float) (intersectionArea / area1),
                            BackwardRatio = (float) (intersectionArea / featureArea)
                        };

                        var index = new CrosswalkIndex
                        {
                            FeatureId1 = cellId,
                            FeatureId2 = featureId
                        };

                        // Grid cell fully in feature, exclude it from further calcuations
                        if (entry.ForwardRatio == 1.0f)
                        {
                            cell.Used = true;
                        }
                        localList.Add(new Tuple<CrosswalkIndex, CrosswalkRatios>(index, entry));
                    }
                }

                lock (sync_lock)
                {
                    localList.ForEach(_ => output.Add(_.Item1, _.Item2));
                }

                var current = Interlocked.Increment(ref currentCount);
                progress.OnProgressChanged(string.Format("Feature {0}/{1} finished", current, featuresCount),
                    current / (featuresCount * 1.0f) * 100.0f);
            });

            progress.OnProgressChanged(string.Format("Finished. Total crosswalks: {0}", output.Count), 100);
            return output;
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

    public struct CrosswalkIndex
    {
        /// <summary>
        /// Feature Index from Input1.
        /// </summary>
        public int FeatureId1 { get; set; }

        /// <summary>
        /// Feature Index from Input2.
        /// </summary>
        public int FeatureId2 { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}", FeatureId1, FeatureId2);
        }
    }
  
    public struct CrosswalkRatios
    {
        /// <summary>
        /// Number in interval [0, 1.0]. Shows how much Feature1 contained in Feature2.
        /// </summary>
        public float ForwardRatio { get; set; }

        /// <summary>
        /// Number in interval [0, 1.0]. Shows how much Feature2 contained in Feature1.
        /// </summary>
        public float BackwardRatio { get; set; }

        public override string ToString()
        {
            return string.Format("F:{0}, B:{1}", ForwardRatio, BackwardRatio);
        }
    }
}