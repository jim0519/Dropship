using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;


namespace DropshipBusiness.Item
{
    public interface IItemService
    {
        IList<D_Item> GetAllItems();

        IList<D_Item> GetItemsBySupplier(int supplierID);

        D_Item GetItem(string sku,int supplierID);

        D_Item GetItemByID(int id);

        bool UpdateLocalItem();

        bool FixInfo();

        void InsertItem(D_Item item);

        void UpdateItem(D_Item item);

        void DeleteItem(D_Item item);
    }
}
