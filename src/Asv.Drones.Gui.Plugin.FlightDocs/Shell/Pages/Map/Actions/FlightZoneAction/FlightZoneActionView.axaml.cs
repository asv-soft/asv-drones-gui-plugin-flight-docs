using Asv.Drones.Gui.Api;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportView(typeof(FlightZoneActionViewModel))]
public partial class FlightZoneActionView : ReactiveUserControl<FlightZoneActionViewModel>
{
    public FlightZoneActionView()
    {
        InitializeComponent();
    }
}