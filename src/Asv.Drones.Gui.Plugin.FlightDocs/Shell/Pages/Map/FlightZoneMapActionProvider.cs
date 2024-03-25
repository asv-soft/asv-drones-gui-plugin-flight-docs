using System.Composition;
using Asv.Drones.Gui.Api;
using DynamicData;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(FlightZoneMapViewModel.UriString, typeof(IViewModelProvider<IMapAction>))]
public class FlightZoneMapActionProvider : ViewModelProviderBase<IMapAction>
{
    [ImportingConstructor]
    public FlightZoneMapActionProvider([ImportMany(FlightZoneMapViewModel.UriString)]IEnumerable<IMapAction> items)
    {
        Source.AddOrUpdate(items);
    }
}