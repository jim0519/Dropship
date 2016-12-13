using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DropshipBusiness.Item
{
    public class DropshipzoneSKUModel 
    {
        public string SKU { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string ListingTitle { get; set; }
        //public string ListingTitle { get; set; }
        [CsvColumn(Name = "price")]
        public decimal Price { get; set; }
        [CsvColumn(Name = "Stock Qty")]
        public int InventoryQty { get; set; }
        public string Status { get; set; }
        public string VIC { get; set; }
        public string NSW { get; set; }
        public string SA { get; set; }
        public string QLD { get; set; }
        public string TAS { get; set; }
        public string WA { get; set; }
        public string NT { get; set; }
        [CsvColumn(Name = "bulky item")]
        public string IsBulkyItem { get; set; }
        [CsvColumn(Name = "Weight (kg)")]
        public string Weight { get; set; }
        [CsvColumn(Name = "Carton Length (cm)")]
        public string Length { get; set; }
        [CsvColumn(Name = "Carton Width (cm)")]
        public string Width { get; set; }
        [CsvColumn(Name = "Carton Height (cm)")]
        public string Height { get; set; }
        //[CsvColumn(Name = "images")]
        //public string Images { get; set; }

        [CsvColumn(Name = "Image 1")]
        public string Image1 { get; set; }
        [CsvColumn(Name = "Image 2")]
        public string Image2 { get; set; }
        [CsvColumn(Name = "Image 3")]
        public string Image3 { get; set; }
        [CsvColumn(Name = "Image 4")]
        public string Image4 { get; set; }
        [CsvColumn(Name = "Image 5")]
        public string Image5 { get; set; }
        [CsvColumn(Name = "Image 6")]
        public string Image6 { get; set; }
        [CsvColumn(Name = "Image 7")]
        public string Image7 { get; set; }
        [CsvColumn(Name = "Image 8")]
        public string Image8 { get; set; }
        [CsvColumn(Name = "Image 9")]
        public string Image9 { get; set; }
        [CsvColumn(Name = "Image 10")]
        public string Image10 { get; set; }
        [CsvColumn(Name = "Image 11")]
        public string Image11 { get; set; }
        [CsvColumn(Name = "Image 12")]
        public string Image12 { get; set; }
        [CsvColumn(Name = "Image 13")]
        public string Image13 { get; set; }
        [CsvColumn(Name = "Image 14")]
        public string Image14 { get; set; }
        [CsvColumn(Name = "Image 15")]
        public string Image15 { get; set; }
    }
}