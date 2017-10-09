using System;
using System.IO;
using System.ServiceModel.Channels;

namespace QNH.Overheid.KernRegister.Business.KvK.WcfExtensions
{
    public class CustomTextMessageEncoder : MessageEncoder
    {
        private readonly MessageEncoder _inner;
        public CustomTextMessageEncoder(MessageEncoder inner)
        {
            _inner = inner;
        }

        public override string ContentType => "text/xml";

        public override string MediaType => _inner.MediaType;

        public override MessageVersion MessageVersion => _inner.MessageVersion;

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            var message = _inner.ReadMessage(buffer, bufferManager, contentType);
            message.Headers.Clear();
            return message;
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            var message = _inner.ReadMessage(stream, maxSizeOfHeaders, contentType);
            message.Headers.Clear();
            return message;
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            var retVal = _inner.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
            return retVal;
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            _inner.WriteMessage(message, stream);
        }
    }

}
