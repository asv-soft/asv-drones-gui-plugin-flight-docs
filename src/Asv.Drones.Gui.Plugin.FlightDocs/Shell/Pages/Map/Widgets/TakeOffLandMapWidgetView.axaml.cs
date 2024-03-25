using Asv.Drones.Gui.Api;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportView(typeof(TakeOffLandMapWidgetViewModel))]
public partial class TakeOffLandMapWidgetView : ReactiveUserControl<TakeOffLandMapWidgetViewModel>
{
    public TakeOffLandMapWidgetView()
    {
        InitializeComponent();
    }
}