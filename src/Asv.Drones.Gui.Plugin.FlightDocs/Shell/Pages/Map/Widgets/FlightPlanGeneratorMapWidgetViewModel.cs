using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Composition;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Asv.Cfg;
using Asv.Common;
using Asv.Drones.Gui.Api;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

public class FlightPlanConfig
{
    public DateTimeOffset FlightStartDate { get; set; } = DateTimeOffset.Now;

    public TimeSpan FlightStartTime { get; set; } = DateTimeOffset.Now.TimeOfDay;
    public int FlightMinAltitude { get; set; }
    public int FlightMaxAltitude { get; set; }
    public double FlightTime { get; set; }
    public string AirportCode { get; set; }
    public string CompanyName { get; set; }
    public List<RegNumber> RegNumbers { get; set; } = new();
    public string VrMrNumber { get; set; }
    public string AdditionalInfo { get; set; }
    public string UavOperatorName { get; set; }
    public string Altitude { get; set; }
}

public class RegNumber
{
    public string RegistrationNumber { get; set; }
}

public class FlightPlanGeneratorMapWidgetViewModel : MapWidgetBase
{
    public const string UriString = $"{FlightDocsWellKnownUri.PageMapFlightZone}.flight-plan";
    private FlightZoneMapViewModel _flightZoneMap;
    private readonly ILocalizationService _loc;
    private readonly FlightPlanConfig _flightPlanConfig;
    private readonly IConfiguration _cfg;
    private bool _isChanged;

    public FlightPlanGeneratorMapWidgetViewModel() : base(new Uri(UriString))
    {
    }

    [ImportingConstructor]
    public FlightPlanGeneratorMapWidgetViewModel(ILocalizationService loc, IConfiguration cfg) : this()
    {
        _loc = loc;
        _cfg = cfg;
        Icon = MaterialIconKind.FlightMode;
        Title = RS.FlightPlanGeneratorMapWidgetViewModel_Title;
        Location = WidgetLocation.Right;
        AltitudeUnits = loc.Altitude.CurrentUnit.Value.Unit;
        RegNumbers = new ObservableCollection<RegNumber>();

        #region Load From Config

        _flightPlanConfig = cfg.Get<FlightPlanConfig>();
        FlightStartDate = _flightPlanConfig.FlightStartDate;
        FlightStartTime = _flightPlanConfig.FlightStartTime;
        FlightMinAltitude = _flightPlanConfig.FlightMinAltitude.ToString();
        FlightMaxAltitude = _flightPlanConfig.FlightMaxAltitude.ToString();
        FlightTime = _flightPlanConfig.FlightTime;
        AirportCode = _flightPlanConfig.AirportCode;
        CompanyName = _flightPlanConfig.CompanyName;
        VrMrNumber = _flightPlanConfig.VrMrNumber;
        AdditionalInfo = _flightPlanConfig.AdditionalInfo;
        UavOperatorName = _flightPlanConfig.UavOperatorName;
        Altitude = _flightPlanConfig.Altitude;

        foreach (var regNumber in _flightPlanConfig.RegNumbers)
        {
            RegNumbers.Add(new RegNumber() { RegistrationNumber = regNumber.RegistrationNumber });
        }

        #endregion

        #region Commands

        GenerateFlightPlanCommand = ReactiveCommand.Create(GenerateFlightPlan);
        AddRegNumberCommand = ReactiveCommand.Create(() =>
        {
            RegNumbers.Add(new RegNumber());
            _flightZoneMap.IsChanged = true;
        });
        RemoveRegNumberCommand = ReactiveCommand.Create(() =>
        {
            if (RegNumbers.Count > 0)
            {
                RegNumbers.Remove(RegNumbers.Last());
                _flightZoneMap.IsChanged = true;
            }
        });
        SaveToCfgCommand = ReactiveCommand.Create(SaveToCfg);

        #endregion


        this.WhenValueChanged(vm => vm.FlightTimeString).Subscribe(v =>
        {
            if (!string.IsNullOrWhiteSpace(v) & double.TryParse(v, out var result))
            {
                FlightTime = result;
            }
        }).DisposeItWith(Disposable);

        this.WhenValueChanged(vm => vm.FlightTime).Subscribe(v =>
        {
            FlightTimeString = v.ToString(CultureInfo.InvariantCulture);
        }).DisposeItWith(Disposable);

        #region Validation

        this.ValidationRule(_ => _.FlightMinAltitude, _ =>
        {
            int.TryParse(_, CultureInfo.InvariantCulture, out var result);
            return result > 0;
        }, _ => RS.FlightPlanGeneratorMapWidgetViewModel_Validation).DisposeItWith(Disposable);

        this.ValidationRule(_ => _.FlightMaxAltitude, _ =>
        {
            int.TryParse(_, CultureInfo.InvariantCulture, out var result);
            return result > 0;
        }, _ => RS.FlightPlanGeneratorMapWidgetViewModel_Validation).DisposeItWith(Disposable);

        this.ValidationRule(_ => _.FlightTime, _ => _ is > 0,
            _ => RS.FlightPlanGeneratorMapWidgetViewModel_Validation).DisposeItWith(Disposable);

        this.ValidationRule(x => x.FlightTime,
            _ => !string.IsNullOrWhiteSpace(_.ToString()),
            RS.FlightPlanGeneratorMapWidgetViewModel_Validation).DisposeItWith(Disposable);

        this.ValidationRule(_ => _.FlightTimeString, _ =>
        {
            double.TryParse(_, out var result);
            return result > 0;
        }, _ => RS.FlightPlanGeneratorMapWidgetViewModel_Validation).DisposeItWith(Disposable);

        #endregion
    }

    private void SaveToCfg()
    {
        _flightPlanConfig.FlightStartDate = FlightStartDate;
        _flightPlanConfig.FlightStartTime = FlightStartTime;
        _flightPlanConfig.FlightMinAltitude = int.Parse(FlightMinAltitude);
        _flightPlanConfig.FlightMaxAltitude = int.Parse(FlightMaxAltitude);
        _flightPlanConfig.FlightTime = FlightTime;
        _flightPlanConfig.AirportCode = AirportCode;
        _flightPlanConfig.CompanyName = CompanyName;
        _flightPlanConfig.RegNumbers = new();
        foreach (var regNumber in RegNumbers)
        {
            _flightPlanConfig.RegNumbers.Add(regNumber);
        }
        _flightPlanConfig.VrMrNumber = VrMrNumber;
        _flightPlanConfig.AdditionalInfo = AdditionalInfo;
        _flightPlanConfig.UavOperatorName = UavOperatorName;
        _flightPlanConfig.Altitude = Altitude;

        _cfg.Set(_flightPlanConfig);
        _flightZoneMap.SaveToCfg();
    }

    protected override void InternalAfterMapInit(IMap context)
    {
        _flightZoneMap = (FlightZoneMapViewModel)context;
        
        this.WhenValueChanged(vm => vm.AirportCode).Subscribe(_ =>
            {
                if (_flightPlanConfig.AirportCode != _) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.CompanyName).Subscribe(_ =>
            {
                if (_flightPlanConfig.CompanyName != _) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.AdditionalInfo).Subscribe(_ =>
            {
                if (_flightPlanConfig.AdditionalInfo != _) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.Altitude).Subscribe(_ =>
            {
                if (_flightPlanConfig.Altitude != _) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.UavOperatorName).Subscribe(_ =>
            {
                if (_flightPlanConfig.UavOperatorName != _) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.VrMrNumber).Subscribe(_ =>
            {
                if (_flightPlanConfig.VrMrNumber != _) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.FlightStartDate).Subscribe(_ =>
            {
                if (!_flightPlanConfig.FlightStartDate.Equals(_)) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.FlightMaxAltitude).Subscribe(_ =>
            {
                if (string.IsNullOrWhiteSpace(_)) return;
                if (!_loc.Altitude.IsValid(_)) return;
                if (_flightPlanConfig.FlightMaxAltitude != int.Parse(_)) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.FlightMinAltitude).Subscribe(_ =>
            {
                if (_.IsNullOrWhiteSpace()) return;
                if (!_loc.Altitude.IsValid(_)) return;
                if (_flightPlanConfig.FlightMinAltitude != int.Parse(_)) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.FlightStartTime).Subscribe(_ =>
            {
                if (!_flightPlanConfig.FlightStartTime.Equals(_)) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);
        this.WhenValueChanged(vm => vm.FlightTime).Subscribe(_ =>
            {
                if (Math.Abs(_flightPlanConfig.FlightTime - _) > 0) _flightZoneMap.IsChanged = true;
            })
            .DisposeItWith(Disposable);

        _flightZoneMap.WhenValueChanged(vm => vm.IsChangeSave).Subscribe(v =>
        {
            if (v) SaveToCfg();
        });
    }
    


    public static string PrintLatitude(double latitude)
    {
        int degrees = (int)Math.Abs(latitude);
        double remainingDegrees = Math.Abs(latitude) - degrees;
        int minutes = (int)(remainingDegrees * 60);
        double remainingMinutes = (remainingDegrees * 60) - minutes;
        double seconds = Math.Round(remainingMinutes * 60, 2);
        while (seconds >= 60d)
        {
            minutes++;
            seconds -= 60;
        }
        return $"{degrees:00}{minutes:00}{seconds:00}{(latitude < 0 ? "S" : "N")}";
    }

    public static string PrintLongitude(double longitude)
    {
        int degrees = (int)Math.Abs(longitude);
        double remainingDegrees = Math.Abs(longitude) - degrees;
        int minutes = (int)(remainingDegrees * 60);
        double remainingMinutes = (remainingDegrees * 60) - minutes;
        double seconds = Math.Round(remainingMinutes * 60, 2);
        while (seconds >= 60d)
        {
            minutes++;
            seconds -= 60;
        }
        return $"{degrees:000}{minutes:00}{seconds:00}{(longitude < 0 ? "W" : "E")}";
    }

    private async void GenerateFlightPlan()
    {
        var flightZone = string.Empty;

        foreach (var anchor in _flightZoneMap.FlightZoneAnchors.Items)
        {
            if (anchor is FlightZoneAnchor flightZoneAnchor)
            {
                flightZone += PrintLatitude(flightZoneAnchor.Location.Latitude) +
                              PrintLongitude(flightZoneAnchor.Location.Longitude) + " ";
            }
        }

        var regNumbers = string.Empty;

        foreach (var regNumber in RegNumbers)
        {
            regNumbers += regNumber.RegistrationNumber + " ";
        }

        var takeOffPoint = string.Empty;
        var landPoint = string.Empty;

        if (_flightZoneMap.TakeOffLandAnchors.Count > 0)
        {
            takeOffPoint = PrintLatitude(_flightZoneMap.TakeOffLandAnchors.Items.First().Location.Latitude) +
                           PrintLongitude(_flightZoneMap.TakeOffLandAnchors.Items.First().Location.Longitude);
        }

        if (_flightZoneMap.TakeOffLandAnchors.Count > 1)
        {
            landPoint = PrintLatitude(_flightZoneMap.TakeOffLandAnchors.Items.Last().Location.Latitude) +
                        PrintLongitude(_flightZoneMap.TakeOffLandAnchors.Items.Last().Location.Longitude);
        }

        var regNumbersWithUavs = string.Empty;
        for (var i = 0; i < RegNumbers.Count; i++)
        {
            regNumbersWithUavs += $"БВС №{i + 1} {RegNumbers[i].RegistrationNumber} ";
        }

        var resultString = "(SHR-ZZZZZ\n" +
                           $"-ZZZZ{FlightStartTime:hhmm}\n" +
                           $"-M{FlightMinAltitude:0000}/M{FlightMaxAltitude:0000} /ZONA {flightZone.Trim()}/\n" +
                           $"-ZZZZ{FlightTime}\n" +
                           $"-DOF/{FlightStartDate.Date:yyMMdd} DEP/{takeOffPoint} DEST/{landPoint}EET/{AirportCode} TYP/BLA{RegNumbers.Count} OPR/{CompanyName} REG/{regNumbers} RMK/{VrMrNumber} доп. инфо.: {AdditionalInfo} ОПЕРАТОР БВС {UavOperatorName}, ВЫСОТА: {Altitude}. {regNumbersWithUavs.Trim()}";

        var dialog = new ContentDialog()
        {
            Title = RS.FlightPlanViewModel_Title,
            PrimaryButtonText = RS.FlightPlanViewModel_PrimaryButtonText
        };

        using var viewModel = new FlightPlanViewModel(resultString);
        dialog.Content = viewModel;
        await dialog.ShowAsync();
    }

    [Reactive] public DateTimeOffset FlightStartDate { get; set; } = DateTimeOffset.Now;
    [Reactive] public TimeSpan FlightStartTime { get; set; } = DateTimeOffset.Now.TimeOfDay;
    [Reactive] public string FlightMinAltitude { get; set; }
    [Reactive] public string FlightMaxAltitude { get; set; }
    [Reactive] private double FlightTime { get; set; }
    [Reactive] public string FlightTimeString { get; set; }
    [Reactive] public string AirportCode { get; set; }
    [Reactive] public string CompanyName { get; set; }
    [Reactive] public ObservableCollection<RegNumber> RegNumbers { get; set; }
    [Reactive] public string VrMrNumber { get; set; }
    [Reactive] public string AdditionalInfo { get; set; }
    [Reactive] public string UavOperatorName { get; set; }
    [Reactive] public string Altitude { get; set; }
    [Reactive] public string AltitudeUnits { get; set; }
    [Reactive] public ICommand GenerateFlightPlanCommand { get; set; }
    [Reactive] public ICommand AddRegNumberCommand { get; set; }
    [Reactive] public ICommand RemoveRegNumberCommand { get; set; }
    [Reactive] public ICommand SaveToCfgCommand { get; set; }
}