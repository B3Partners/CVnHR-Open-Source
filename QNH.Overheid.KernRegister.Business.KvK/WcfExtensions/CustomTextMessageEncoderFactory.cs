using System.ServiceModel.Channels;

namespace QNH.Overheid.KernRegister.Business.KvK.WcfExtensions
{
    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        private MessageEncoderFactory _inner;
        internal CustomTextMessageEncoderFactory(MessageEncoderFactory inner)
        {
            _inner = inner;
        }

        public override MessageEncoder Encoder => new CustomTextMessageEncoder(_inner.Encoder);

        public override MessageVersion MessageVersion => _inner.MessageVersion;

        internal string MediaType => Encoder.MediaType;
    }

}
