select * 
from D_Item
where StatusID=2

select * from M_ItemImage

select * from D_Image

select * 
from T_Status

select * from D_Listing


select * from T_ListingChannel

select * from T_User

--insert new item to old dropship database

insert into AutoPostAdDropship.dbo.AutoPostAdPostData
select 
New.SKU,
New.Price,
New.Title,
0 as CategoryID,
New.InventoryQty,
1 as AddressID,
1 as AccountID,
1 as CustomFieldGroupID,
'' as BusinessLogoPath,
New.Description,
'' as ImagesPath,
'' as CustomID,
New.StatusID as Status,
0 as Postage,
case when New.PostageRuleID=1 then 'FreeShipping' else '' end as Notes,-- will need to change postage rule ID
4 as AdTypeID,
1 as ScheduleRuleID
from 
(
select * from Dropship.dbo.D_Item
where SupplierID=1
) New 
left join 
(
select *
from AutoPostAdDropship.dbo.AutoPostAdPostData
where AdTypeID=4
) Old on New.SKU=Old.SKU
where Old.SKU is null

--Fix dropship images data
--ebayService.GetDropshipzoneProductImagesPath();


--update old dropship database


update  U
--select *
set InventoryQty=New.InventoryQty,
Price=New.Price,
Status=New.StatusID,
Notes=case when New.PostageRuleID=1 then 'FreeShipping' else '' end,
Postage=case when Old.Postage=0 then New.MaxPostage else Old.Postage end
from AutoPostAdDropship.dbo.AutoPostAdPostData U
inner join
(
select *
from AutoPostAdDropship.dbo.AutoPostAdPostData
where AdTypeID=4
) Old on U.ID=Old.ID
inner join
(
	select I.*,isnull(MaxPostage,0) as MaxPostage
	from Dropship.dbo.D_Item I
	left join 
	(
		select 
		PostageRuleID,
		MAX(cast(Formula as decimal(18,2))) as MaxPostage
		from Dropship.dbo.T_PostageRuleLine
		group by PostageRuleID
	) P on I.PostageRuleID=P.PostageRuleID
	where SupplierID=1
	--and StatusID=1
) New on New.SKU=Old.SKU








--Data Insert

--T_Supplier
insert into T_Supplier
select
'New Aim DropshipZone' as Name,
'New Aim Pty Ltd.' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


--T_User
insert into T_User
select
'Jim' as Name,
'Software Designer Jim' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_User
select
'Hong Lang' as Name,
'Panyu Hong Lang' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy






--T_ListingChannel
insert into T_ListingChannel
select
2 as UserID,
'Crazy Mall eBay' as Name,
'Crazy Mall eBay' as Description,
'crazy-mall' as Ref1,
'AgAAAA**AQAAAA**aAAAAA**KGibVA**nY+sHZ2PrBmdj6wVnY+sEZ2PrA2dj6AFk4WjDpiBoA6dj6x9nY+seQ**fAYBAA**AAMAAA**FUq6vGZAH5ojYHRQoIO7+ma0TixoXM2RvTrWldG1OhXwbnqKAZ19wz/dbG/b3NV7upfFn/Q/ON06rdXXxugW23PkW97mS1Oht+XrLOjICHKq6tJ3/uo/vMfkQPodG39YAJocS4MqZ72it383OPwGAbEvt3U1Y2ymbgJnMO83qdS6THSBEVvj23mnmPhHvjURMKvlr+s3sSz3cr9U6ml5OmKzgVTsYj40m8mr9LbNQIOUUs8OVz+zrcvyENEum9XAl2t+cxVKGw0yX08QXjlxgEWj6EXhLyPJ/WC/d1arFR2QElYlI5Qfbsri7CJ4xybzENbCKY6zh/uwbx0YZ/PrDUHac2DsFb2EFMjmpYNyERs6IP3qLPFk37giJJ7CUq2SvAEjMav812OGGYvlBhMvnYOf7wpzgIHbksJJghEBE8hr4kl0+wWDraUT/MO8/CQrU/Mry3lTMMbVU0PZCOKk6CRtwmD1nAUEuedq7V4lCd8N8XdBs0o3YxM3aCrOZc12+67IfV4q0/8ZUUGFrk6hEC8jMniuqmeC/6ks3UCChjEaa7UPUlbHQ9OKDw6jUefvVImDwl0nFt31Xcv6o3u/mNhN0esMBn08esNTL1ZNWPmLm9LbtTkNajbr5STa7xy3rgAw+RNsTcVzp9vIlPxD/AS3BWSDn8pOwxRU+y7Z8bEXRbk3PQ3BBEXdecSnbbSVkkkFjMS+4xSdrVXwZkwk4KV4zu64yhUUDrmk7ksdTqFz9uD03v2eG3qMkbszTk8U' as Ref2,
'' as Ref3,
'' as Ref4,
'' as Ref5,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy



--T_Status
insert into T_Status
select
'ITEM' as EntityType,
'Active' as Name,
'Item Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'ITEM' as EntityType,
'Disabled' as Name,
'Item Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy

insert into T_Status
select
'LISTING' as EntityType,
'Active' as Name,
'Listing Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'LISTING' as EntityType,
'Disabled' as Name,
'Listing Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'IMAGE' as EntityType,
'Active' as Name,
'Image Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'IMAGE' as EntityType,
'Disabled' as Name,
'Image Disabled Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'USER' as EntityType,
'Active' as Name,
'User Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'USER' as EntityType,
'Disabled' as Name,
'User Disabled Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'ROLE' as EntityType,
'Active' as Name,
'Role Active Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_Status
select
'ROLE' as EntityType,
'Disabled' as Name,
'Role Disabled Status' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


--T_ValueRule
insert into T_ValueRule
select
'crazy-mall price rule' as Name,
'crazy-mall price rule earn at least 10% selling price' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


--T_ValueRuleLine
insert into T_ValueRuleLine
select
ID as ValueRuleID,
'D_Item.Price' as FieldName,
0 as MinValue,
9999999999 as MaxValue,
'Abs(Ceiling(( {{ItemPrice}}+Abs(0.3))/Abs(0.774)))-Abs(0.05)' as Formula,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy
from T_ValueRule
where Name='crazy-mall price rule'


--crazy mall inventory qty rule
insert into T_ValueRule
select
'crazy-mall inventory qty rule' as Name,
'crazy-mall inventory qty rule' as Description,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy


insert into T_ValueRuleLine
select
ID as ValueRuleID,
'D_Item.InventoryQty' as FieldName,
50 as MinValue,
9999999999 as MaxValue,
'10' as Formula,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy
from T_ValueRule
where Name='crazy-mall inventory qty rule'

insert into T_ValueRuleLine
select
ID as ValueRuleID,
'D_Item.InventoryQty' as FieldName,
5 as MinValue,
49.99 as MaxValue,
'5' as Formula,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy
from T_ValueRule
where Name='crazy-mall inventory qty rule'

insert into T_ValueRuleLine
select
ID as ValueRuleID,
'D_Item.InventoryQty' as FieldName,
0 as MinValue,
4.99 as MaxValue,
'0' as Formula,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy
from T_ValueRule
where Name='crazy-mall inventory qty rule'




insert into D_Item
select 
SKU,
Title,
Description,
Price,
InventoryQty,
case when Status='2' then 2 else 1 end as Status,
0 as PostageRuleID,
1 as SupplierID,
CustomID as SupplierItemID,
'' as Ref1,
'' as Ref2,
'' as Ref3,
'' as Ref4,
'' as Ref5,
GETDATE() as CreateTime,
'System' as CreateBy,
GETDATE() as EditTime,
'System' as EditBy
from AutoPostAdDropship.dbo.AutoPostAdPostData D
where AdTypeID=4



--check eBay








select * from
(
select * from D_Listing
) L inner join
(
select * from DSZItemChanges
) C on L.ListingSKU=C.Title
where L.ListingStatusID=3
and C.Priority='Critical'
order by C.[Add Date] desc