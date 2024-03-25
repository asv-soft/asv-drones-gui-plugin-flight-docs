using System.Composition;
using Asv.Common;
using Asv.Drones.Gui.Api;
using DynamicData;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(FlightZoneMapViewModel.UriString, typeof(IViewModelProvider<IMapAnchor>))]
public class TakeOffLandMapAnchorProvider : ViewModelProviderBase<IMapAnchor>
{
    [ImportingConstructor]
    public TakeOffLandMapAnchorProvider()
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