using eBay.Service.Core.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DropshipCommon;
using eBay.Service.Core.Soap;
using eBay.Service.Call;

namespace DropshipBusiness.eBay
{
    public interface IeBayAPIContextProvider
    {
        IList<ApiContext> GetAPIContext();

        ApiContext GetAPIContextBySellerID(string sellerID);
    }

    public interface IeBayAPICallManager
    {
        //Download New eBay Orders by period
        OrderType[] DownloadNeweBayOrder(DateTime? fromTime, DateTime? toTime);

        //Get eBay Orders by orderlineitemID
        OrderType[] GeteBayOrdersByOrderLineItemID(IList<OrderLineItemIDObject> orderLineItemIDs);

        //complete sale with tracking number
        bool UpdateeBayShipment(IList<ShipmentDetail> shipmentDetails);

        //update ebay item with local infomation
        bool UpdateeBayItem(UpdateeBayItemRequest request);

        //get active seller listing
        ItemType[] GeteBayActiveListing(GeteBayActiveListingRequest request);

        //get active seller listing
        ItemType[] GeteBaySellerListBySKUs(GeteBaySellerListBySKUsRequest request);
    }

    public class OrderLineItemIDObject
    {
        public string OrderLineItemID { get; set; }
        public string SellerID { get; set; }
    }

    public class ShipmentDetail
    {
        public string OrderLineItemID { get; set; }
        public string ShipmentTrackingNumber { get; set; }
        public string ShippingCarrierUsed { get; set; }
        public DateTime ShippedTime { get; set; }
        public string SellerID { get; set; }
        
    }

    public class eBayRequestBase
    {
        public string SellerID { get; set; }
    }

    public class UpdateeBayItemRequest : eBayRequestBase
    {
        public ItemType[] Items { get; set; }
    }

    public class GeteBayActiveListingRequest : eBayRequestBase
    { 
    
    }

    public class GeteBaySellerListBySKUsRequest : eBayRequestBase
    {
        public IList<string> SKUs { get; set; }
    }

    public class eBayAPIContextProvider : IeBayAPIContextProvider
    {
        public IList<ApiContext> GetAPIContext()
        {
            var lsteBayAPIContextConfig= DropshipConfig.Instance.eBayAPIConfigList;
            var lstApiContext = new List<ApiContext>();

            foreach (var config in lsteBayAPIContextConfig)
            {
                lstApiContext.Add(GetAPIContextByConfig(config));
            }

            return lstApiContext;
        }

        public ApiContext GetAPIContextBySellerID(string sellerID)
        {
            if (!string.IsNullOrEmpty(sellerID))
            {
                var lsteBayAPIContextConfig = DropshipConfig.Instance.eBayAPIConfigList;
                var config = lsteBayAPIContextConfig.FirstOrDefault(c => c.SellerID.Equals(sellerID));
                if (config != null)
                {
                    return GetAPIContextByConfig(config);
                }
            }
            return default(ApiContext);
        }

        private ApiContext GetAPIContextByConfig(eBayAPIContextConfig config)
        {
            var apiContext = new ApiContext();
            var apiCredential = new ApiCredential();
            apiCredential.eBayToken = config.eBayToken;
            apiCredential.eBayAccount.UserName = config.SellerID;
            apiContext.SoapApiServerUrl = config.ServiceURL;


            apiContext.ApiCredential = apiCredential;
            apiContext.Site = (SiteCodeType)Enum.Parse(typeof(SiteCodeType), config.eBaySiteID);
            return apiContext;
        }
    }


    public class eBayAPICallManager : IeBayAPICallManager
    {

        private readonly IeBayAPIContextProvider _eBayAPIContextProvider;

        public eBayAPICallManager(IeBayAPIContextProvider eBayAPIContextProvider)
        {
            _eBayAPIContextProvider = eBayAPIContextProvider;
        }

        public OrderType[] DownloadNeweBayOrder(DateTime? fromTime, DateTime? toTime)
        {
            
            try
            {
                GetOrdersCall getOrdersCall;
                GetOrdersRequestType getOrdersRequest;
                PaginationType paging = new PaginationType();
                
                int totalPage;

                DetailLevelCodeTypeCollection detailLevelColl = new DetailLevelCodeTypeCollection();
                detailLevelColl.Add(DetailLevelCodeType.ReturnAll);

                List<OrderType> returnOrders=new List<OrderType>();
                foreach (var apiContext in _eBayAPIContextProvider.GetAPIContext())
                {
                    
                    getOrdersCall = new GetOrdersCall(apiContext);
                    getOrdersRequest = new GetOrdersRequestType();
                    getOrdersRequest.OrderRole = TradingRoleCodeType.Seller;
                   
                    getOrdersRequest.DetailLevel = detailLevelColl;
                    //var getOrderRequestDatetimeNow = DateTime.Now.AddMinutes(-2);
                    if (fromTime != null)
                        getOrdersRequest.CreateTimeFrom = ((DateTime)fromTime).ToUniversalTime();
                    if (toTime != null)
                        getOrdersRequest.CreateTimeTo = ((DateTime)toTime).ToUniversalTime();



                    int pageNumber = 1;
                    do
                    {
                        paging.EntriesPerPage = 100;
                        paging.PageNumber = pageNumber;
                        getOrdersRequest.Pagination = paging;
                        var getOrdersResponse = getOrdersCall.ExecuteRequest(getOrdersRequest) as GetOrdersResponseType;


                        if (getOrdersResponse.OrderArray != null && getOrdersResponse.OrderArray.Count > 0)
                            returnOrders.AddRange(getOrdersResponse.OrderArray.ToArray());

                        totalPage = getOrdersResponse.PaginationResult.TotalNumberOfPages;
                        pageNumber++;
                    } while (pageNumber <= totalPage);
                }

                return returnOrders.ToArray();

                
            }
            catch (Exception ex)
            {
                return default(OrderType[]);
            }
            
        }

        public OrderType[] GeteBayOrdersByOrderLineItemID(IList<OrderLineItemIDObject> orderLineItemIDs)
        { 
            try
            {
                if(orderLineItemIDs==null||orderLineItemIDs.Count==0)
                    return default(OrderType[]);

                GetOrderTransactionsCall getOrderTransactionCall;
                GetOrderTransactionsRequestType getOrderTransactionRequest;

                DetailLevelCodeTypeCollection detailLevelColl = new DetailLevelCodeTypeCollection();
                detailLevelColl.Add(DetailLevelCodeType.ReturnAll);

                var apiContexts = _eBayAPIContextProvider.GetAPIContext();

                List<OrderType> returnOrders = new List<OrderType>();


                var orderLineItemIDGroup = orderLineItemIDs.GroupBy(i => i.SellerID);
                foreach (var sellerGroupItemID in orderLineItemIDGroup)
                {
                    var apiContext=apiContexts.Where(c => c.ApiCredential.eBayAccount.UserName.ToLower().Equals(sellerGroupItemID.Key.ToLower())).FirstOrDefault();
                    if (apiContext != null)
                    {
                        getOrderTransactionCall=new GetOrderTransactionsCall(apiContext);
                        getOrderTransactionRequest = new GetOrderTransactionsRequestType();
                        getOrderTransactionRequest.DetailLevel = detailLevelColl;

                        int pageSize = 20;
                        int totalPage=Convert.ToInt32( Math.Ceiling( Convert.ToDecimal( sellerGroupItemID.Count())/Convert.ToDecimal(pageSize)));
                        int pageNumber = 1;

                        do
                        {
                            getOrderTransactionRequest.ItemTransactionIDArray = new ItemTransactionIDTypeCollection();
                            var lstTransactionIDs = sellerGroupItemID.Select(gi => new ItemTransactionIDType() { OrderLineItemID = gi.OrderLineItemID }).Skip((pageNumber - 1) * pageSize).Take(pageSize);
                            getOrderTransactionRequest.ItemTransactionIDArray.AddRange(lstTransactionIDs.ToArray());

                            var getOrderTransactionResponse = getOrderTransactionCall.ExecuteRequest(getOrderTransactionRequest) as GetOrderTransactionsResponseType;
                            if (getOrderTransactionResponse.OrderArray != null && getOrderTransactionResponse.OrderArray.Count > 0)
                                returnOrders.AddRange(getOrderTransactionResponse.OrderArray.ToArray());

                            pageNumber++;
                        } while (pageNumber <= totalPage);
                        //foreach (var orderLineItemIDObj in sellerGroupItemID)
                        //{ 
                            
                        //}
                    }
                }


                return returnOrders.ToArray();

            }
            catch (Exception ex)
            {
                return default(OrderType[]);
            }
        }


        public bool UpdateeBayShipment(IList<ShipmentDetail> shipmentDetails)
        {
            try
            {
                CompleteSaleCall completeSaleCall;
                CompleteSaleRequestType completeSaleRequest;

                var apiContexts = _eBayAPIContextProvider.GetAPIContext();

                var shipmentDetailGroup = shipmentDetails.GroupBy(i => i.SellerID);
                foreach (var shipmentDetailGroupItem in shipmentDetailGroup)
                {
                    var apiContext = apiContexts.Where(c => c.ApiCredential.eBayAccount.UserName.ToLower().Equals(shipmentDetailGroupItem.Key.ToLower())).FirstOrDefault();
                    if (apiContext != null)
                    {
                        completeSaleCall = new CompleteSaleCall(apiContext);
                        foreach (var shipmentDetail in shipmentDetailGroupItem)
                        {
                            if (string.IsNullOrEmpty(shipmentDetail.OrderLineItemID))
                                continue;

                            completeSaleRequest = new CompleteSaleRequestType();
                            if (isOrderLineItemID(shipmentDetail.OrderLineItemID))
                            {
                                completeSaleRequest.OrderLineItemID = shipmentDetail.OrderLineItemID;
                            }
                            else
                            {
                                completeSaleRequest.OrderID = shipmentDetail.OrderLineItemID;
                            }

                            if (!string.IsNullOrEmpty(shipmentDetail.ShipmentTrackingNumber) && !string.IsNullOrEmpty(shipmentDetail.ShipmentTrackingNumber))
                            {
                                completeSaleRequest.Shipment = new ShipmentType();
                                completeSaleRequest.Shipment.ShipmentTrackingNumber = shipmentDetail.ShipmentTrackingNumber;
                                completeSaleRequest.Shipment.ShippingCarrierUsed = shipmentDetail.ShippingCarrierUsed;
                            }

                            completeSaleRequest.Shipped = true;

                            var completeSaleResponse = completeSaleCall.ExecuteRequest(completeSaleRequest) as CompleteSaleResponseType;
                            if (completeSaleResponse.Ack == AckCodeType.Failure || completeSaleResponse.Ack == AckCodeType.PartialFailure)
                            {
                                LogManager.Instance.Error("Update eBay Shipment Failed for Order "+shipmentDetail.OrderLineItemID+Environment.NewLine);
                                if (completeSaleResponse.Errors != null && completeSaleResponse.Errors.Count > 0)
                                {
                                    LogManager.Instance.Error("Error Detail:" + shipmentDetail.OrderLineItemID + " " + completeSaleResponse.Errors[0].LongMessage + Environment.NewLine);
                                }
                            }
                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        private bool isOrderLineItemID(string id)
        {
            return id.IndexOf("-") != -1 ;
        }


        public bool UpdateeBayItem(UpdateeBayItemRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SellerID))
                    throw new Exception("No Seller ID");
                var apiContext = _eBayAPIContextProvider.GetAPIContextBySellerID(request.SellerID);
                var reviseFixedPriceItemCall = new ReviseFixedPriceItemCall(apiContext);
                var deletedFields = new StringCollection();
                foreach(var updateItem in request.Items)
                {
                    try
                    {
                        reviseFixedPriceItemCall.ReviseFixedPriceItem(updateItem, deletedFields);
                        if (reviseFixedPriceItemCall.ApiResponse.Ack == AckCodeType.Failure || reviseFixedPriceItemCall.ApiResponse.Ack == AckCodeType.PartialFailure)
                        {
                            if (reviseFixedPriceItemCall.ApiResponse.Errors != null && reviseFixedPriceItemCall.ApiResponse.Errors.Count > 0)
                            {
                                throw new Exception("SKU: " + updateItem.SKU + " Error Message: " + reviseFixedPriceItemCall.ApiResponse.Errors[0].LongMessage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.Error("Item ID: " + updateItem.ItemID + (updateItem.SKU!=null?" SKU: "+updateItem.SKU:"") + " " + ex.ToString());
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ItemType[] GeteBayActiveListing(GeteBayActiveListingRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SellerID))
                    throw new Exception("No Seller ID");
                var apiContext = _eBayAPIContextProvider.GetAPIContextBySellerID(request.SellerID);
                var myeBaySellingCall = new GetMyeBaySellingCall(apiContext);
               
                myeBaySellingCall.ActiveList = new ItemListCustomizationType();
                myeBaySellingCall.ActiveList.ListingType = ListingTypeCodeType.FixedPriceItem;
                myeBaySellingCall.ActiveList.Include = true;

                var getItemCall = new GetItemCall(apiContext);
                getItemCall.DetailLevelList.Add(DetailLevelCodeType.ItemReturnDescription);

                int pageNumber = 1;
                int pageSize = 50;
                int totalPage = 1;

                PaginationType pageInfo = new PaginationType();

                var lstActiveItem = new List<ItemType>();
                

                do
                {
                    pageInfo.PageNumber = pageNumber;
                    pageInfo.EntriesPerPage = pageSize;
                    myeBaySellingCall.ActiveList.Pagination = pageInfo;
                    myeBaySellingCall.GetMyeBaySelling();

                    if (myeBaySellingCall.ActiveListReturn != null && myeBaySellingCall.ActiveListReturn.PaginationResult != null)
                    {
                        totalPage = myeBaySellingCall.ActiveListReturn.PaginationResult.TotalNumberOfPages;
                    }

                    if (myeBaySellingCall.ActiveListReturn != null &&
                            myeBaySellingCall.ActiveListReturn.ItemArray != null &&
                            myeBaySellingCall.ActiveListReturn.ItemArray.Count > 0)
                    {
                        foreach (ItemType actitem in myeBaySellingCall.ActiveListReturn.ItemArray)
                        {
                            try
                            {
                                var item = getItemCall.GetItem(actitem.ItemID);
                                if (item != null)
                                {
                                    lstActiveItem.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Instance.Error(ex.Message);
                            }
                        }
                    }

                    pageNumber++;
                } while (pageNumber <= totalPage);

                return lstActiveItem.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ItemType[] GeteBaySellerListBySKUs(GeteBaySellerListBySKUsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SellerID))
                    throw new Exception("No Seller ID");
                var apiContext = _eBayAPIContextProvider.GetAPIContextBySellerID(request.SellerID);

                var getItemCall = new GetItemCall(apiContext);
                getItemCall.DetailLevelList.Add(DetailLevelCodeType.ItemReturnDescription);

                var lstSellerListing = new List<ItemType>();
                var getSellerListCall = new GetSellerListCall(apiContext);
                getSellerListCall.EndTimeFrom = DateTime.Now.ToUniversalTime();
                getSellerListCall.EndTimeTo = DateTime.Now.AddDays(90).ToUniversalTime();
                getSellerListCall.SKUArrayList = new StringCollection();
                foreach (var sku in request.SKUs)
                {
                    getSellerListCall.SKUArrayList.Add(sku.ToUpper());
                }

                int pageNumber = 1;
                int pageSize = 50;
                int totalPage = 1;

                PaginationType pageInfo = new PaginationType();

                do
                {
                    pageInfo.PageNumber = pageNumber;
                    pageInfo.EntriesPerPage = pageSize;
                    getSellerListCall.Pagination = pageInfo;

                    getSellerListCall.GetSellerList();
                    if ( getSellerListCall.PaginationResult != null)
                    {
                        totalPage = getSellerListCall.PaginationResult.TotalNumberOfPages;
                    }

                    if (getSellerListCall.ItemList != null)
                    {
                        foreach (var listItem in getSellerListCall.ItemList.ToArray())
                        {
                            try
                            {
                                var item = getItemCall.GetItem(listItem.ItemID);
                                if (item != null)
                                {
                                    lstSellerListing.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Instance.Error(ex.Message);
                            }
                        }
                    }

                    pageNumber++;
                }
                while (pageNumber <= totalPage);

                return lstSellerListing.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
