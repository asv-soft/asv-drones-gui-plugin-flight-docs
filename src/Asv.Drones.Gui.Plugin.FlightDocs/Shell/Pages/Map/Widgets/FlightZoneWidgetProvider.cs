using System.Composition;
using Asv.Cfg;
using Asv.Drones.Gui.Api;
using DynamicData;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(FlightZoneMapViewModel.UriString, typeof(IViewModelProvider<IMapWidget>))]
public class FlightZoneWidgetProvider : ViewModelProviderBase<IMapWidget>
{
    [ImportingConstructor]
    public FlightZoneWidgetProvider(ILocalizationService loc, IConfiguration cfg)
    {
        Source.AddOrUpdate(new FlightZoneMapWidgetViewModel(loc));
        Source.AddOrUpdate(new TakeOffLandMapWidgetViewModel(loc));
        Source.AddOrUpdate(new FlightPlanGeneratorMapWidgetViewModel(loc, cfg));
    }
}