using System.ComponentModel;

namespace BenMAP.SelectByLocation
{
    public enum SpatialSelectionMethod
    {
        [Description("Contain the source layer feature")]
        Contain,

        [Description("Intersect the source layer feature")]
        Intersect,
    }
}