using System;

namespace SubstrateCore.Services
{
    public class CommonServices : ICommonServices
    {
        public CommonServices(IContextService contextService, IMessageService messageService, IDialogService dialogService)
        {
            ContextService = contextService;
            MessageService = messageService;
            DialogService = dialogService;
        }

        public IContextService ContextService { get; }


        public IMessageService MessageService { get; }

        public IDialogService DialogService { get; }

    }
}
