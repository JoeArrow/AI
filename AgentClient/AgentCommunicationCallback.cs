using System;
using System.Threading;
using System.ServiceModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace AgentClient
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class AgentCommunicationCallback : IAgentCommunicationCallback
    {
        private SynchronizationContext _syncContext = AsyncOperationManager.SynchronizationContext;
        public event EventHandler<UpdatedListEventArgs> ServiceCallbackEvent;

        // ------------------------------------------------

        public void SendUpdatedList(List<string> items)
        {
            _syncContext.Post(new SendOrPostCallback(OnServiceCallbackEvent), new UpdatedListEventArgs(items));
        }

        // ------------------------------------------------

        private void OnServiceCallbackEvent(object state)
        {
            var handler = ServiceCallbackEvent;
            var e = state as UpdatedListEventArgs;

            if(handler != null)
            {
                handler(this, e);
            }
        }
    }
}
