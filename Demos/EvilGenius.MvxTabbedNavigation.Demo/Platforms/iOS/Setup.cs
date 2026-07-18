using EvilGenius.MvxTabbedNavigation.Demo.Core;
using EvilGenius.MvxTabbedNavigation.Platforms.iOS.Presenters;
using Microsoft.Extensions.Logging;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.Plugin.Color;
using Serilog;
using Serilog.Extensions.Logging;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace EvilGenius.MvxTabbedNavigation.Demo.Platforms.iOS;

public class Setup : MvxIosSetup<App>
{
    protected override ILoggerFactory CreateLogFactory()
    {
        // serilog configuration
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .CreateLogger();

        return new SerilogLoggerFactory();
    }

    protected override ILoggerProvider CreateLogProvider() => new SerilogLoggerProvider();

    protected override IMvxIosViewPresenter CreateViewPresenter() => new TabbedViewPresenter(Window!);

//The plugin assemblies are skipped in net6.0. Why?
    public override IEnumerable<Assembly> GetPluginAssemblies() 
        => base.GetPluginAssemblies().MergeWith(typeof(MvxNativeColorValueConverter));
}