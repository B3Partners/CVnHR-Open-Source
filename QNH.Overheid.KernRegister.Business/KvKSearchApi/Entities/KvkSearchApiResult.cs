using System.Collections.Generic;

namespace QNH.Overheid.KernRegister.Business.KvKSearchApi.Entities
{
    public class KvkSearchApiResultWrapper
    {
        public string ApiVersion { get; set; }
        public object Meta { get; set; }
        public KvkSearchApiResult Data { get; set; }
    }

    public class KvkSearchApiResult
    {
        public IEnumerable<ApiItem> Items{get;set;}
        public int ItemsPerPage{get;set;}
        public int TotalItems{get;set;}
        public int StartPage{get;set;}
    }

    public class ApiItem
    {
        public string KvkNumber{get;set;}
        public string Rsin{get;set;}
        public ApiTradeNames TradeNames { get; set; }
        public bool HasEntryInBusinessRegister{get;set;}
        public bool HasNonMailingIndication{get;set;}
        public bool IsLegalPerson{get;set;}
        public bool IsBranch{get;set;}
        public bool IsMainBranch{get;set;}
        public IEnumerable<ApiAddress> Addresses{get;set;}
    }

    public class ApiTradeNames
    {
        public string ShortBusinessName { get; set; }
        public string BusinessName { get; set; }
        public IEnumerable<string> CurrentTradeNames { get; set; }
        public IEnumerable<string> CurrentStatutoryNames { get; set; }
    }

    public class ApiAddress
    {
        public string Type{get;set;}
        public string Street{get;set;}
        public string HouseNumber{get;set;}
        public string HouseNumberAddition{get;set;}
        public string PostalCode{get;set;}
        public string City{get;set;}
        public string Country{get;set;}
    }
}
