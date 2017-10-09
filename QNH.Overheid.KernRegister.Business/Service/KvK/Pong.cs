using System;

namespace QNH.Overheid.KernRegister.Business.Service.KvK
{
    public class Pong
    {
        public bool IsSuccesful { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public object ExceptionMessage { get; set; }
    }
}