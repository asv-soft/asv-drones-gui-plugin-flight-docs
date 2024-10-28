using System.Composition;
using Asv.Cfg;
using Asv.Drones.Gui.Api;
using Microsoft.Extensions.Logging;

namespace Asv.Drones.Gui.Plugin.FlightDocs;

[PluginEntryPoint("FlightDocs")]
[Shared]
public class FlightDocsPlugin : IPluginEntryPoint
{
    private ILogger _log;
    
    [ImportingConstructor]
    public FlightDocsPlugin(ILoggerFactory factory, IConfiguration cfg, IApplicationHost host)
    {
      _log = factory.CreateLogger<FlightDocsPlugin>();
    }
    public async void Initialize()
    {
       

    }

    public void Init()
    {
        
    }

    public void OnFrameworkInitializationCompleted()
    {
        
    }

    public void OnShutdownRequested()
    {
        
    }
}