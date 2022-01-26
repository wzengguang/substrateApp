using System;

namespace SubstrateCore.Services
{
    public class CommonServices : ICommonServices
    {
        public CommonServices(IContextService contextService, IMessageService messageService, IDialogService dialogService, ILogService logService)
        {
            ContextService = contextService;
            MessageService = messageService;
            DialogService = dialogService;
            LogService = logService;
        }

        public IContextService ContextService { get; }


        public IMessageService MessageService { get; }

        public IDialogService DialogService { get; }

        public ILogService LogService { get; }
    }
}
