using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace QNH.Overheid.KernRegister.Business.KvK.Service
{
    public class CustomInspectMessageBehavior : Attribute, IEndpointBehavior
    {
        private readonly Action<Message> _inspectMessageAction;

        public CustomInspectMessageBehavior(Action<Message> inspectMessageAction = null)
        {
            _inspectMessageAction = inspectMessageAction;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new CustomMessageInspector(_inspectMessageAction));
        }

    }
}