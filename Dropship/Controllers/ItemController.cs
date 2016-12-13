using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DropshipFramework.Controllers;
using DropshipBusiness.Item;
using DropshipFramework.Kendoui;
using Dropship.Models.Item;
using Dropship.Extensions;

namespace Dropship.Controllers
{
    public class ItemController : BaseController
    {
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {


            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ItemListViewModel model)
        {

            var items = _itemService.GetAllItems();
            var renderItems = items.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).ToList();
            var itemViewModels = renderItems.Select(i => i.ToModel());

            var gridModel = new DataSourceResult() { Data = itemViewModels, Total = items.Count };

            //return View();
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult UpdateLocalItem()
        {
            var isSuccess = _itemService.UpdateLocalItem();
            return Json(new { Result = isSuccess });
        }

        [HttpPost]
        public ActionResult FixInfo()
        {
            var isSuccess = _itemService.FixInfo();
            return Json(new { Result = isSuccess });
        }
    }
}