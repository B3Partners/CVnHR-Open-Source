using System.ServiceModel.Description;

namespace QNH.Overheid.KernRegister.Business.KvK.WcfExtensions
{
    public class RawXMLBehavior : IEndpointBehavior
    {
        private string _path;

        public RawXMLBehavior(string path)
        {
            _path = path;
        }
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // NOOP
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            //Adding the inspector only makes sense if there is a path where the result can be stored
            if (!string.IsNullOrWhiteSpace(_path))
            {
                RawXMLMessageInspector.Directory = _path;
                clientRuntime.MessageInspectors.Add(RawXMLMessageInspector.Instance);
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
