using System.ComponentModel;

namespace BenMAP.SelectByLocation
{
    public enum SelectionMethod
    {
        [Description("Select from currently selected in")]
        SelectFromCurrentlySelectedIn,

        [Description("Select features from")]
        SelectFeaturesFrom,
    }
}