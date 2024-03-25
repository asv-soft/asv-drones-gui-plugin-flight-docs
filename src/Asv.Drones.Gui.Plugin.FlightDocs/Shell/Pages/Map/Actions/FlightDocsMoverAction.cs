﻿using System.Composition;
using Asv.Drones.Gui.Api;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[Export(FlightZoneMapViewModel.UriString,typeof(IMapAction))]
[method: ImportingConstructor]
public class FlightDocsMoverAction() : MapMoverActionViewModel($"{FlightZoneMapViewModel.UriString}.action.mover");
