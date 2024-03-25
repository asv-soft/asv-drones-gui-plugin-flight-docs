﻿using System.Composition;
using Asv.Drones.Gui.Api;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportShellPage(UriString)]
public class FlightPlanViewModel : ViewModelBase
{
    private const string UriString = FlightZoneMapViewModel.UriString + ".dialogs.flight-zone";
        
    [ImportingConstructor]
    public FlightPlanViewModel(string result) : base(new Uri(UriString))
    {
        Result = result;
    }
    
    [Reactive]
    public string Result { get; set; }
}