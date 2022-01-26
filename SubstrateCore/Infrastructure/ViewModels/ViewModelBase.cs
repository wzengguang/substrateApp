
using SubstrateCore.ViewModels;
using System;
using System.Diagnostics;
using Windows.System;

namespace SubstrateCore.Services
{
    public class ViewModelBase : ObservableObject
    {
        protected DispatcherQueue DispatcherQueue { get; }

        private Stopwatch _stopwatch = new Stopwatch();

        public ViewModelBase(ICommonServices commonServices)
        {
            ContextService = commonServices.ContextService;
            MessageService = commonServices.MessageService;
            DialogService = commonServices.DialogService;
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public IContextService ContextService { get; }
        public IMessageService MessageService { get; }
        public IDialogService DialogService { get; }

        public bool IsMainView => ContextService.IsMainView;

        virtual public string Title => String.Empty;

        public void StartStatusMessage(string message)
        {
            StatusMessage(message);
            _stopwatch.Reset();
            _stopwatch.Start();
        }
        public void EndStatusMessage(string message)
        {
            _stopwatch.Stop();
            StatusMessage($"{message} ({_stopwatch.Elapsed.TotalSeconds:#0.000} seconds)");
        }

        public void StatusReady()
        {
            MessageService.Send(this, "StatusMessage", "Ready");
        }
        public void StatusMessage(string message)
        {
            MessageService.Send(this, "StatusMessage", message);
        }
        public void StatusError(string message)
        {
            MessageService.Send(this, "StatusError", message);
        }

        public void EnableThisView(string message = null)
        {
            message = message ?? "Ready";
            MessageService.Send(this, "EnableThisView", message);
        }
        public void DisableThisView(string message)
        {
            MessageService.Send(this, "DisableThisView", message);
        }

        public void EnableOtherViews(string message = null)
        {
            message = message ?? "Ready";
            MessageService.Send(this, "EnableOtherViews", message);
        }
        public void DisableOtherViews(string message)
        {
            MessageService.Send(this, "DisableOtherViews", message);
        }

        public void EnableAllViews(string message = null)
        {
            message = message ?? "Ready";
            MessageService.Send(this, "EnableAllViews", message);
        }
        public void DisableAllViews(string message)
        {
            MessageService.Send(this, "DisableAllViews", message);
        }
    }
}
