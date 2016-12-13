using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json;
//using DropshipCommon.Models;

namespace DropshipCommon
{
    public class Constants
    {
        public const char SplitChar = ',';
        public const string DropshipDBKey = "DropshipDB";
        public const string SystemUser = "System";
        public const string DropshipzoneItemListStandardURL = "http://dropshipzone.com.au/sample/Standard/sku_list.csv";
        public const string DropshipzoneItemListEnterpriseURL = "http://dropshipzone.com.au/sample/Enterprise/sku_list.csv";

        public const string ListingPriceReplaceString = "{{ItemPrice}}";
    }

    public enum ComponentLifeStyle
    {
        Singleton = 0,
        Transient = 1,
        LifetimeScope = 2
    }

    public enum YesNo
    { 
        Y,
        N
    }

    public enum Mode
    { 
        Auto,
        Manual
    }

    public enum GoogleResultFormat
    { 
        xml,
        json
    }

    public enum OrderStatus
    {
        CREATED=1,
        READY=2,
        SHIPPED=3,
        CANCELLED=4
    }

    public enum PaymentStatus
    {
        UNPAID=1,
        PAID=2,
        REFUND=3
    }

    public enum OMSOrderType
    {
        ECM=1,
        ECV=2,
        MANUAL=3,
        RESEND=4,
        FAULTY=5,
        CVMAGENTO=6,
        CMNOP=7
    }

    public enum ShippingMethod
    {
        LETTER = 1,
        PARCEL = 2,
        PICKUP = 3,
        EXPRESSLETTER=4,
        EXPRESSPARCEL=5,
        UNKNOWN=6
    }

    public enum PaymentMethod
    {
        PAYPAL = 1,
        BANKTRANSFER = 2,
        CASH = 3,
    }

    public enum NotifyType
    {
        Success,
        Error
    }


    //public sealed class OMSOrderType
    //{
    //    public const string eBayCrazyVictor = "ECV";
    //    public const string eBayCrazyMall = "ECM";
    //    public const string Manual = "MANUAL";
    //    public const string RESEND = "RESEND";
    //    public const string FAULTY = "FAULTY";
    //}

    public sealed class DropshipCacheKey
    {
        public const string ItemStatusList = "ITEM-STATUS-LIST";
        public const string ListingStatusList = "LISTING-STATUS-LIST";
        public const string SupplierList = "SUPPLIER-LIST";
        public const string ListingChannelList = "LISTING-CHANNEL-LIST";
    }

    public enum DropshipEntityType
    {
        ITEM,
        LISTING
    }

    //public sealed class ShippingCarrier
    //{
    //    public const string AusPost = "Australia Post";
    //    public const string Fastway = "FASTWAY COURIERS";
    //    public const string AlliedExpress = "ALLIED EXPRESS";
    //}

    //public enum ManifestStatus
    //{
    //    Created = 1,
    //    Freezed=2,
    //    Saved=3,
    //    Closed=4,
    //    Deleted=5
    //}

    //public enum ShipmentStatus
    //{
    //    Open=0,
    //    Processing=1,
    //    Cancelled=2,
    //    Closed=3
    //}

}
