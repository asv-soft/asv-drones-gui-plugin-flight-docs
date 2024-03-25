using Asv.Drones.Gui.Api;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportView(typeof(FlightPlanGeneratorMapWidgetViewModel))]
public partial class FlightPlanGeneratorMapWidgetView : ReactiveUserControl<FlightPlanGeneratorMapWidgetViewModel>
{
    public FlightPlanGeneratorMapWidgetView()
    {
        InitializeComponent();
    }
}