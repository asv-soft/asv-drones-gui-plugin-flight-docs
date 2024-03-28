using System.Composition;
using Asv.Cfg;
using Asv.Common;
using Asv.Drones.Gui.Api;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

public interface IFlightZoneMap
{
    SourceList<IMapAnchor> FlightZoneAnchors { get; }
    SourceList<IMapAnchor> TakeOffLandAnchors { get; }
}

public class FlightZonePoint
{
    public GeoPoint Location { get; set; }
    public string Name { get; set; }
}

public class TakeOffLandPoint
{
    public GeoPoint Location { get; set; }
    public string Name { get; set; }
    public TakeOffLand TakeOffLand { get; set; }
}

public class FlightZoneMapViewModelConfig
{
    public double Zoom { get; set; }
    public GeoPoint MapCenter { get; set; }
    public List<FlightZonePoint> FlightZoneAnchors { get; set; } = new ();
    public List<TakeOffLandPoint> TakeOffLandAnchors { get; set; } = new ();
}

[ExportShellPage(UriString)]
public class FlightZoneMapViewModel : MapPageViewModel, IFlightZoneMap
{
    public const string UriString = $"{FlightDocsWellKnownUri.PageMapFlightZone}";
    public static readonly Uri Uri = new(UriString);

    private readonly SourceList<IMapAnchor> _flightZoneAnchors = new();
    private readonly SourceList<IMapAnchor> _takeOffLandAnchors = new();
    private readonly IConfiguration _cfg;
    private readonly FlightZoneMapViewModelConfig _flightConfig;

    public FlightZoneMapViewModel()
    {
        if (Design.IsDesignMode)
        {
            
        }
    }

    [ImportingConstructor]
    public FlightZoneMapViewModel(IMapService map, IConfiguration cfg, ILocalizationService loc,
        [ImportMany(UriString)] IEnumerable<IViewModelProvider<IMapStatusItem>> status,
        [ImportMany(UriString)] IEnumerable<IViewModelProvider<IMapMenuItem>> exportedMenuItems,
        [ImportMany(UriString)] IEnumerable<IViewModelProvider<IMapAnchor>> markers,
        [ImportMany(UriString)] IEnumerable<IViewModelProvider<IMapWidget>> widgets,
        [ImportMany(UriString)] IEnumerable<IViewModelProvider<IMapAction>> actions) : base(Uri, map, status, exportedMenuItems, markers, widgets, actions)
    {
        _cfg = cfg;
        _flightConfig = cfg.Get<FlightZoneMapViewModelConfig>();
        Zoom = _flightConfig.Zoom is 0 ? 1 : _flightConfig.Zoom;
        Center = _flightConfig.MapCenter;

        for (var i = 0; i < _flightConfig.FlightZoneAnchors.Count; i++)
        {
            _flightZoneAnchors.Add(new FlightZoneAnchor(Guid.NewGuid().ToString(), _flightConfig.FlightZoneAnchors[i].Location, i, loc) {Name = _flightConfig.FlightZoneAnchors[i].Name});
        }
        
        foreach (var t in _flightConfig.TakeOffLandAnchors)
        {
            _takeOffLandAnchors.Add(new TakeOffLandAnchor(Guid.NewGuid().ToString(), t.Location, t.TakeOffLand, loc) {Name = t.Name});
        }
        
        _flightZoneAnchors.Add(new FlightZonePolygon());
        
        foreach (var provider in markers)
        {
            if (provider is FlightZoneMapAnchorProvider flightZoneProvider)
            {
                flightZoneProvider.Update(_flightZoneAnchors);
            }
            
            if (provider is TakeOffLandMapAnchorProvider takeOffLandProvider)
            {
                takeOffLandProvider.Update(_takeOffLandAnchors);
            }
        }
        
        this.WhenPropertyChanged(vm => vm.Zoom)
            .Subscribe(_ =>
            {
                _flightConfig.MapCenter = Center;
                _flightConfig.Zoom = Zoom;
                cfg.Set(_flightConfig);
            })
            .DisposeItWith(Disposable);
            
        this.WhenPropertyChanged(vm => vm.Center)
            .Subscribe(_ =>
            {
                _flightConfig.MapCenter = Center;
                _flightConfig.Zoom = Zoom;
                cfg.Set(_flightConfig);
            })
            .DisposeItWith(Disposable);
    }

    public void SaveToCfg()
    {
        _flightConfig.FlightZoneAnchors = new ();
        foreach (var anchor in FlightZoneAnchors.Items)
        {
            if (anchor is FlightZoneAnchor flightZoneAnchor)
            {
                _flightConfig.FlightZoneAnchors.Add(new FlightZonePoint() {Location = flightZoneAnchor.Location, Name = flightZoneAnchor.Name});
            }
        }
            
        _flightConfig.TakeOffLandAnchors = new ();
        foreach (var anchor in TakeOffLandAnchors.Items)
        {
            var takeOffLandAnchor = (TakeOffLandAnchor)anchor;
            _flightConfig.TakeOffLandAnchors.Add(new TakeOffLandPoint() {Location = takeOffLandAnchor.Location, Name = takeOffLandAnchor.Name, TakeOffLand = takeOffLandAnchor.TakeOffLand});
        }
        _cfg.Set(_flightConfig);
        IsChanged = false;
    }
    
    public override async Task<bool> TryClose()
    {
        if (!IsChanged) return true;
        var dialog = new ContentDialog()
        {
            Title = RS.FlightZoneMapViewModel_DataLossDialog_Title,
            Content = RS.FlightZoneMapViewModel_DataLossDialog_Content,
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = RS.FlightZoneMapViewModel_DataLossDialog_PrimaryButtonText,
            SecondaryButtonText = RS.FlightZoneMapViewModel_DataLossDialog_SecondaryButtonText,
            CloseButtonText = RS.FlightZoneMapViewModel_DataLossDialog_CloseButtonText
        };
        var result = await dialog.ShowAsync();
        switch (result)
        {
            case ContentDialogResult.Primary:
                IsChangeSave = true;
                SaveToCfg();
                return true;
            case ContentDialogResult.Secondary:
                return true;
            case ContentDialogResult.None:
                return false;
            default:
                return true;
        }
    }
    
    [Reactive]
    public  bool IsChanged { get; set; }
    [Reactive]
    public bool IsChangeSave { get; set; }
    public SourceList<IMapAnchor> FlightZoneAnchors => _flightZoneAnchors;
    public SourceList<IMapAnchor> TakeOffLandAnchors => _takeOffLandAnchors;
}