using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;
using DropshipData;
using DropshipCommon;
using System.IO;
using LINQtoCSV;
using DropshipBusiness.Common;

namespace DropshipBusiness.Item
{
    public class ItemService:IItemService
    {
        private readonly IRepository<D_Item> _itemRepository;
        private readonly IImageService _imageService;
        private readonly IPostageRuleService _postageRuleService;

        public ItemService(IRepository<D_Item> itemRepository,
            IImageService imageService,
            IPostageRuleService postageRuleService)
        {
            _itemRepository = itemRepository;
            _imageService = imageService;
            _postageRuleService = postageRuleService;
        }

        public IList<D_Item> GetAllItems()
        {
            var items = _itemRepository.Table.ToList();
            return items;
        }

        public D_Item GetItem(string sku, int supplierID)
        {
            var item = _itemRepository.Table.FirstOrDefault(i => i.SKU.ToLower().Equals(sku.ToLower())
                &&i.SupplierID==supplierID);
            return item;

        }


        public bool UpdateLocalItem()
        {
            try
            {
                var createTime = DateTime.Now;
                var createBy = Constants.SystemUser;
                var skuList = GetSupplierSKUList();
                
                var localItemList = GetItemsBySupplier(1);//TODO Get Dropshipzone Supplier ID
                //var skuNullCount = skuList.Where(sl => sl==null).ToList();
                #region Update Item
                var updateItemList = from sl in skuList
                                     join li in localItemList on sl.SKU.ToUpper() equals li.SKU.ToUpper()
                                     select new
                                     {
                                         li.ID,
                                         SKU = sl.SKU.ToUpper(),
                                         sl.Title,
                                         sl.InventoryQty,
                                         sl.Price,
                                         sl.Description,
                                         sl.IsBulkyItem,
                                         sl.Weight,
                                         sl.Length,
                                         sl.Width,
                                         sl.Height,
                                         sl.VIC,
                                         sl.NSW,
                                         sl.SA,
                                         sl.QLD,
                                         sl.TAS,
                                         sl.WA,
                                         sl.NT,
                                         sl.Image1,
                                         sl.Image2,
                                         sl.Image3,
                                         sl.Image4,
                                         sl.Image5,
                                         sl.Image6,
                                         sl.Image7,
                                         sl.Image8,
                                         sl.Image9,
                                         sl.Image10,
                                         sl.Image11,
                                         sl.Image12,
                                         sl.Image13,
                                         sl.Image14,
                                         sl.Image15
                                        
                                     };

                foreach (var updateItem in updateItemList)
                {
                    var item = localItemList.FirstOrDefault(li => li.ID == updateItem.ID);
                    item.Title = updateItem.Title;
                    item.Description = updateItem.Description;
                    item.InventoryQty = updateItem.InventoryQty;
                    item.Price = updateItem.Price;
                    item.Ref1 = updateItem.IsBulkyItem;
                    item.Ref2 = updateItem.Weight;
                    item.Ref3 = updateItem.Length;
                    item.Ref4 = updateItem.Width;
                    item.Ref5 = updateItem.Height;
                    item.StatusID = 1;//TODO Get item active status id

                    


                    //postage rule
                    var postageRuleName = updateItem.nameof(n => n.VIC) + ":" + updateItem.VIC + ";" +
                        updateItem.nameof(n => n.NSW) + ":" + updateItem.NSW + ";" +
                        updateItem.nameof(n => n.SA) + ":" + updateItem.SA + ";" +
                        updateItem.nameof(n => n.QLD) + ":" + updateItem.QLD + ";" +
                        updateItem.nameof(n => n.TAS) + ":" + updateItem.TAS + ";" +
                        updateItem.nameof(n => n.WA) + ":" + updateItem.WA + ";" +
                        updateItem.nameof(n => n.NT) + ":" + updateItem.NT;

                    var existingRule = _postageRuleService.GetPostageRuleByName(postageRuleName);
                    if (existingRule == null)
                    {
                        var newRule = new T_PostageRule();
                        newRule.Name = postageRuleName;
                        newRule.Description = postageRuleName;
                        newRule.CreateTime = createTime;
                        newRule.CreateBy = createBy;
                        newRule.EditTime = createTime;
                        newRule.EditBy = createBy;

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "3000", PostcodeTo = "3999", Formula = updateItem.VIC, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "8000", PostcodeTo = "8999", Formula = updateItem.VIC, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0200", PostcodeTo = "0299", Formula = updateItem.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "1000", PostcodeTo = "2999", Formula = updateItem.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "5000", PostcodeTo = "5999", Formula = updateItem.SA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "4000", PostcodeTo = "4999", Formula = updateItem.QLD, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "9000", PostcodeTo = "9999", Formula = updateItem.QLD, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "7000", PostcodeTo = "7999", Formula = updateItem.TAS, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "6000", PostcodeTo = "6797", Formula = updateItem.WA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "6800", PostcodeTo = "6999", Formula = updateItem.WA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0800", PostcodeTo = "0999", Formula = updateItem.NT, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        existingRule = _postageRuleService.InsertPostageRule(newRule);
                    }

                    item.PostageRuleID = existingRule.ID;

                    _itemRepository.Update(item);
                }

                #endregion

                #region Delete Disabled Item
                var disableItemList = from li in localItemList
                                      where !skuList.Any(sl => sl.SKU.ToUpper().Equals(li.SKU.ToUpper()))
                                      && li.StatusID == 1//TODO Get item active status id
                                      select li;

                foreach (var disableItem in disableItemList)
                {
                    disableItem.StatusID = 2;//TODO Get item disable status id
                    _itemRepository.Update(disableItem, i => i.StatusID);
                }

                #endregion

                #region Add New Item
                var addItemList = from sl in skuList
                                  where !localItemList.Any(li => li.SKU.ToUpper().Equals(sl.SKU.ToUpper()))
                                  select sl;

                
                foreach (var additem in addItemList)
                {
                    var newItem = new D_Item();
                    newItem.SKU = additem.SKU;
                    newItem.Title = additem.Title;
                    newItem.Price = additem.Price;
                    newItem.InventoryQty = additem.InventoryQty;
                    newItem.Description = additem.Description;
                    newItem.StatusID = 1;//TODO Get item active status id
                    newItem.SupplierID = 1;//TODO Get Dropshipzone supplier id
                    newItem.Ref1 = additem.IsBulkyItem;
                    newItem.Ref2 = additem.Weight;
                    newItem.Ref3 = additem.Length;
                    newItem.Ref4 = additem.Width;
                    newItem.Ref5 = additem.Height;
                    newItem.CreateTime = createTime;
                    newItem.CreateBy = createBy;
                    newItem.EditTime = createTime;
                    newItem.EditBy = createBy;
                    newItem.FillOutNull();

                    //images
                    //var imagesURL = additem.Images.Split(';');
                    var imagesURL = new List<string>();
                    if (!string.IsNullOrEmpty(additem.Image1))
                        imagesURL.Add(additem.Image1);
                    if (!string.IsNullOrEmpty(additem.Image2))
                        imagesURL.Add(additem.Image2);
                    if (!string.IsNullOrEmpty(additem.Image3))
                        imagesURL.Add(additem.Image3);
                    if (!string.IsNullOrEmpty(additem.Image4))
                        imagesURL.Add(additem.Image4);
                    if (!string.IsNullOrEmpty(additem.Image5))
                        imagesURL.Add(additem.Image5);
                    if (!string.IsNullOrEmpty(additem.Image6))
                        imagesURL.Add(additem.Image6);
                    if (!string.IsNullOrEmpty(additem.Image7))
                        imagesURL.Add(additem.Image7);
                    if (!string.IsNullOrEmpty(additem.Image8))
                        imagesURL.Add(additem.Image8);
                    if (!string.IsNullOrEmpty(additem.Image9))
                        imagesURL.Add(additem.Image9);
                    if (!string.IsNullOrEmpty(additem.Image10))
                        imagesURL.Add(additem.Image10);
                    if (!string.IsNullOrEmpty(additem.Image11))
                        imagesURL.Add(additem.Image11);
                    if (!string.IsNullOrEmpty(additem.Image12))
                        imagesURL.Add(additem.Image12);
                    if (!string.IsNullOrEmpty(additem.Image13))
                        imagesURL.Add(additem.Image13);
                    if (!string.IsNullOrEmpty(additem.Image14))
                        imagesURL.Add(additem.Image14);
                    if (!string.IsNullOrEmpty(additem.Image15))
                        imagesURL.Add(additem.Image15);
                    int i = 0;
                    DirectoryInfo di = new DirectoryInfo(DropshipConfig.Instance.ImageFilesPath + additem.SKU + "\\");
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                    using (var wc = new DropshipWebClient())
                    {
                        foreach (var imageURL in imagesURL)
                        {
                            try
                            {
                                var imageFileName = CommonFunc.GetImageFileName(additem.SKU, i);
                                var saveImageFileFullName = Path.Combine(di.FullName, imageFileName);

                                wc.DownloadFile(imageURL, saveImageFileFullName);

                                newItem.ItemImages.Add(new M_ItemImage()
                                {
                                    Image = _imageService.InsertImage(new D_Image()
                                    {
                                        ImagePath = additem.SKU + "\\" + imageFileName,
                                        CreateTime = createTime,
                                        CreateBy = createBy,
                                        EditTime = createTime,
                                        EditBy = createBy
                                    }),
                                    DisplayOrder = i + 1,
                                    StatusID = 5,//TODO Get item active status id
                                    CreateTime = createTime,
                                    CreateBy = createBy,
                                    EditTime = createTime,
                                    EditBy = createBy
                                });
                            }
                            catch (Exception ex)
                            {
                                LogManager.Instance.Error(imageURL+" download failed. "+ex.Message);
                            }

                            i++;
                        }
                    }

                    //postage rule
                    var postageRuleName = additem.nameof(n => n.VIC) + ":" + additem.VIC + ";" +
                        additem.nameof(n => n.NSW) + ":" + additem.NSW + ";" +
                        additem.nameof(n => n.SA) + ":" + additem.SA + ";" +
                        additem.nameof(n => n.QLD) + ":" + additem.QLD + ";" +
                        additem.nameof(n => n.TAS) + ":" + additem.TAS + ";" +
                        additem.nameof(n => n.WA) + ":" + additem.WA + ";" +
                        additem.nameof(n => n.NT) + ":" + additem.NT;

                    var existingRule = _postageRuleService.GetPostageRuleByName(postageRuleName);
                    if (existingRule == null)
                    {
                        var newRule = new T_PostageRule();
                        newRule.Name = postageRuleName;
                        newRule.Description = postageRuleName;
                        newRule.CreateTime = createTime;
                        newRule.CreateBy = createBy;
                        newRule.EditTime = createTime;
                        newRule.EditBy = createBy;

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "3000", PostcodeTo = "3999", Formula = additem.VIC, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "8000", PostcodeTo = "8999", Formula = additem.VIC, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0200", PostcodeTo = "0299", Formula = additem.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "1000", PostcodeTo = "2999", Formula = additem.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "5000", PostcodeTo = "5999", Formula = additem.SA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "4000", PostcodeTo = "4999", Formula = additem.QLD, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "9000", PostcodeTo = "9999", Formula = additem.QLD, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "7000", PostcodeTo = "7999", Formula = additem.TAS, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "6000", PostcodeTo = "6797", Formula = additem.WA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "6800", PostcodeTo = "6999", Formula = additem.WA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0800", PostcodeTo = "0999", Formula = additem.NT, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        existingRule = _postageRuleService.InsertPostageRule(newRule);
                    }

                    newItem.PostageRuleID = existingRule.ID;

                    _itemRepository.Insert(newItem);

                }


                

                #endregion


                return true;

            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
                return false;
            }
        }


        public bool FixInfo()
        {
            try 
            {
                var skuList = GetSupplierSKUList();

                var localItemList = GetItemsBySupplier(1);//TODO Get dropshipzone supplier id

                var grpSKUList = skuList.GroupBy(s => new {s.VIC, s.NSW, s.SA, s.QLD, s.TAS, s.WA, s.NT });
                var createTime = DateTime.Now;
                var createBy = Constants.SystemUser;
                foreach (var postageGrp in grpSKUList)
                { 
                    var postageRuleName=postageGrp.Key.nameof(n=>n.VIC)+":"+postageGrp.Key.VIC+";"+
                        postageGrp.Key.nameof(n=>n.NSW)+":"+postageGrp.Key.NSW+";"+
                        postageGrp.Key.nameof(n=>n.SA)+":"+postageGrp.Key.SA+";"+
                        postageGrp.Key.nameof(n=>n.QLD)+":"+postageGrp.Key.QLD+";"+
                        postageGrp.Key.nameof(n=>n.TAS)+":"+postageGrp.Key.TAS+";"+
                        postageGrp.Key.nameof(n=>n.WA)+":"+postageGrp.Key.WA+";"+
                        postageGrp.Key.nameof(n=>n.NT)+":"+postageGrp.Key.NT;

                    var existingRule = _postageRuleService.GetPostageRuleByName(postageRuleName);
                    if (existingRule == null)
                    {
                        var newRule = new T_PostageRule();
                        newRule.Name = postageRuleName;
                        newRule.Description = postageRuleName;
                        newRule.CreateTime = createTime;
                        newRule.CreateBy = createBy;
                        newRule.EditTime = createTime;
                        newRule.EditBy = createBy;

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "3000", PostcodeTo = "3999", Formula = postageGrp.Key.VIC, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "8000", PostcodeTo = "8999", Formula = postageGrp.Key.VIC, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0200", PostcodeTo = "0299", Formula = postageGrp.Key.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "1000", PostcodeTo = "1999", Formula = postageGrp.Key.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "2000", PostcodeTo = "2999", Formula = postageGrp.Key.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        //newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "2619", PostcodeTo = "2899", Formula = postageGrp.Key.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        //newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "2921", PostcodeTo = "2999", Formula = postageGrp.Key.NSW, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "5000", PostcodeTo = "5799", Formula = postageGrp.Key.SA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "5800", PostcodeTo = "5999", Formula = postageGrp.Key.SA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "4000", PostcodeTo = "4999", Formula = postageGrp.Key.QLD, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "9000", PostcodeTo = "9999", Formula = postageGrp.Key.QLD, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });


                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "7000", PostcodeTo = "7799", Formula = postageGrp.Key.TAS, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "7800", PostcodeTo = "7999", Formula = postageGrp.Key.TAS, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "6000", PostcodeTo = "6797", Formula = postageGrp.Key.WA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "6800", PostcodeTo = "6999", Formula = postageGrp.Key.WA, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });

                        newRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0800", PostcodeTo = "0999", Formula = postageGrp.Key.NT, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });


                        existingRule = _postageRuleService.InsertPostageRule(newRule);
                    }
                    else
                    {
                        existingRule.T_PostageRuleLine.Add(new T_PostageRuleLine() { PostcodeFrom = "0800", PostcodeTo = "0999", Formula = postageGrp.Key.NT, CreateTime = createTime, CreateBy = createBy, EditTime = createTime, EditBy = createBy });
                        existingRule = _postageRuleService.UpdatePostageRule(existingRule);
                    }

                    //foreach (var skuLine in postageGrp)
                    //{
                    //    var localItemLine = GetItem(skuLine.SKU, 1);//TODO Get dropshipzone supplier id
                    //    if (localItemLine != null)
                    //    {
                    //        localItemLine.PostageRuleID = existingRule.ID;
                    //        _itemRepository.Update(localItemLine, i => i.PostageRuleID);
                    //    }
                    //}
                }

                //images

                foreach (var skuLine in skuList)
                {
                    var localItemLine = GetItem(skuLine.SKU, 1);//TODO Get dropshipzone supplier id

                    if (localItemLine != null && (localItemLine.ItemImages == null || localItemLine.ItemImages.Count==0))
                    {
                        //var imagesURL = skuLine.Images.Split(';');
                        var imagesURL = new List<string>();
                        if (!string.IsNullOrEmpty(skuLine.Image1))
                            imagesURL.Add(skuLine.Image1);
                        if (!string.IsNullOrEmpty(skuLine.Image2))
                            imagesURL.Add(skuLine.Image2);
                        if (!string.IsNullOrEmpty(skuLine.Image3))
                            imagesURL.Add(skuLine.Image3);
                        if (!string.IsNullOrEmpty(skuLine.Image4))
                            imagesURL.Add(skuLine.Image4);
                        if (!string.IsNullOrEmpty(skuLine.Image5))
                            imagesURL.Add(skuLine.Image5);
                        if (!string.IsNullOrEmpty(skuLine.Image6))
                            imagesURL.Add(skuLine.Image6);
                        if (!string.IsNullOrEmpty(skuLine.Image7))
                            imagesURL.Add(skuLine.Image7);
                        if (!string.IsNullOrEmpty(skuLine.Image8))
                            imagesURL.Add(skuLine.Image8);
                        if (!string.IsNullOrEmpty(skuLine.Image9))
                            imagesURL.Add(skuLine.Image9);
                        if (!string.IsNullOrEmpty(skuLine.Image10))
                            imagesURL.Add(skuLine.Image10);
                        if (!string.IsNullOrEmpty(skuLine.Image11))
                            imagesURL.Add(skuLine.Image11);
                        if (!string.IsNullOrEmpty(skuLine.Image12))
                            imagesURL.Add(skuLine.Image12);
                        if (!string.IsNullOrEmpty(skuLine.Image13))
                            imagesURL.Add(skuLine.Image13);
                        if (!string.IsNullOrEmpty(skuLine.Image14))
                            imagesURL.Add(skuLine.Image14);
                        if (!string.IsNullOrEmpty(skuLine.Image15))
                            imagesURL.Add(skuLine.Image15);
                        int i = 0;
                        DirectoryInfo di = new DirectoryInfo(DropshipConfig.Instance.ImageFilesPath + skuLine.SKU + "\\");
                        if (!di.Exists)
                        {
                            di.Create();

                            using (var wc = new DropshipWebClient())
                            {
                                foreach (var imageURL in imagesURL)
                                {
                                    try
                                    {
                                        var imageFileName = CommonFunc.GetImageFileName(skuLine.SKU, i);
                                        var saveImageFileFullName = Path.Combine(di.FullName, imageFileName);

                                        wc.DownloadFile(imageURL, saveImageFileFullName);

                                        localItemLine.ItemImages.Add(new M_ItemImage()
                                        {
                                            Image = _imageService.InsertImage(new D_Image()
                                            {
                                                ImagePath = skuLine.SKU + "\\" + imageFileName,
                                                CreateTime = createTime,
                                                CreateBy = createBy,
                                                EditTime = createTime,
                                                EditBy = createBy
                                            }),
                                            DisplayOrder = i + 1,
                                            StatusID = 5,//TODO Get item active status id
                                            CreateTime = createTime,
                                            CreateBy = createBy,
                                            EditTime = createTime,
                                            EditBy = createBy
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        LogManager.Instance.Error(imageURL + " download failed. " + ex.Message);
                                    }

                                    i++;
                                }
                            }
                        }
                    }
                }



                return true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex.Message);
                return false;
            }
        }

        public IList<D_Item> GetItemsBySupplier(int supplierID)
        {
            var items = _itemRepository.Table.Where(i => i.SupplierID == supplierID);
            return items.ToList();
        }


        public D_Item GetItemByID(int id)
        {
            var item = _itemRepository.GetById(id);
            return item;
        }


        public void InsertItem(D_Item item)
        {
            if (item != null)
                _itemRepository.Insert(item);
        }

        public void UpdateItem(D_Item item)
        {
            if (item != null)
                _itemRepository.Update(item);
        }

        public void DeleteItem(D_Item item)
        {
            if (item != null)
                _itemRepository.Delete(item);
        }


        #region Private Method

        private IList<DropshipzoneSKUModel> GetSupplierSKUList()
        {
            IList<DropshipzoneSKUModel> skuList = null;
            using (var webClient = new DropshipWebClient())
            {
                var byCsv = webClient.DownloadData(Constants.DropshipzoneItemListStandardURL);

                using (var ms = new MemoryStream(byCsv))
                {
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms))
                    {
                        var csvFileDescription = new CsvFileDescription() { SeparatorChar = ',', FirstLineHasColumnNames = true, IgnoreUnknownColumns = true, TextEncoding = Encoding.Default };
                        var csvContext = new CsvContext();
                        skuList = csvContext.Read<DropshipzoneSKUModel>(sr, csvFileDescription).ToList();

                    }
                }
            }

            return skuList;
        }

        

        #endregion
    }
}
