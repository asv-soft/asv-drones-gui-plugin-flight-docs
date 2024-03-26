using System.Collections.Specialized;
using System.Composition;
using Asv.Drones.Gui.Api;
using DynamicData;
using Material.Icons;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[ExportShellPage(UriString)]
public class FlightPlanViewModel : ViewModelBase, IShellPage
{
    private const string UriString = FlightDocsWellKnownUri.PageMapDialogs;

    [ImportingConstructor]
    public FlightPlanViewModel(string result) : base(new Uri(UriString))
    {
        Result = result;
    }
    
    [Reactive]
    public string Result { get; set; }

    public void SetArgs(NameValueCollection args)
    {
    }

    public Task<bool> TryClose()
    {
        return Task.FromResult(true);
    }

    public MaterialIconKind Icon { get; }
    public string Title { get; }
    public IObservable<IChangeSet<IMenuItem, Uri>> HeaderItems { get; }
    public IObservable<IChangeSet<IShellStatusItem, Uri>> StatusItems { get; }
}