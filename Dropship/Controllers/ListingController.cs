using Dropship.Models.Listing;
using DropshipBusiness.Listing;
using DropshipFramework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dropship.Extensions;
using DropshipCommon.Infrastructure;
using DropshipBusiness.Common;
using DropshipCommon;
using DropshipFramework.MVC;
using DropshipFramework.Controllers;
using DropshipCommon.Models;
using DropshipFramework;

namespace Dropship.Controllers
{
    public class ListingController : BaseController
    {
        private readonly IListingService _listingService;
        private readonly ICommonService _commonService;
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        public ListingController(IListingService listingService,
            ICommonService commonService,
            ICacheManager cacheManager,
            IWorkContext workContext)
        {
            _listingService = listingService;
            _commonService = commonService;
            _cacheManager = cacheManager;
            _workContext = workContext;
        }
        // GET: Listing
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {

            var model = new ListingListViewModel();
            var listingModel = new D_Listing();
            model.AvailableUpdateFields = new List<string>();
            model.AvailableUpdateFields.Add(listingModel.nameof(l=>l.ListingInventoryQty));
            model.AvailableUpdateFields.Add(listingModel.nameof(l => l.ListingPrice));
            //model.AvailableUpdateFields.Add(listingModel.nameof(l => l.ListingDescription));
            model.AvailableUpdateFields.Add(listingModel.nameof(l => l.ListingPostageRule));

            model.AvailableListingChannels = new SelectList( _workContext.CurrentUser.ListingChannels.Select(c=>new {ID=c.ID,Name=c.Name}),"ID","Name").ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ListingListViewModel model)
        {

            var listings = _listingService.GetListingByChannelID(model.ListingChannelID);
            var renderListings = listings.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).ToList();
            var listingStatusList = _cacheManager.Get(DropshipCacheKey.ListingStatusList, () => _commonService.GetStatusByEntityType(DropshipEntityType.LISTING));
            var itemStatusList = _cacheManager.Get(DropshipCacheKey.ItemStatusList, () => _commonService.GetStatusByEntityType(DropshipEntityType.ITEM));
            var itemViewModels = renderListings.Select(i =>
            {
                var viewModel = i.ToModel();
                var listingStatus=listingStatusList.FirstOrDefault(ls=>ls.ID==i.ListingStatusID);
                if (listingStatus != null)
                    viewModel.ListingStatus = listingStatus.Name;
                if (i.Item != null)
                {
                    viewModel.ItemPrice = i.Item.Price;
                    viewModel.ItemInventoryQty = i.Item.InventoryQty;
                    viewModel.ItemTitle = i.Item.Title;
                    var itemStatus = itemStatusList.FirstOrDefault(s => s.ID == i.Item.StatusID);
                    if (itemStatus != null)
                        viewModel.ItemStatus = itemStatus.Name;
                }
                return viewModel;
            });


            var gridModel = new DataSourceResult() { Data = itemViewModels, Total = listings.Count };

            //return View();
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult SyncLocalListing(int listingChannelID)
        {
            var isSuccess = _listingService.SyncLocalListingByChannel(listingChannelID);
            return Json(new { Result = isSuccess });
        }

        [HttpPost]
        public ActionResult SyncOnlineListing(ListingListViewModel model)
        {
            var isSuccess = _listingService.SyncOnlineListingByChannel(model.ListingChannelID, model.SelectedUpdateFields);
            return Json(new { Result = isSuccess });
        }

        [HttpPost]
        public ActionResult UpdateFieldByRule(ListingListViewModel model)
        {
            var isSuccess = _listingService.UpdateFieldByRuleByChannel(model.ListingChannelID, model.SelectedUpdateFields);
            return Json(new { Result = true });
        }

        

        [HttpPost]
        public ActionResult ListingUpdate(ListingGridViewModel model)
        //public ActionResult ListingUpdate(int id,
        //    string listingTitle, 
        //    decimal ListingPrice, 
        //    int ListingInventoryQty, 
        //    string listingID, 
        //    string ListingSKU, 
        //    DateTime? LastUpdateTime, 
        //    DateTime? CreateTime, 
        //    DateTime? EditTime,
        //    string CreateBy)
        {

            var updateListing = _listingService.GetListingByID(model.ID);
            if (updateListing!=null)
            { 
                _listingService.SetListingPrice(updateListing, model.ListingPrice);
                updateListing.ListingTitle = model.ListingTitle;
                updateListing.ListingInventoryQty = model.ListingInventoryQty;

                _listingService.UpdateListing(updateListing);
            }

            return new NullJsonResult();
        }

        //[HttpPost]
        //public ActionResult ListingDelete(int id)
        //{

        //    return new NullJsonResult();
        //}

        [HttpPost]
        public ActionResult SyncLocal(ListingGridViewModel model)
        {
            var isSuccess = _listingService.SyncLocalListingByID(model.ID);
            return Json(new { Result = isSuccess });
        }

        [HttpPost]
        public ActionResult SyncOnline(ListingGridViewModel model)
        {
            var isSuccess = _listingService.SyncOnlineListingByID(model.ID);
            return Json(new { Result = isSuccess });
        }
    }
}