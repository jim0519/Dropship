using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipBusiness.Events;
using DropshipBusiness.Listing;
using DropshipCommon.Models;
using HtmlAgilityPack;
namespace Nop.Plugin.Widgets.NivoSlider.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ListingPriceUpdatedEventConsumer:
        IConsumer<ListingPriceUpdated<D_Listing>>,
        IConsumer<ListingPostageRuleUpdated<D_Listing>>
    {
        //freeshipping not bulky
        public const string strFreePostage = "Postage is calculated as per item. We offer free shipping service to Australian wide for this item excluding the following postcode: 2831, 5701, 6740, 6743, 6799 and 7151.";
        //freeshipping and bulky
        public const string strBulkyFreePostage = "Postage is calculated as per item. We offer free shipping service to Australian wide for this item excluding the following areas: NT 0800-0999, NSW 2641, 2717, QLD 4421, 4450-4499, 4680, 4700-4805, 9920-9959, 4806-4899, 4900-4999, 9960-9999, SA 5701, TAS 7151, WA 6055, 6215-6699, 6700-6799 due to no shipping contract, sorry about all inconvenience occured.";
        //freight and bulky
        public const string strBulkyItemFreight = "Postage is calculated as per item. We cannot ship to NT 0800-0999, NSW 2641, 2717, QLD 4421, 4450-4499, 4680, 4700-4805, 9920-9959, 4806-4899, 4900-4999, 9960-9999, SA 5701, TAS 7151, WA 6055, 6215-6699, 6700-6799 for this bulky item because we have no shipping contract for these areas, sorry about all inconvenience occured.";
        //freight and not bulky
        public const string strNotBulkyFreight = "Postage is calculated as per item. We offer freight shipping service to Australian wide for this item excluding the following postcode: 2831, 5701, 6740, 6743, 6799 and 7151.";

        private readonly IListingService _listingService;
        public ListingPriceUpdatedEventConsumer(IListingService listingService)
        {
            _listingService = listingService;
        }



        public void HandleEvent(ListingPriceUpdated<D_Listing> eventMessage)
        {
            var listing = eventMessage.Entity;
            var descHtmlDoc = new HtmlDocument();

            descHtmlDoc.LoadHtml(listing.ListingDescription);
            var elementPrice = descHtmlDoc.GetElementbyId("price");
            if (elementPrice != null)
            {
                WriteText(elementPrice, "$" + listing.ListingPrice.ToString());
                listing.ListingDescription = descHtmlDoc.DocumentNode.OuterHtml;
            }

            //_listingService.UpdateListing(listing);
        }

        public void HandleEvent(ListingPostageRuleUpdated<D_Listing> eventMessage)
        {
            var listing = eventMessage.Entity;
            var descHtmlDoc = new HtmlDocument();
            if (listing.ListingDescription.IndexOf("dealsplash") != -1)
            {
                listing.ListingDescription = listing.ListingDescription.Replace("dealsplash", "ozcrazymall");
            }
            descHtmlDoc.LoadHtml(listing.ListingDescription);

            //update shipping & delivery
            var elementPostageDesc = descHtmlDoc.DocumentNode.SelectSingleNode("//*[@id='delivery']/p[3]");
            var elementFreeFastIcon = descHtmlDoc.DocumentNode.SelectSingleNode("//*[@id='delivery']/img");

            if (elementPostageDesc != null)
            {
                if (listing.Item.PostageRuleID!=1&& listing.Item.Ref1.ToLower() =="yes")//Bulky Item
                {
                    WriteText(elementPostageDesc, strBulkyItemFreight);
                }
                else if (listing.Item.PostageRuleID != 1 && listing.Item.Ref1.ToLower() == "no")
                {
                    WriteText(elementPostageDesc, strNotBulkyFreight);
                }
                else if (listing.Item.PostageRuleID == 1 && listing.Item.Ref1.ToLower() == "no")
                {
                    WriteText(elementPostageDesc, strFreePostage);
                }
                else
                {
                    WriteText(elementPostageDesc, strBulkyFreePostage);
                }
            }

            if (elementFreeFastIcon != null)
            {
                if (listing.Item.PostageRuleID != 1)
                {
                    elementFreeFastIcon.Remove();
                }
            }

            listing.ListingDescription = descHtmlDoc.DocumentNode.OuterHtml;


        }


        private void WriteText(HtmlNode node, string text)
        {
            if (node.ChildNodes.Count > 0)
            {
                node.ReplaceChild(node.OwnerDocument.CreateTextNode(text), node.ChildNodes.First());
            }
            else
            {
                node.AppendChild(node.OwnerDocument.CreateTextNode(text));
            }
        }
    }
}
