using Asv.Drones.Gui.Api;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportView(typeof(FlightPlanViewModel))]
public partial class FlightPlanView : ReactiveUserControl<FlightPlanViewModel>
{
    public FlightPlanView()
    {
        InitializeComponent();
    }
}