using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotSpatial.Controls;
using DotSpatial.Data;

namespace BenMAP.SelectByLocation
{
    public class SelectByLocationWorker
    {
        private readonly IMapPolygonLayer _selectionLayer;
        private readonly IMapFeatureLayer _targetLayer;
        private readonly SelectionMethod _selectionMethod;
        private readonly SpatialSelectionMethod _spatialSelectionMethod;

        public SelectByLocationWorker(IMapPolygonLayer selectionLayer, IMapFeatureLayer targetLayer, SelectionMethod selectionMethod, SpatialSelectionMethod spatialSelectionMethod)
        {
            if (selectionLayer == null) throw new ArgumentNullException("selectionLayer");
            if (targetLayer == null) throw new ArgumentNullException("targetLayer");

            _selectionLayer = selectionLayer;
            _targetLayer = targetLayer;
            _selectionMethod = selectionMethod;
            _spatialSelectionMethod = spatialSelectionMethod;
        }

        public event EventHandler<OnSelectingEventArgs> OnSelecting;

        public List<int> Select()
        {
            IList<IFeature> sourceFeatures;
            switch (_selectionMethod)
            {
                case SelectionMethod.SelectFeaturesFrom:
                    sourceFeatures = _selectionLayer.DataSet.Features;
                    break;
                case SelectionMethod.SelectFromCurrentlySelectedIn:
                    sourceFeatures = _selectionLayer.Selection.ToFeatureList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Func<IFeature, IFeature, bool> selectionFunc;
            Func<Extent, Extent, bool> fastCheck;
            switch (_spatialSelectionMethod)
            {
                case SpatialSelectionMethod.Contain:
                    selectionFunc = (src, trgt) => src.Geometry.Contains(trgt.Geometry);
                    fastCheck = (src, trgt) => src.Intersects(trgt);
                    break;
                case SpatialSelectionMethod.Intersect:
                    selectionFunc = (src, trgt) => src.Geometry.Intersects(trgt.Geometry);
                    fastCheck = (src, trgt) => src.Intersects(trgt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var toSelect = new List<int>();
            var syncLock = new object();
            for (int i = 0; i < sourceFeatures.Count; i++)
            {
                var sourceFeature = sourceFeatures[i];
                var h = OnSelecting;
                if (h != null)
                {
                    h(this, new OnSelectingEventArgs(i, sourceFeatures.Count));
                }

                // Fast check: entire target layer -> given sourceFeature
                if (!fastCheck(_targetLayer.Extent, sourceFeature.Geometry.EnvelopeInternal.ToExtent())) continue;

                Parallel.ForEach(_targetLayer.DataSet.Features, delegate(IFeature feature)
                {
                    if (selectionFunc(sourceFeature, feature))
                    {
                        lock (syncLock)
                        {
                            toSelect.Add(feature.Fid);
                        }
                    }
                });
            }

            return toSelect;
        }
    }

    public class OnSelectingEventArgs : EventArgs
    {
        public OnSelectingEventArgs(int current, int total)
        {
            Current = current;
            Total = total;
        }

        public int Current { get; private set; }
        public int Total { get; private set; }
    }
}