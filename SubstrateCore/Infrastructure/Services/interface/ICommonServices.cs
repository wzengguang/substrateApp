
using System;

namespace SubstrateCore.Services
{
    public interface ICommonServices
    {
        IContextService ContextService { get; }
        IMessageService MessageService { get; }
        IDialogService DialogService { get; }
    }
}
