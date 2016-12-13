using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DropshipCommon.Models;
using System.Web.Mvc;
using Dropship.Models.Item;
using Dropship.Models.Listing;

namespace Dropship.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }


        #region Item
        //Item
        public static ItemGridViewModel ToModel(this D_Item entity)
        {
            return entity.MapTo<D_Item, ItemGridViewModel>();
        }

        #endregion

        #region Listing
        //Item
        public static ListingGridViewModel ToModel(this D_Listing entity)
        {
            return entity.MapTo<D_Listing, ListingGridViewModel>();
        }

        #endregion

    }

    public static class URLExtensions
    {
        public static MvcHtmlString MakeNavBarActive(this UrlHelper helper, string controllerName)
        {
            string result = "active";

            var currentControllerName = helper.RequestContext.RouteData.Values["controller"].ToString();

            if (!currentControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }

            return MvcHtmlString.Create(result);
        }
    }
}