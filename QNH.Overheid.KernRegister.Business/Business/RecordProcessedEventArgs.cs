using System;
using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Business.Business
{
    public class RecordProcessedEventArgs : EventArgs
    {
        public ProcessTaskTypes Type { get; set; }
        public int Progress { get; set; }
        public string InschrijvingNaam { get; set; }
        public int SuccesProgress { get; set; }
        public int SuccesCount { get; set; }
        public int ErrorCount { get; set; }
        public int TotalNew { get; set; }
        public int TotalUpdated { get; set; }
        public int TotalAlreadyExisted { get; set; }
        public bool IsError { get; set; }
        public string KvkNummer { get; set; }
        public List<string> Errors { get; set; }
    }
}