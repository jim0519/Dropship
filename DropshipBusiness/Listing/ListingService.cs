using DropshipBusiness.eBay;
using DropshipBusiness.Item;
using DropshipCommon.Models;
using DropshipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon;
using eBay.Service.Core.Soap;
using DropshipBusiness.Events;

namespace DropshipBusiness.Listing
{
    public class ListingService:IListingService
    {
        private readonly IeBayAPICallManager _eBayAPICallManager;
        private readonly IRepository<D_Listing> _listingRepository;
        private readonly IRepository<T_ListingChannel> _listingChannelRepository;
        private readonly IItemService _itemService;
        private readonly IEventPublisher _eventPublisher;

        public ListingService(IeBayAPICallManager eBayAPICallManager,
            IRepository<D_Listing> listingRepository,
            IRepository<T_ListingChannel> listingChannelRepository,
            IItemService itemService,
            IEventPublisher eventPublisher)
        {
            _eBayAPICallManager = eBayAPICallManager;
            _listingRepository = listingRepository;
            _itemService = itemService;
            _listingChannelRepository = listingChannelRepository;
            _eventPublisher = eventPublisher;
        }

        public bool SyncLocalListingByChannel(int listingChannelID)
        {
            try
            {
                
                //get local listing
                var localListing = GetListingByChannelID(listingChannelID,false);

                //get local item
                var localItem = _itemService.GetAllItems();

                //get ebay active listing
                var request = new GeteBaySellerListBySKUsRequest() { SellerID = GetListingChannelByID(listingChannelID).Ref1 , SKUs=localItem.Select(s=>s.SKU).ToList()};
                var activeListing = _eBayAPICallManager.GeteBaySellerListBySKUs(request);

                //check if there is same sku in active online listing
                //var countSKU=activeListing.GroupBy(i=>i.SKU).Where(g=>g.Count()>1).Count();
                //if(countSKU>1)
                //    throw new Exception("Online Listing SKU Duplicated");
                //var countSKU = activeListing.GroupBy(i => i.SKU).Where(g => g.Count() > 1);

                //whether ebay listing match local listing(sku, itemID), update local listing with ebay item, if local item is disable, set online listing qty=0
                //whether ebay listing match local listing(sku, itemID), if sku is equal but itemID not, then update item id and local listing; 
                //if cannot find local listing by sku but can find in item, then add local listing and link with item;
                //if local listing not in active listing list, then disable the local listing
                //if ebay listing sku not in local listing or item, then ignore


                var createTime = DateTime.Now;
                var createBy = Constants.SystemUser;
                #region UPDATE:whether ebay listing match local listing(sku, itemID), update local listing with ebay item, if local item is disable, set online listing qty=0
                var updateInfoList = from al in activeListing
                                join ll in localListing on new { sku = al.SKU.ToUpper(), itemid = al.ItemID } equals new { sku = ll.ListingSKU.ToUpper(), itemid=ll.ListingID }
                                select new
                                {
                                    ll.ID,
                                    al.ItemID,
                                    al.SKU,
                                    Price=Convert.ToDecimal(al.SellingStatus.CurrentPrice.Value),
                                    Qty = al.Quantity - al.SellingStatus.QuantitySold,
                                    ShippingDetails = al.ShippingDetails
                                };

                foreach (var updateMatch in updateInfoList)
                {
                    var localListingItem = localListing.FirstOrDefault(ll=>ll.ID==updateMatch.ID);
                    if (localListingItem != null)
                    {
                        localListingItem.ListingInventoryQty = updateMatch.Qty;
                        localListingItem.ListingPrice = updateMatch.Price;

                        UpdateListingPostageRule(updateMatch.ShippingDetails, localListingItem);

                        localListingItem.LastUpdateTime = createTime;
                        _listingRepository.Update(localListingItem, l => l.ListingInventoryQty,l=>l.ListingPrice,l=>l.ListingPostageRuleID, l=>l.LastUpdateTime);
                    }
                }

                //var matchList = from al in activeListing
                //                from ll in localListing
                //                where al.SKU.Equals(ll.ListingSKU) && al.ItemID.Equals(ll.ListingID)
                //                select new { 
                //                    ll.ID,
                //                    al.ItemID,
                //                    al.SKU,
                //                    al.QuantityAvailable
                //                };

                #endregion

                #region DELETE: if local listing not in active listing list, then disable the local listing
                //var disableLocalListing = from ll in localListing
                //                          where ll.ListingStatusID == 3
                //                          && !activeListing.Select(l => l.SKU.ToUpper()).Contains(ll.ListingSKU.ToUpper())
                //                          select ll;

                var disableLocalListing = from ll in localListing
                                          where ll.ListingStatusID == 3
                                          && !activeListing.Any(al => al.SKU == ll.ListingSKU && al.ItemID == ll.ListingID)
                                          select ll;

                foreach (var dl in disableLocalListing)
                {
                    dl.ListingStatusID = 4;
                    dl.LastUpdateTime = createTime;
                    _listingRepository.Update(dl, l => l.ListingStatusID, l => l.LastUpdateTime);
                }
                #endregion

                #region ADD: if cannot find local listing by sku but can find in item, then add local listing and link with item;
                var addListingList = from al in activeListing
                                     join li in localItem on al.SKU.ToUpper() equals li.SKU.ToUpper() into g
                                     from alli in g.DefaultIfEmpty()
                                     //where !localListing.Select(ll => ll.ListingSKU.ToUpper()).Contains(al.SKU.ToUpper())
                                     where !localListing.Any(ll=>ll.ListingSKU==al.SKU && ll.ListingID==al.ItemID)
                                     select new { 
                                     ItemID=(alli.ID==null?0:alli.ID),
                                     ListingChannelID = listingChannelID,
                                     ListingID=al.ItemID,
                                     ListingSKU = al.SKU.ToUpper(),
                                     ListingTitle=al.Title,
                                     ListingDescription=al.Description,
                                     ListingPrice = Convert.ToDecimal(al.SellingStatus.CurrentPrice.Value),
                                     ListingInventory = al.Quantity - al.SellingStatus.QuantitySold,
                                     ShippingDetails = al.ShippingDetails,
                                     LastUpdateTime=DateTime.Now
                                     };

                foreach (var addListing in addListingList)
                {
                    var newLocalListing = new D_Listing();
                    
                    newLocalListing.ItemID = addListing.ItemID;
                    newLocalListing.ListingChannelID = addListing.ListingChannelID;
                    newLocalListing.ListingID = addListing.ListingID;
                    newLocalListing.ListingSKU = addListing.ListingSKU;
                    newLocalListing.ListingTitle = addListing.ListingTitle;
                    newLocalListing.ListingDescription = addListing.ListingDescription;
                    newLocalListing.ListingPrice = addListing.ListingPrice;
                    newLocalListing.ListingInventoryQty = addListing.ListingInventory;
                    newLocalListing.ListingStatusID = 3;//TODO: get active lising status ID in status list
                    newLocalListing.ListingPriceRuleID = 1;//TODO: Default price rule ID
                    newLocalListing.ListingInventoryQtyRuleID = 2;//TODO: Default inventory qty rule ID

                    UpdateListingPostageRule(addListing.ShippingDetails, newLocalListing);

                    newLocalListing.LastUpdateTime = createTime;
                    newLocalListing.CreateTime = createTime;
                    newLocalListing.CreateBy = createBy;
                    newLocalListing.EditTime = createTime;
                    newLocalListing.EditBy = createBy;
                    newLocalListing.FillOutNull();

                    _listingRepository.Insert(newLocalListing);
                }

                #endregion

                //#region whether ebay listing match local listing(sku, itemID), if sku is equal but itemID not, then update item id and local listing; 
                //var updateInfoItemIDList = from al in activeListing
                //                           join ll in localListing on al.SKU.ToUpper() equals ll.ListingSKU.ToUpper()
                //                           where al.ItemID != ll.ListingID
                //                           select new { 
                //                               ll.ID,
                //                               al.ItemID,
                //                               al.SKU,
                //                               Qty = al.Quantity - al.SellingStatus.QuantitySold
                //                           };

                //foreach (var updateMatch in updateInfoItemIDList)
                //{
                //    var localListingItem = localListing.FirstOrDefault(ll => ll.ID == updateMatch.ID);
                //    if (localListingItem != null)
                //    {
                //        localListingItem.ListingID = updateMatch.ItemID;
                //        localListingItem.ListingInventoryQty = updateMatch.Qty;
                //        _listingRepository.Update(localListingItem,l=>l.ListingID, l => l.ListingInventoryQty);
                //    }
                //}
                //#endregion
                


                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
                return false;
            }
        }

        public bool SyncOnlineListingByChannel(int listingChannelID)
        {
            return SyncOnlineListingByChannel(listingChannelID, null);
        }

        public bool SyncOnlineListingByChannel(int listingChannelID, IList<string> fieldNames)
        {
            var lstListing = GetListingByChannelID(listingChannelID);
            return SyncOnlineListing(lstListing, fieldNames);
        }

        public bool SyncOnlineListingByID(int id)
        {
            var listing = GetListingByID(id);
            return SyncOnlineListing(listing);
        }

        public bool SyncOnlineListing(IList<D_Listing> listings)
        {
            return SyncOnlineListing(listings, null);
        }

        public bool SyncOnlineListing(IList<D_Listing> listings, IList<string> fieldNames)
        {
            try
            {
                //listings = listings.Where(l => l.ListingDescription.Contains("We cannot ship to")).ToList();
                if (listings == null || listings.Count == 0)
                    throw new Exception("No Listing Passed to Function");
                var firstListing = listings.FirstOrDefault();

                var request = new UpdateeBayItemRequest() { SellerID = GetListingChannelByID(firstListing.ListingChannelID).Ref1 };
                var lstUpdateItem = new List<ItemType>();
                foreach (var listing in listings)
                {
                    var updateItem = new ItemType();
                    updateItem.ItemID = listing.ListingID;
                    updateItem.Title = listing.ListingTitle;
                    if (fieldNames == null)
                    {
                        updateItem.StartPrice = new AmountType() { currencyID = CurrencyCodeType.AUD };
                        updateItem.StartPrice.Value = Convert.ToDouble(listing.ListingPrice);
                        updateItem.Description = listing.ListingDescription;
                        updateItem.Quantity = listing.ListingInventoryQty;
                    }
                    else
                    {
                        if (fieldNames.Contains(listing.nameof(l => l.ListingInventoryQty)))
                        {
                            updateItem.Quantity = listing.ListingInventoryQty;
                        }

                        if (fieldNames.Contains(listing.nameof(l => l.ListingPrice)))
                        {
                            updateItem.StartPrice = new AmountType() { currencyID = CurrencyCodeType.AUD };
                            updateItem.StartPrice.Value = Convert.ToDouble(listing.ListingPrice);
                            updateItem.Description = listing.ListingDescription;
                        }

                        if (fieldNames.Contains(listing.nameof(l => l.ListingPostageRule)))
                        {
                            updateItem.Description = listing.ListingDescription;
                        }
                    }

                    lstUpdateItem.Add(updateItem);
                }
                request.Items = lstUpdateItem.ToArray();

                _eBayAPICallManager.UpdateeBayItem(request);

                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
                return false;
            }
        }

        public bool SyncOnlineListing(D_Listing listing)
        {
            var lstLising = listing.ToEnumerable().ToList();
            return SyncOnlineListing(lstLising);
        }

        public IList<D_Listing> GetListingByChannelID(int listingChannelID,bool isOnlyActive=true)
        {
            var list = _listingRepository.Table.Where(l=>l.ListingChannelID==listingChannelID);

            if (isOnlyActive)
                list = list.Where(l => l.ListingStatusID == 3);

            return list.ToList();
        }

        public D_Listing GetListingByID(int id)
        {
            return _listingRepository.GetById(id);
        }


        public T_ListingChannel GetListingChannelByID(int id)
        {
            return _listingChannelRepository.Table.FirstOrDefault(lc => lc.ID == id);
        }


        public bool SyncLocalListingByID(int id)
        {
            try
            {
                var listing = GetListingByID(id);

                //get ebay active listing
                var request = new GeteBaySellerListBySKUsRequest() { SellerID = GetListingChannelByID(listing.ListingChannelID).Ref1, SKUs = listing.ListingSKU.ToEnumerable().ToList() };
                var activeListing = _eBayAPICallManager.GeteBaySellerListBySKUs(request);
                var matchActiveListing = activeListing.FirstOrDefault(al=>al.ItemID.Equals(listing.ListingID)&&al.SKU.ToUpper().Equals(listing.ListingSKU.ToUpper()));
                var lastUpdateTime = DateTime.Now;
                if (matchActiveListing != null)
                {
                    listing.ListingTitle = matchActiveListing.Title ;
                    listing.ListingDescription = matchActiveListing.Description;
                    if (matchActiveListing.SellingStatus != null)
                    {
                        listing.ListingInventoryQty = matchActiveListing.Quantity - matchActiveListing.SellingStatus.QuantitySold;
                        listing.ListingPrice = Convert.ToDecimal(matchActiveListing.SellingStatus.CurrentPrice.Value);
                    }
                    UpdateListingPostageRule(matchActiveListing.ShippingDetails, listing);

                    listing.LastUpdateTime = lastUpdateTime;
                    _listingRepository.Update(listing);
                }
                else
                {
                    listing.ListingStatusID = 4;
                    listing.LastUpdateTime = lastUpdateTime;
                    _listingRepository.Update(listing, l => l.ListingStatusID, l => l.LastUpdateTime);
                }


                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
                return false;
            }
        }


        public void InsertListing(D_Listing listing)
        {
            if (listing != null)
                _listingRepository.Insert(listing);
        }

        public void UpdateListing(D_Listing listing)
        {
            if (listing != null)
                _listingRepository.Update(listing);
        }

        public void DeleteListing(D_Listing listing)
        {
            throw new NotImplementedException();
        }


        public bool UpdateFieldByRuleByChannel(int listingChannelID, IList<string> fieldNames)
        {
            try
            {
                if (fieldNames == null || fieldNames.Count == 0)
                    return true;
                var listings = GetListingByChannelID(listingChannelID);
                var editTime = DateTime.Now;
                foreach (var listing in listings)
                {
                    if (listing.Item != null && listing.Item.StatusID == 2)//TODO Get item disable status id
                    {
                        //listing.ListingStatusID = 4;//TODO Get listing disable status id
                        listing.ListingInventoryQty = 0;
                        listing.EditTime = editTime;
                        _listingRepository.Update(listing, l => l.ListingInventoryQty, l => l.EditTime);
                        continue;
                    }

                    if (fieldNames.Contains(listing.nameof(l => l.ListingInventoryQty)) && listing.ListingInventoryQtyRule!=null && listing.Item != null)
                    {
                        var matchInventoryQtyRuleLine = listing.ListingInventoryQtyRule.T_ValueRuleLine.FirstOrDefault(rl => listing.Item.InventoryQty >= rl.MinValue && listing.Item.InventoryQty < rl.MaxValue);
                        if (matchInventoryQtyRuleLine != null)
                        {
                            var paramList = new List<KeyValuePair<string, object>>();
                            var paramKeyValue = new KeyValuePair<string, object>(listing.Item.nameof(x => x.InventoryQty), listing.Item.InventoryQty);
                            paramList.Add(paramKeyValue);
                            var updateInventoryQty = Convert.ToInt32( CommonFunc.EvaluateFormula(matchInventoryQtyRuleLine.Formula, paramList));
                            listing.ListingInventoryQty = updateInventoryQty;
                            listing.EditTime = editTime;
                        }
                    }

                    if (fieldNames.Contains(listing.nameof(l => l.ListingPrice)) && listing.ListingPriceRule != null && listing.Item != null)
                    {
                        var matchInventoryQtyRuleLine = listing.ListingPriceRule.T_ValueRuleLine.FirstOrDefault(rl => listing.Item.Price >= rl.MinValue && listing.Item.Price < rl.MaxValue);
                        if (matchInventoryQtyRuleLine != null)
                        {
                            var paramList = new List<KeyValuePair<string, object>>();
                            var paramKeyValue = new KeyValuePair<string, object>(listing.nameof(x => x.Item.Price), listing.Item.Price);
                            paramList.Add(paramKeyValue);
                            var updatePrice = Convert.ToDecimal(CommonFunc.EvaluateFormula(matchInventoryQtyRuleLine.Formula.Replace(Constants.ListingPriceReplaceString, listing.Item.nameof(x => x.Price)), paramList));
                            SetListingPrice(listing, updatePrice);
                            listing.EditTime = editTime;

                            //_eventPublisher.Publish(new ListingPriceUpdated<D_Listing>(listing));
                        }
                    }

                    if (fieldNames.Contains(listing.nameof(l => l.ListingPostageRule)) && listing.Item != null)
                    {
                        //var paramList = new List<KeyValuePair<string, object>>();
                        //var paramKeyValue = new KeyValuePair<string, object>(listing.nameof(x => x.Item.Price), listing.Item.Price);
                        //paramList.Add(paramKeyValue);
                        //var updatePrice = Convert.ToDecimal(CommonFunc.EvaluateFormula(matchInventoryQtyRuleLine.Formula.Replace(Constants.ListingPriceReplaceString, listing.Item.nameof(x => x.Price)), paramList));
                        SetListingPostageRule(listing);

                        listing.EditTime = editTime;

                       
                        
                    }

                    _listingRepository.Update(listing, l => l.ListingInventoryQty,l=>l.ListingDescription, l => l.ListingPrice,l=>l.ListingPostageRuleID, l=>l.EditTime);

                    
                }


                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
                return false;
            }
        }

        private void SetListingPostageRule(D_Listing listing)
        {
            if (listing.Item.PostageRuleID == 1)
                listing.ListingPostageRuleID = 1;
            else
                listing.ListingPostageRuleID = 0;
            _eventPublisher.Publish(new ListingPostageRuleUpdated<D_Listing>(listing));
        }


        public void SetListingPrice(D_Listing listing, decimal newPrice)
        {
            listing.ListingPrice = newPrice;
            _eventPublisher.Publish(new ListingPriceUpdated<D_Listing>(listing));
        }


        #region private method

        private void UpdateListingPostageRule(ShippingDetailsType shippingDetails, D_Listing listing)
        {
            if (shippingDetails != null && shippingDetails.ShippingServiceOptions != null)
            {
                var shippingServiceOption = shippingDetails.ShippingServiceOptions.ToArray().FirstOrDefault();
                if (shippingServiceOption != null)
                {
                    if (shippingServiceOption.ShippingService == "AU_StandardDelivery")
                    {
                        listing.ListingPostageRuleID = 1;//FreeShipping
                    }
                }
            }
        }

        #endregion
    }
}
