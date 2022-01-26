using Microsoft.Extensions.DependencyInjection;
using SubstrateCore.Repository;
using SubstrateCore.Services;
using SubstrateCore.ViewModels;
using System;
using System.Collections.Concurrent;
using Windows.UI.ViewManagement;

namespace SubstrateCore.Configuration
{
    public class ServiceLocator : IDisposable
    {
        static private readonly ConcurrentDictionary<int, ServiceLocator> _serviceLocators = new ConcurrentDictionary<int, ServiceLocator>();

        static private ServiceProvider _rootServiceProvider = null;

        static public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDataRepositoryFactory, DataRepositoryFactory>();

            serviceCollection.AddSingleton<ISettingsService, SettingsService>();
            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<ILogService, LogService>();
            serviceCollection.AddSingleton<IDialogService, DialogService>();
            serviceCollection.AddSingleton<IFilePickerService, FilePickerService>();
            serviceCollection.AddScoped<IContextService, ContextService>();
            serviceCollection.AddScoped<ICommonServices, CommonServices>();

            serviceCollection.AddSingleton<IProjectService, ProjectService>();
            serviceCollection.AddSingleton<ISearchPathService, SearchPathService>();

            serviceCollection.AddTransient<TargetPathPageViewModel>();
            serviceCollection.AddTransient<SearchFilePathViewModel>();
            serviceCollection.AddTransient<RemoveReferenceViewModel>();
            serviceCollection.AddTransient<SettingViewModel>();
            serviceCollection.AddTransient<GetReferenceViewModel>();
            serviceCollection.AddTransient<AddReferenceViewModel>();
            serviceCollection.AddTransient<ToolViewModel>();

            _rootServiceProvider = serviceCollection.BuildServiceProvider();
        }

        static public ServiceLocator Current
        {
            get
            {
                int currentViewId = ApplicationView.GetForCurrentView().Id;
                return _serviceLocators.GetOrAdd(currentViewId, key => new ServiceLocator());
            }
        }

        static public void DisposeCurrent()
        {
            int currentViewId = ApplicationView.GetForCurrentView().Id;
            if (_serviceLocators.TryRemove(currentViewId, out ServiceLocator current))
            {
                current.Dispose();
            }
        }

        private IServiceScope _serviceScope = null;

        private ServiceLocator()
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
