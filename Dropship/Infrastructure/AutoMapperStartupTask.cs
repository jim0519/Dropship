using AutoMapper;
using Dropship.Models.Item;
using Dropship.Models.Listing;
using DropshipCommon;
using DropshipCommon.Infrastructure;
using DropshipCommon.Models;
using System;
using System.Linq.Expressions;

namespace Dropship.Infrastructure
{
   

    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            Mapper.CreateMap<D_Item, ItemGridViewModel>();
            Mapper.CreateMap<D_Listing, ListingGridViewModel>();
        }

        public int Order
        {
            get
            {
                return 0;
            }
        }
    }
}
