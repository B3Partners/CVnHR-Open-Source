using System;

namespace QNH.Overheid.KernRegister.Business.Service.KvK
{
    internal class MissingHoofdVestigingException : Exception
    {
        public MissingHoofdVestigingException()
        {
        }

        public MissingHoofdVestigingException(string message) : base(message)
        {
        }

        public MissingHoofdVestigingException(string message, MissingHoofdVestigingException innerException)
            : base(message, innerException)
        {
        }
    }
}