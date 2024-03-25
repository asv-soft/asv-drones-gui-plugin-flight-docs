using Asv.Drones.Gui.Api;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportView(typeof(FlightZoneMapWidgetViewModel))]
public partial class FlightZoneMapWidgetView : ReactiveUserControl<FlightZoneMapWidgetViewModel>
{
    public FlightZoneMapWidgetView()
    {
        InitializeComponent();
    }
}