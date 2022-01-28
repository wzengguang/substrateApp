using Microsoft.Extensions.DependencyInjection;
using SubstrateCore.Repositories;

namespace SubstrateApp.Configuration
{
    static public class Startup
    {
        static private readonly ServiceCollection _serviceCollection = new ServiceCollection();

        public static void ConfigureAsync()
        {
            //AppCenter.Start("7b48b5c7-768f-49e3-a2e4-7293abe8b0ca", typeof(Analytics), typeof(Crashes));
            //Analytics.TrackEvent("AppStarted");

            ServiceProvider.Configure(_serviceCollection);

            //var projectService = ServiceLocator.Current.GetService<IProjectService>();
            SQLiteDb.InitializeDb();
        }
    }
}