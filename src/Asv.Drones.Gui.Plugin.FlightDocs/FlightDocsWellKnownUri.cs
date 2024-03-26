using Asv.Drones.Gui.Api;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

public static class FlightDocsWellKnownUri
{
    private const string PageMap = $"{WellKnownUri.ShellPage}.map";
    
    public const string PageMapFlightZone = $"{PageMap}.flight-zone";
    public const string PageMapDialogs = $"{PageMap}.dialogs";

    //Anchor actions
    private const string PageMapFlightZoneLayer = $"{PageMapFlightZone}/layer";
    
    public const string PageMapFlightZoneLayerZonePolygon = $"{PageMapFlightZoneLayer}/flight-zone-polygon";
    public const string PageMapFlightZoneLayerRuler = $"{PageMapFlightZoneLayer}/ruler/?id{{0}}";


    //Actions
    private const string PageMapActions = $"{PageMap}.actions";
    
    public const string PageMapActionsFlightZone = $"{PageMapActions}.flight-zone";
    public const string PageMapActionsTakeOfLand = $"{PageMapActions}.take-of-land";
}