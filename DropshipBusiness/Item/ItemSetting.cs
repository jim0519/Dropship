using DropshipBusiness.Setting;
using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DropshipBusiness.Item
{
    public class ItemSettings:ISettings
    {
        public bool ReDownloadImage { get; set; }

        public string IgnoreSKUStartList { get; set; }
    }
}