using System;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace QNH.Overheid.KernRegister.Business.KvK.WcfExtensions
{
    public class RawXmlActionBehavior : IEndpointBehavior
    {
        private Action<XDocument> InspectAction { get; set; }

        public RawXmlActionBehavior(Action<XDocument> inspectAction)
        {
            InspectAction = inspectAction;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // NOOP
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            if (InspectAction != null)
            {
                clientRuntime.MessageInspectors.Add(new RawXmlMessageActionInspector() { InspectAction = InspectAction });
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            // NOOP
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // NOOP
        }
    }
}