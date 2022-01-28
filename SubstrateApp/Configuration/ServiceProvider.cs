using Microsoft.Extensions.DependencyInjection;
using SubstrateCore.Repositories;
using SubstrateCore.Services;
using SubstrateApp.ViewModels;
using System;
using System.Collections.Concurrent;
using Windows.UI.ViewManagement;

namespace SubstrateApp.Configuration
{
    public class ServiceProvider : IDisposable
    {
        static private readonly ConcurrentDictionary<int, ServiceProvider> _serviceLocators = new ConcurrentDictionary<int, ServiceProvider>();

        static private Microsoft.Extensions.DependencyInjection.ServiceProvider _rootServiceProvider = null;

        static public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ProjectRepository>();
            serviceCollection.AddSingleton<IProjectService, ProjectService>();
            serviceCollection.AddSingleton<ISearchPathService, SearchPathService>();
            serviceCollection.AddTransient<TargetPathPageViewModel>();
            serviceCollection.AddTransient<SearchFilePathViewModel>();
            serviceCollection.AddTransient<RemoveReferenceViewModel>();
            serviceCollection.AddTransient<SettingViewModel>();
            serviceCollection.AddTransient<GetReferenceViewModel>();
            serviceCollection.AddTransient<AddReferenceViewModel>();
            serviceCollection.AddTransient<ToolViewModel>();
            serviceCollection.AddTransient<SettingViewModel>();
            serviceCollection.AddSingleton<ScanViewModel>();

            _rootServiceProvider = serviceCollection.BuildServiceProvider();
        }

        static public ServiceProvider Current
        {
            get
            {
                int currentViewId = ApplicationView.GetForCurrentView().Id;
                return _serviceLocators.GetOrAdd(currentViewId, key => new ServiceProvider());
            }
        }

        static public void DisposeCurrent()
        {
            int currentViewId = ApplicationView.GetForCurrentView().Id;
            if (_serviceLocators.TryRemove(currentViewId, out ServiceProvider current))
            {
                current.Dispose();
            }
        }

        private IServiceScope _serviceScope = null;

        private ServiceProvider()
        {
            _serviceScope = _rootServiceProvider.CreateScope();
        }

        public T GetService<T>()
        {
            return GetService<T>(true);
        }

        public T GetService<T>(bool isRequired)
        {
            if (isRequired)
            {
                return _serviceScope.ServiceProvider.GetRequiredService<T>();
            }
            return _serviceScope.ServiceProvider.GetService<T>();
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_serviceScope != null)
                {
                    _serviceScope.Dispose();
                }
            }
        }
        #endregion
    }
}
