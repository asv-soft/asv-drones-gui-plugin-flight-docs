using System.Composition;
using Asv.Drones.Gui.Api;
using Material.Icons;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(typeof(IShellMenuItem))]
public class FlightZoneShellMenuItem : ShellMenuItem
{
    public FlightZoneShellMenuItem() : base($"{WellKnownUri.ShellMenu}.flight-docs")
    {
        Name = RS.FlightZoneShellMenuItem_Header_Title;
        NavigateTo = FlightZoneMapViewModel.Uri;
        Icon = MaterialIconDataProvider.GetData(MaterialIconKind.DocumentSign);
        Position = ShellMenuPosition.Top;
        Type = ShellMenuItemType.PageNavigation;
        Order = 10;
    }
}