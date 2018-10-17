using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace QNH.Overheid.KernRegister.Business.KvK.Service
{
    public class CustomMessageInspector : IClientMessageInspector
    {
        private readonly Action<Message> _inspectMessageAction;

        public CustomMessageInspector(Action<Message> inspectMessageAction = null)
        {
            _inspectMessageAction = inspectMessageAction;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _inspectMessageAction?.Invoke(request);
            return request;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }
}