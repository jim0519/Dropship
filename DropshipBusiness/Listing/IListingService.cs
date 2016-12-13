using DropshipCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropshipBusiness.Listing
{
    public interface IListingService
    {
        bool SyncLocalListingByChannel(int listingChannelID);

        bool SyncLocalListingByID(int id);

        bool SyncOnlineListingByChannel(int listingChannelID);

        bool SyncOnlineListingByChannel(int listingChannelID, IList<string> fieldNames);

        bool SyncOnlineListingByID(int id);

        bool SyncOnlineListing(IList<D_Listing> listings);

        bool SyncOnlineListing(IList<D_Listing> listings, IList<string> fieldNames);

        bool SyncOnlineListing(D_Listing listing);



        bool UpdateFieldByRuleByChannel(int listingChannelID,IList<string> fieldNames);

        IList<D_Listing> GetListingByChannelID(int listingChannelID, bool isOnlyActive = true);

        D_Listing GetListingByID(int id);

        T_ListingChannel GetListingChannelByID(int id);

        void InsertListing(D_Listing listing);

        void UpdateListing(D_Listing listing);

        void DeleteListing(D_Listing listing);

        void SetListingPrice(D_Listing listing, decimal newPrice);
    }
}
