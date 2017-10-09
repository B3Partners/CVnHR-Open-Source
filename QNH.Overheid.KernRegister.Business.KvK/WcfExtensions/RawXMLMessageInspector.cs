using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;

namespace QNH.Overheid.KernRegister.Business.KvK.WcfExtensions
{
    public sealed class RawXMLMessageInspector : IClientMessageInspector
    {
        public static volatile string Directory;

        #region Singleton implementation
        
        private static readonly object Lock = new object();
        private static volatile RawXMLMessageInspector _instance;

        public static RawXMLMessageInspector Instance
        {
            get
            {
                if (_instance == null)
                    lock (Lock)
                        if (_instance == null)
                            _instance = new RawXMLMessageInspector();

                return _instance;
            }
        }

        private RawXMLMessageInspector()
        { }

        #endregion

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var xDoc = XDocument.Load(XmlReader.Create(new StringReader(reply.ToString())));
            var kvkNummer = xDoc.Descendants().FirstOrDefault(node => node.Name.LocalName == "referentie")?.Value;

            var path = Directory;
            if (!path.EndsWith(".xml"))
            {
                path = $"{path}\\{DateTime.Now:yyyy-MM-dd-HHmmss}-{kvkNummer}.xml";
            }

            //geef het resultaat als string weer
            using (var sw = File.AppendText(path)) { 
                
                //var result = XD.Descendants().FirstOrDefault(p => p.Name.LocalName == "inhoud");
                //XD = null;
                sw.Write(xDoc);
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel) => null;
    }
}
