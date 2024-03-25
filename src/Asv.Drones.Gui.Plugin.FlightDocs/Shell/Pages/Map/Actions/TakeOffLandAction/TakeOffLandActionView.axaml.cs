using Asv.Drones.Gui.Api;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportView(typeof(TakeOffLandActionViewModel))]
public partial class TakeOffLandActionView : ReactiveUserControl<TakeOffLandActionViewModel>
{
    public TakeOffLandActionView()
    {
        InitializeComponent();
    }
}