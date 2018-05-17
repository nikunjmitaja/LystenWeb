using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LystenApi.Db;
using LystenApi.Models;

namespace LystenApi.ViewModel
{

    public class CategoryEventImageVM
    {
        public CategoryEventImageVM()
        {
        }

        public int CategoryId { get; set; }
        public string Image { get; set; }
        public int Id { get;  set; }
    }


    public class RequestCallModel
    {
        public RequestCallModel()
        {
        }

        public IEnumerable<object> TimeZone { get; set; }
        public IEnumerable<object> Minutes { get; set; }
    }

    public class ReportUserViewModel
    {
        public ReportUserViewModel()
        {
        }

        public int ToUserId { get; set; }
        public int FromUserId { get; set; }
        public string Description { get; set; }

    }

    public class SettingViewModel
    {
        public SettingViewModel()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }

    }



    public class TimeZoneInfoModel
    {
        public TimeZoneInfoModel()
        {
        }
        public string DisplayName { get; set; }
        public string Id { get; set; }
    }

    public class CallingPriceMastersModel
    {
        public CallingPriceMastersModel()
        {
        }

        public string Time { get; set; }
        public int Id { get; set; }
        public string Name { get;  set; }
        public string Amount { get;  set; }
    }

    public class CallingPriceViewModel
    {
        public CallingPriceViewModel()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
    public class EventMainModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string CreatedDate { get; set; }
        public string Image { get; set; }
        public string CategoryName { get;  set; }
        public int CategoryId { get;  set; }
        public string CategoryImageForEvent { get;  set; }
    }


    public class EventViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Location { get; set; }
        public string CreatedDate { get; set; }
        public string Image { get; set; }
        public string CategoryName { get; set; }
    }

    public class GroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupTypeId { get; set; }
        public string CreatorId { get; set; }
        public string Image { get; set; }
        public string CategoryId { get; set; }
        public int IsOwner { get; set; }
        public int IsMember { get;  set; }
    }

    public class UserGroupViewModel
    {
        public int GroupId { get; set; }
        public string UsersId { get; set; }
        public int CreatorId { get; set; }
        public string IsAdded { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MasterViewModel
    {
        public string ItemCount { get; set; }
        public string PolicyCount { get; set; }
        public string ProductionTotal { get; set; }
        public string PartyCount { get; set; }
        public string DeliveryChalanCount { get; set; }
        public string CurrencySymbol { get; set; }
        public string TotalRate { get; set; }
        public NestedViewModel Chart { get; set; }
    }

    public class NestedViewModel
    {
        public List<SalesViewModel> Sales { get; set; }
        public List<ProductionViewModel> Production { get; set; }
    }

    public class ProductionViewModel
    {
        public string Name { get; set; }
        public double Amount { get; set; }
    }

    public class SalesViewModel
    {
        public string Name { get; set; }
        public double Amount { get; set; }
    }

    public class CountryViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class StateViewModel
    {
        public string CountryName { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string CountryId { get; set; }
    }
    public class CityViewModel
    {
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string StateId { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string CountryId { get; set; }
    }
    public class PartyMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string PanNo { get; set; }
        public string PinCode { get; set; }
        public bool IsActive { get; set; }
    }
    public class CategoryMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
    public class MachineMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class UnitMasterViewModel
    {
        public int Id { get; set; }
        public string SuperUnit { get; set; }
        public bool IsActive { get; set; }
    }

    public class PolicyMasterViewModelSave
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
    public class PolicyMasterViewModel
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
    }
    public class CompanyMasterViewModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public bool? IsActive { get; set; }
        public string WebSite { get; set; }
        public int Id { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
    public class ItemMasterViewModel
    {
        //public int Id { get; set; }
        //public string Name { get; set; }
        //public int CategoryId { get; set; }
        //public string CategoryName { get; set; }
        //public string AliasName { get; set; }
        //public string SubUnitName { get; set; }
        //public string UnitName { get; set; }
        //public int UnitId { get; set; }
        //public int SubUnitId { get; set; }
        //public string Value { get; set; }
        //public bool InBatches { get; set; }
        //public string OpeningStock { get; set; }
        //public string Meter { get; set; }
        //public string Quantity { get; set; }
        //public string AvgRate { get; set; }
        //public int CompanyId { get; set; }
        //public int CreatedBy { get; set; }
        //public System.DateTime CreatedDate { get; set; }
        //public System.DateTime ModifyDate { get; set; }
        //public bool IsActive { get; set; }
        //public List<InwardOutwardMasterModel> InwardData { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string AliasName { get; set; }
        public string CategoryName { get; set; }
        public string NetWeight { get; set; }
        public string Meter { get; set; }
        public string InwardDate { get; set; }
        public string UnitName { get; set; }
        public string SubUnitName { get; set; }
        public string OpeningStock { get; set; }
        public string AvgRate { get; set; }
        public bool IsActive { get; set; }
        public string Value { get; set; }
        public string Quantity { get; set; }
        public string OpeningStockDetail { get; set; }
    }


    public class Item_MasterModel
    {
        public int Id { get; internal set; }
        public string OpeningStock { get; set; }
        public string Meter { get; set; }
        public string Quantity { get; set; }
        public string AvgRate { get; set; }
        public bool? InBatches { get; internal set; }
        public string CategoryName { get; internal set; }
        public string SuperUnitName { get; internal set; }
        public string SubUnitName { get; internal set; }
        public string Name { get; internal set; }
        public int? CategoryId { get; internal set; }
        public string AliasName { get; internal set; }
        public int? UnitId { get; internal set; }
        public int? SubUnitId { get; internal set; }
        public string Rate { get; internal set; }
        public bool? IsActive { get; internal set; }
        public string UnitValue { get; internal set; }
        public string Value { get; internal set; }
        public List<InwardOutward_MasterModel> InwardOutward { get; set; }
    }

    public class InwardOutward_MasterModel
    {
        public int Id { get; set; }
        public string BatchNo { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string Cartonno { get; set; }
        public Nullable<decimal> Netweight { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Meter { get; set; }
        public Nullable<decimal> OpeningStock { get; set; }
        public string Quantity { get; set; }
        public string InwardDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public string Name { get; internal set; }
        public string Value { get; internal set; }
    }
    public class InwardOutwardMasterModelList
    {
        public int ItemId { get; set; }
        public int Id { get; set; }
        public string Cartonno { get; set; }
        public string BatchNo { get; set; }
        public bool IsActive { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Meter { get; set; }
        public string Quantity { get; set; }
        public string Value { get; set; }
        public decimal? Rate { get; set; }
        public string Unit { get; set; }
        public string ItemName { get; set; }
    }
    public class BatchList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public double NetWeight { get; set; }
        public double Meter { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Value { get; set; }
    }



    public class ProductionModelSave
    {
        public string MachineName { get; set; }
        public int Id { get; set; }
        public int? MachineId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public bool? IsActive { get; set; }
        public dynamic Meters { get; set; }
        public dynamic NetWeight { get; set; }
        public string ProductionNo { get; set; }
        public string Companyname { get; set; }
        public string Cartonno { get; set; }
        public Nullable<decimal> Netweight { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Meter { get; set; }
        public int CreatedBy { get; set; }
        public string ProductionDate { get; set; }
    }

    public class DeliveryModelList
    {
        public int Id { get; set; }
        public int Challandetailid { get; set; }
        public string Challanno { get; set; }
        public int Partyid { get; set; }
        public string Partyname { get; set; }
        public string PDFFIle { get; set; }
        public double Totalnetwet { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; }
    }
    public class DatumDelivery
    {
        public string ItemName { get; set; }
        public string NetWeight { get; set; }
        public string Meter { get; set; }
        public string Rate { get; set; }
        public string ProductionNumber { get; set; }
    }

    public class DeliveryNote
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class DeliveryModelSave
    {

        public int Id { get; set; }
        public int Challandetailid { get; set; }
        public string Challanno { get; set; }
        public int Partyid { get; set; }
        public string Partyname { get; set; }
        public string PartyAddress { get; set; }
        public string PDFFIle { get; set; }
        public double Totalnetwet { get; set; }
        public string Unit { get; set; }
        public string Deliveryname { get; set; }
        public string Deliveryaddress { get; set; }
        public string PinCode { get; set; }

        public string Brokername { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }

        public string PartyCityName { get; set; }
        public string PartyStateName { get; set; }
        public string PartyCountryName { get; set; }


        public System.DateTime Deliverydate { get; set; }
        public string Transport { get; set; }
        public string LRNo { get; set; }
        public System.DateTime LRDate { get; set; }
        public string Vehicalno { get; set; }
        public string Totalmeter { get; set; }
        public string InvoiceNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string PolicyNumber { get; set; }
        public string SupplierRef { get; set; }
        public string Destination { get; set; }
        public string OtherRef { get; set; }
        public string BuyerOrderNo { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string PlaceReceiptByShipper { get; set; }
        public string CityPortOfLoading { get; set; }
        public string CityPortOfDischarge { get; set; }
        public string PaymentTerm { get; set; }
        public string TermsOfDelivery { get; set; }
        public string SalestaxNo { get; set; }
        public string CstNo { get; set; }

        public List<DatumDelivery> Data { get; set; }

    }
    public class DeliveryModelSaveEdit
    {

        public int Id { get; set; }
        public int Challandetailid { get; set; }
        public string Challanno { get; set; }
        public int Partyid { get; set; }
        public string Partyname { get; set; }
        public string PartyAddress { get; set; }
        public string PDFFIle { get; set; }
        public double Totalnetwet { get; set; }
        public string Unit { get; set; }
        public string Deliveryname { get; set; }
        public string Deliveryaddress { get; set; }
        public string PinCode { get; set; }

        public string Brokername { get; set; }
        public string BrokerAddress { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }

        public string PartyCityName { get; set; }
        public string PartyStateName { get; set; }
        public string PartyCountryName { get; set; }


        public string Deliverydate { get; set; }
        public string Transport { get; set; }
        public string LRNo { get; set; }
        public string LRDate { get; set; }
        public string Vehicalno { get; set; }
        public string Totalmeter { get; set; }
        public string InvoiceNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string PolicyNumber { get; set; }
        public string SupplierRef { get; set; }
        public string Destination { get; set; }
        public string OtherRef { get; set; }
        public string BuyerOrderNo { get; set; }
        public string OrderDate { get; set; }
        public string PlaceReceiptByShipper { get; set; }
        public string CityPortOfLoading { get; set; }
        public string CityPortOfDischarge { get; set; }
        public string PaymentTerm { get; set; }
        public string TermsOfDelivery { get; set; }
        public string SalestaxNo { get; set; }
        public string CstNo { get; set; }

        public List<DatumDelivery> Data { get; set; }

    }

    public class DeliveryNewModel
    {
        public int Id { get; set; }
        public int Challandetailid { get; set; }
        public string Challanno { get; set; }
        public int Partyid { get; set; }
        public string Partyname { get; set; }
        public string PDFFIle { get; set; }
        public double Totalnetwet { get; set; }
        public string Unit { get; set; }
        public string Deliveryname { get; set; }
        public string Deliveryaddress { get; set; }
        public string PinCode { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public System.DateTime Deliverydate { get; set; }
        public string Transport { get; set; }
        public string LRNo { get; set; }
        public System.DateTime LRDate { get; set; }
        public string Vehicalno { get; set; }
        public string Totalmeter { get; set; }
        public string InvoiceNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string PolicyNumber { get; set; }
        public string SupplierRef { get; set; }
        public string Destination { get; set; }
        public string OtherRef { get; set; }
        public string BuyerOrderNo { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string PlaceReceiptByShipper { get; set; }
        public string CityPortOfLoading { get; set; }
        public string CityPortOfDischarge { get; set; }
        public string PaymentTerm { get; set; }
        public string TermsOfDelivery { get; set; }
        public string SalestaxNo { get; set; }
        public string CstNo { get; set; }

        public bool IsActive { get; set; }
        public int? Brokerid { get; set; }
        public int ProductionId { get; set; }
        public int? ItemId { get; set; }
        public decimal? Rate { get; set; }
        public string Data { get; set; }
    }
    public class DeliverychallanDDL
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class Party_MasterDDL
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class InvoiceModelList
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public decimal TotalNetWeight { get; set; }
        public string Unit { get; set; }
        public decimal TotalMeter { get; set; }
        public decimal TotalRate { get; set; }
        public bool IsActive { get; set; }
        public string PartyName { get; set; }
        public string PDFFIle { get; set; }
    }


    public class ProductionNoDDL
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double NetWeight { get; set; }
        public double Meters { get; set; }
        public bool IsActive { get; set; }
    }

    public class InvoiceNoDDL
    {
        public string Name { get; set; }
    }
    public class RootObject1
    {
        public List<Datum> Data { get; set; }

    }
    public class Datum
    {
        public int Id { get; set; }
        public string LotNo { get; set; }
        public string Type { get; set; }
        public decimal Rate { get; set; }
        public int ItemId { get; set; }
        public int CompanyId { get; set; }
        public string Cartonno { get; set; }
        public decimal Netweight { get; set; }
        public string Unit { get; set; }
        public decimal Meter { get; set; }
        public decimal OpeningStock { get; set; }
        public string Quantity { get; set; }
        public System.DateTime InwardDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime ModifyDate { get; set; }
    }
    public class ProductionDetailModel
    {
        public ProductionDetailModel()
        {
        }
        public double TotalWeight { get; set; }
        public double TotalMeter { get; set; }
        public dynamic ProductionD { get; set; }

    }

    public class ProductionDetailModelForPDF
    {
        public string ProductionNo { get; set; }
        public Nullable<decimal> Netweight { get; set; }
        public Nullable<decimal> Meter { get; set; }
        public string MachineName { get; set; }
        public string ItemName { get; set; }
        public string ProductionDate { get; set; }
        public string SuperUnit { get; set; }
    }
}