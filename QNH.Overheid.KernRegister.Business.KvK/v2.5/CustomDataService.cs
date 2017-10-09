using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.ServiceModel.Channels;
using System.Xml;
using QNH.Overheid.KernRegister.Business.KvK.WcfExtensions;

namespace QNH.Overheid.KernRegister.Business.KvK.Service
{
    public class CustomDataService : DataserviceClient
    {
        public CustomDataService() 
            : base("KvKDataServiceSoap")
        {
            var binding = Endpoint.Binding as CustomBinding;
            if(binding == null) throw new ConfigurationErrorsException("Incorrect WCF configuration. Use CustomBinding instead!");
            var textMessageEncoder = binding.Elements.OfType<TextMessageEncodingBindingElement>().First();
            binding.Elements.Remove(textMessageEncoder);
            binding.Elements.Insert(1, new CustomTextMessageEncoderBindingElement(textMessageEncoder));

            // Override all contract protectionLevels to signing only !@#$@#GRMBL!@#$@#
            Endpoint.Contract.ProtectionLevel = ProtectionLevel.Sign;
            foreach (var operationDescription in Endpoint.Contract.Operations)
            {
                operationDescription.ProtectionLevel = ProtectionLevel.Sign;
                foreach (var msg in operationDescription.Messages)
                {
                    msg.ProtectionLevel = ProtectionLevel.Sign;
                    foreach (var header in msg.Headers)
                    {
                        header.ProtectionLevel = ProtectionLevel.Sign;
                    }
                    foreach (var messageProperty in msg.Properties)
                    {
                        messageProperty.ProtectionLevel = ProtectionLevel.Sign;
                    }
                }
            }

            // Remove the urn: part from the messageID header !@#$@#GRMBL!@#$@#
            Endpoint.Behaviors.Add(new CustomInspectMessageBehavior((request) =>
            {
                request.Headers.MessageId = new UniqueId(request.Headers.MessageId.ToString().Replace("urn:", ""));
                request.Headers.ReplyTo = null;
            }));

            // Specify fallback mechanism
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
        }
    }
}
