using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LystenApi.Models
{
    public class MasterModel
    {
        public MasterModel()
        {
        }

        public int CountryId { get; internal set; }
        public string Name { get; internal set; }
        public int Id { get; internal set; }
        public bool? IsActive { get; internal set; }
        public string CountryCode { get; internal set; }
        public int StateId { get; internal set; }

        public string Value { get; set; }
        public string SuperUnitName { get; internal set; }
        public string SubUnitName { get; internal set; }
        public string CountryName { get; internal set; }
        public string StateName { get; internal set; }
        public string CityName { get; internal set; }
        public int CityId { get; internal set; }
        public string MachineName { get; internal set; }
        public string CategoryName { get; internal set; }
        public string CompanyCount { get; internal set; }
        public string CategoryCount { get; internal set; }
        public decimal TotalSale { get; internal set; }
        public string MachineCount { get; internal set; }
        public string ItemCount { get; internal set; }
        public string ProductionTotal { get; internal set; }
        public string DeliveryChalanCount { get; internal set; }
        public int Policy { get; internal set; }
        public string PolicyCount { get; internal set; }
        public string TotalRate { get; internal set; }
        public string PartyCount { get; set; }
        public string BrokerCount { get; set; }
        public string CurrencySymbol { get; set; }
        public string PreFix { get; set; }
        public string InFix { get; set; }
        public string PostFix { get; set; }
        public string StartDate { get; internal set; }
        public string EndDate { get; internal set; }
    }
}