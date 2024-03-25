using System.Composition;
using Asv.Common;
using Asv.Drones.Gui.Api;
using DynamicData;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(FlightZoneMapViewModel.UriString, typeof(IViewModelProvider<IMapAnchor>))]
public class FlightZoneMapAnchorProvider : ViewModelProviderBase<IMapAnchor>
{
    [ImportingConstructor]
    public FlightZoneMapAnchorProvider()
    {
        
    }

    public void Update(SourceList<IMapAnchor> anchors)
    {
        anchors.Connect()
            .OnItemAdded(_ => Source.AddOrUpdate(_))
            .OnItemRemoved(_ => Source.Remove(_))
            .Subscribe()
            .DisposeItWith(Disposable);
    }
}