using DropshipFramework.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dropship.Models.Listing
{
    public class ListingListViewModel : BaseViewModel
    {
        public int ListingChannelID { get; set; }

        public IList<SelectListItem> AvailableListingChannels { get; set; }
        
        public IList<string> AvailableUpdateFields { get; set; }

        public IList<string> SelectedUpdateFields { get; set; }
    }
}