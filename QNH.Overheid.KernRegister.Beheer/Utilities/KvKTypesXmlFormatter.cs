using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using QNH.Overheid.KernRegister.Business.SignaalService;
using StructureMap.TypeRules;

namespace QNH.Overheid.KernRegister.Beheer.Utilities
{
    public class KvKTypesXmlFormatter : MediaTypeFormatter
    {
        private static readonly Type[] Types = typeof(SignaalType).Assembly.GetTypes().Where(t =>
                    t.CanBeCastTo(typeof(SignaalType))
                    || t.CanBeCastTo(typeof(BerichtType))).ToArray();

        private static readonly IEnumerable<XmlSerializer> Serializers = Types.Select(t => new XmlSerializer(t));

        public KvKTypesXmlFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
        }

        public override bool CanReadType(Type type) => Types.Any(t => t == type);

        public override bool CanWriteType(Type type) => Types.Any(t => t == type);

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    readStream.CopyTo(memoryStream);
                    var xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                    using (var xmlReader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(xml), new XmlDictionaryReaderQuotas()))
                    {
                        var serializer = Serializers.First(s => s.CanDeserialize(xmlReader));
                        var result = serializer.Deserialize(xmlReader);
                        taskCompletionSource.SetResult(result);
                    }
                }
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            return taskCompletionSource.Task;
        }
    }
}