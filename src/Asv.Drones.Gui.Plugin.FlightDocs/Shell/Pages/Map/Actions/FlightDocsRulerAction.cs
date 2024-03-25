using System.Composition;
using Asv.Drones.Gui.Api;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(FlightZoneMapViewModel.UriString,typeof(IMapAction))]
[method: ImportingConstructor]
public class FlightDocsRulerAction(ILocalizationService loc) : MapRulerActionViewModel($"{FlightZoneMapViewModel.UriString}.action.ruler", loc);