using System;

namespace QNH.Overheid.KernRegister.Business.Business
{
    public class RecordProcessedEventArgs : EventArgs
    {
        public int Progress { get; set; }
        public string InschrijvingNaam { get; set; }
        public int SuccesProgress { get; set; }
        public int SuccesCount { get; set; }
        public int ErrorCount { get; set; }
        public int TotalNew { get; set; }
        public int TotalUpdated { get; set; }
        public int TotalAlreadyExisted { get; set; }
    }
}