using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace QNH.Overheid.KernRegister.Business.Integration.WcfExtensions
{
    public sealed class CookieManagerMessageInspector : IClientMessageInspector
    {
        private string _sharedCookie;

        #region Singleton implementation

        private static object _lock =new object();
        private static volatile CookieManagerMessageInspector _instance;

        public static CookieManagerMessageInspector Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new CookieManagerMessageInspector();
                
                return _instance;
            }
        }

        private CookieManagerMessageInspector()
        {}

        #endregion

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            HttpResponseMessageProperty httpResponse =
                reply.Properties[HttpResponseMessageProperty.Name]
                as HttpResponseMessageProperty;

            if (httpResponse != null)
            {
                string cookie = httpResponse.Headers[HttpResponseHeader.SetCookie];

                if (!string.IsNullOrEmpty(cookie))
                {
                    this._sharedCookie = cookie;
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest;

            // The HTTP request object is made available in the outgoing message only
            // when the Visual Studio Debugger is attacched to the running process
            if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                request.Properties.Add(HttpRequestMessageProperty.Name,
                    new HttpRequestMessageProperty());
            }

            httpRequest = (HttpRequestMessageProperty)
                request.Properties[HttpRequestMessageProperty.Name];
            httpRequest.Headers.Add(HttpRequestHeader.Cookie, this._sharedCookie);

            return null;
        }
    }
}
