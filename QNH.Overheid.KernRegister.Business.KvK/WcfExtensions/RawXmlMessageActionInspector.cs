using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;

namespace QNH.Overheid.KernRegister.Business.KvK.WcfExtensions
{
    public class RawXmlMessageActionInspector : IClientMessageInspector
    {
        /// <summary>
        /// Specify an action to (asynchronously) inspect the xml message
        /// </summary>
        public Action<XDocument> InspectAction { get; set; }

        public object BeforeSendRequest(ref Message request, IClientChannel channel) => null;

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (InspectAction != null)
            {
                var replyString = reply.ToString();
                using (var stringReader = new StringReader(replyString))
                    using (var xmlReader = XmlReader.Create(stringReader))
                    {
                        var xDoc = XDocument.Load(xmlReader);
                        InspectAction(xDoc);
                    }
            }
        }
    }
}