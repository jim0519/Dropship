
--Dropship Item
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[D_Item]') AND type in (N'U'))
DROP TABLE [dbo].[D_Item]
GO
CREATE TABLE [dbo].[D_Item](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SKU] [varchar](4000) NOT NULL,
	[Title] [varchar](4000) NOT NULL,
	[Description] [varchar](MAX) NOT NULL,
	[Price] decimal(18,2) NOT NULL,
	[InventoryQty] [int] NOT NULL,
	--[CategoryIDs][varchar](4000) NOT NULL,
	[StatusID][int] NOT NULL,
	[PostageRuleID] int NOT NULL,
	[SupplierID][int] NOT NULL,
	[SupplierItemID][varchar](4000) NOT NULL,--supplier system custom id
	[Ref1] [varchar](4000) NOT NULL,
	[Ref2] [varchar](4000) NOT NULL,
	[Ref3] [varchar](4000) NOT NULL,
	[Ref4] [varchar](4000) NOT NULL,
	[Ref5] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_D_Item] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--Inventory and Price Rule

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_ValueRule]') AND type in (N'U'))
DROP TABLE [dbo].[T_ValueRule]
GO
CREATE TABLE [dbo].[T_ValueRule](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](MAX) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_ValueRule] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_ValueRuleLine]') AND type in (N'U'))
DROP TABLE [dbo].[T_ValueRuleLine]
GO
CREATE TABLE [dbo].[T_ValueRuleLine](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ValueRuleID] [int] NOT NULL,
	[FieldName] [varchar](4000) NOT NULL,
	[MinValue] decimal(18,2) NOT NULL,
	[MaxValue] decimal(18,2) NOT NULL,
	[Formula] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_D_ValueRule] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_ValueRuleLine_T_ValueRule]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_ValueRuleLine]'))
ALTER TABLE [dbo].[T_ValueRuleLine]  WITH CHECK ADD CONSTRAINT [FK_T_ValueRuleLine_T_ValueRule] FOREIGN KEY([ValueRuleID])
REFERENCES [dbo].[T_ValueRule] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO





--Postage rule
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_PostageRule]') AND type in (N'U'))
DROP TABLE [dbo].[T_PostageRule]
GO
CREATE TABLE [dbo].[T_PostageRule](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](MAX) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_PostageRule] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_PostageRuleLine]') AND type in (N'U'))
DROP TABLE [dbo].[T_PostageRuleLine]
GO
CREATE TABLE [dbo].[T_PostageRuleLine](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PostageRuleID] [int] NOT NULL,
	[PostcodeFrom] [varchar](4000) NOT NULL,
	[PostcodeTo] [varchar](4000) not null,
	[Formula] [varchar](4000) not null,--Store formula, do not need a fixed postage because this is a rule and rule will not change frequently
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_PostageRuleLine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_PostageRuleLine_T_PostageRule]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_PostageRuleLine]'))
ALTER TABLE [dbo].[T_PostageRuleLine]  WITH CHECK ADD CONSTRAINT [FK_T_PostageRuleLine_T_PostageRule] FOREIGN KEY([PostageRuleID])
REFERENCES [dbo].[T_PostageRule] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO



--Dropship Listing

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[D_Listing]') AND type in (N'U'))
DROP TABLE [dbo].[D_Listing]
GO
CREATE TABLE [dbo].[D_Listing](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,--D_Item ID, one listing may contain multiple item ids, but now we do not consider variation on ebay
	[ListingChannelID] [int] NOT NULL,--Listing channel id
	--[UserID] [int] NOT NULL,
	[ListingID] [varchar](4000) NOT NULL,
	[ListingSKU] [varchar](4000) NOT NULL,
	[ListingTitle] [varchar](4000) NOT NULL,
	[ListingDescription] [varchar](MAX) NOT NULL,
	[ListingPrice] decimal(18,2) NOT NULL,
	[ListingInventoryQty] [int] NOT NULL,
	--[CategoryIDs][varchar](4000) NOT NULL,
	[ListingStatusID][int] NOT NULL,
	--[IsFreeshipping][bit] NOT NULL,
	[ListingPriceRuleID] [int] NOT NULL,
	[ListingInventoryQtyRuleID] [int] NOT NULL,
	[ListingPostageRuleID] [int] NOT NULL,
	[ListingDescriptionTemplateID] [int] NOT NULL,
	[LastUpdateTime] datetime not null,
	[Ref1] [varchar](4000) NOT NULL,
	[Ref2] [varchar](4000) NOT NULL,
	[Ref3] [varchar](4000) NOT NULL,
	[Ref4] [varchar](4000) NOT NULL,
	[Ref5] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_D_Listing] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[D_Listing]  WITH NOCHECK ADD  CONSTRAINT [FK_D_Listing_T_ValueRule] FOREIGN KEY([ListingPriceRuleID])
REFERENCES [dbo].[T_ValueRule] ([ID])
GO
ALTER TABLE [dbo].[D_Listing] NOCHECK CONSTRAINT [FK_D_Listing_T_ValueRule]
GO

ALTER TABLE [dbo].[D_Listing]  WITH NOCHECK ADD  CONSTRAINT [FK_D_Listing_T_ValueRule_ListingInventoryQtyRule] FOREIGN KEY([ListingInventoryQtyRuleID])
REFERENCES [dbo].[T_ValueRule] ([ID])
GO
ALTER TABLE [dbo].[D_Listing] NOCHECK CONSTRAINT [FK_D_Listing_T_ValueRule_ListingInventoryQtyRule]
GO

ALTER TABLE [dbo].[D_Listing]  WITH NOCHECK ADD  CONSTRAINT [FK_D_Listing_T_ValueRule_ListingPostageRule] FOREIGN KEY([ListingPostageRuleID])
REFERENCES [dbo].[T_ValueRule] ([ID])
GO
ALTER TABLE [dbo].[D_Listing] NOCHECK CONSTRAINT [FK_D_Listing_T_ValueRule_ListingPostageRule]
GO

--ALTER TABLE [dbo].[D_Listing]  WITH NOCHECK ADD  CONSTRAINT [FK_D_Listing_T_ValueRule_ListingDescriptionTemplate] FOREIGN KEY([ListingDescriptionTemplateID])
--REFERENCES [dbo].[T_ValueRule] ([ID])
--GO

--Listing Description Template
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_ListingDescriptionTemplate]') AND type in (N'U'))
DROP TABLE [dbo].[T_ListingDescriptionTemplate]
GO
CREATE TABLE [dbo].[T_ListingDescriptionTemplate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[Template] [varchar](MAX) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_ListingDescriptionTemplate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--Listing Channel
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_ListingChannel]') AND type in (N'U'))
DROP TABLE [dbo].[T_ListingChannel]
GO
CREATE TABLE [dbo].[T_ListingChannel](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[Ref1] [varchar](4000) NOT NULL,--user id
	[Ref2] [varchar](4000) NOT NULL,--password or token
	[Ref3] [varchar](4000) NOT NULL,--site id
	[Ref4] [varchar](4000) NOT NULL,--service url
	[Ref5] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_ListingChannel] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_D_Listing_T_ListingChannel]') AND parent_object_id = OBJECT_ID(N'[dbo].[D_Listing]'))
ALTER TABLE [dbo].[D_Listing]  WITH CHECK ADD CONSTRAINT [FK_D_Listing_T_ListingChannel] FOREIGN KEY([ListingChannelID])
REFERENCES [dbo].[T_ListingChannel] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


--Suppliers

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Supplier]') AND type in (N'U'))
DROP TABLE [dbo].[T_Supplier]
GO
CREATE TABLE [dbo].[T_Supplier](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_Supplier] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_D_Item_T_Supplier]') AND parent_object_id = OBJECT_ID(N'[dbo].[D_Item]'))
ALTER TABLE [dbo].[D_Item]  WITH CHECK ADD CONSTRAINT [FK_D_Item_T_Supplier] FOREIGN KEY([SupplierID])
REFERENCES [dbo].[T_Supplier] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


--Users

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_User]') AND type in (N'U'))
DROP TABLE [dbo].[T_User]
GO
CREATE TABLE [dbo].[T_User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_T_ListingChannel_T_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[T_ListingChannel]'))
ALTER TABLE [dbo].[T_ListingChannel]  WITH CHECK ADD CONSTRAINT [FK_T_ListingChannel_T_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[T_User] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO



--Add StatusID
IF NOT EXISTS (SELECT * FROM SysObjects O INNER JOIN SysColumns C ON O.ID=C.ID WHERE
 ObjectProperty(O.ID,'IsUserTable')=1 AND O.Name='T_User' AND C.Name='StatusID')
	ALTER TABLE dbo.T_User ADD
		StatusID int NOT NULL CONSTRAINT DF_T_User_StatusID DEFAULT 7
GO
		
IF EXISTS (SELECT [name] FROM sysobjects WHERE [name] = 'DF_T_User_StatusID')
	ALTER TABLE dbo.T_User
		DROP CONSTRAINT DF_T_User_StatusID
GO

--Add Email, Password, PasswordSalt
IF NOT EXISTS (SELECT * FROM SysObjects O INNER JOIN SysColumns C ON O.ID=C.ID WHERE
 ObjectProperty(O.ID,'IsUserTable')=1 AND O.Name='T_User' AND C.Name='Email')
	ALTER TABLE dbo.T_User ADD
		Email varchar(4000) NOT NULL CONSTRAINT DF_T_User_Email DEFAULT ''
GO
		
IF EXISTS (SELECT [name] FROM sysobjects WHERE [name] = 'DF_T_User_Email')
	ALTER TABLE dbo.T_User
		DROP CONSTRAINT DF_T_User_Email
GO

IF NOT EXISTS (SELECT * FROM SysObjects O INNER JOIN SysColumns C ON O.ID=C.ID WHERE
 ObjectProperty(O.ID,'IsUserTable')=1 AND O.Name='T_User' AND C.Name='Password')
	ALTER TABLE dbo.T_User ADD
		[Password] varchar(4000) NOT NULL CONSTRAINT DF_T_User_Password DEFAULT ''
GO
		
IF EXISTS (SELECT [name] FROM sysobjects WHERE [name] = 'DF_T_User_Password')
	ALTER TABLE dbo.T_User
		DROP CONSTRAINT DF_T_User_Password
GO

IF NOT EXISTS (SELECT * FROM SysObjects O INNER JOIN SysColumns C ON O.ID=C.ID WHERE
 ObjectProperty(O.ID,'IsUserTable')=1 AND O.Name='T_User' AND C.Name='PasswordSalt')
	ALTER TABLE dbo.T_User ADD
		PasswordSalt varchar(4000) NOT NULL CONSTRAINT DF_T_User_PasswordSalt DEFAULT ''
GO
		
IF EXISTS (SELECT [name] FROM sysobjects WHERE [name] = 'DF_T_User_PasswordSalt')
	ALTER TABLE dbo.T_User
		DROP CONSTRAINT DF_T_User_PasswordSalt
GO

--Status

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Status]') AND type in (N'U'))
DROP TABLE [dbo].[T_Status]
GO
CREATE TABLE [dbo].[T_Status](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EntityType] [varchar](4000) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_Status] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--Categories
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Category]') AND type in (N'U'))
DROP TABLE [dbo].[T_Category]
GO
CREATE TABLE [dbo].[T_Category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [varchar](4000) NOT NULL,
	[CategoryName][varchar](4000) NOT NULL,
	[ParentCategoryID] [varchar](4000) NOT NULL,
	[SupplierID] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_Category] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



--Images

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[D_Image]') AND type in (N'U'))
DROP TABLE [dbo].[D_Image]
GO
CREATE TABLE [dbo].[D_Image](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ImagePath] [varchar](4000) NOT NULL,
	--[DisplayOrder] [int] NOT NULL,
	--[Status][varchar](20) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_D_Image] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[M_ItemImage]') AND type in (N'U'))
DROP TABLE [dbo].[M_ItemImage]
GO
CREATE TABLE [dbo].[M_ItemImage](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ImageID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_M_ItemImage] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_M_ItemImage_D_Image]') AND parent_object_id = OBJECT_ID(N'[dbo].[M_ItemImage]'))
ALTER TABLE [dbo].[M_ItemImage]  WITH CHECK ADD CONSTRAINT [FK_M_ItemImage_D_Image] FOREIGN KEY([ImageID])
REFERENCES [dbo].[D_Image] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_M_ItemImage_D_Item]') AND parent_object_id = OBJECT_ID(N'[dbo].[M_ItemImage]'))
ALTER TABLE [dbo].[M_ItemImage]  WITH CHECK ADD CONSTRAINT [FK_M_ItemImage_D_Item] FOREIGN KEY([ItemID])
REFERENCES [dbo].[D_Item] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO










IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Role]') AND type in (N'U'))
DROP TABLE [dbo].[T_Role]
GO
CREATE TABLE [dbo].[T_Role](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[StatusID] [int] NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_Role] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[M_UserRole]') AND type in (N'U'))
DROP TABLE [dbo].[M_UserRole]
GO
CREATE TABLE [dbo].[M_UserRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[RoleID] [int] NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_M_UserRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_M_UserRole_T_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[M_UserRole]'))
ALTER TABLE [dbo].[M_UserRole]  WITH CHECK ADD CONSTRAINT [FK_M_UserRole_T_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[T_User] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_M_UserRole_T_Role]') AND parent_object_id = OBJECT_ID(N'[dbo].[M_UserRole]'))
ALTER TABLE [dbo].[M_UserRole]  WITH CHECK ADD CONSTRAINT [FK_M_UserRole_T_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[T_Role] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_Permission]') AND type in (N'U'))
DROP TABLE [dbo].[T_Permission]
GO
CREATE TABLE [dbo].[T_Permission](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](4000) NOT NULL,
	[Description] [varchar](4000) NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_T_Permission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[M_RolePermission]') AND type in (N'U'))
DROP TABLE [dbo].[M_RolePermission]
GO
CREATE TABLE [dbo].[M_RolePermission](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
	[CreateTime] datetime not null,
	[CreateBy] [varchar](4000) not null,
	[EditTime] datetime not null,
	[EditBy] [varchar](4000) not null,

 CONSTRAINT [PK_M_RolePermission] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_M_RolePermission_T_Role]') AND parent_object_id = OBJECT_ID(N'[dbo].[M_RolePermission]'))
ALTER TABLE [dbo].[M_RolePermission]  WITH CHECK ADD CONSTRAINT [FK_M_RolePermission_T_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[T_Role] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_M_RolePermission_T_Permission]') AND parent_object_id = OBJECT_ID(N'[dbo].[M_RolePermission]'))
ALTER TABLE [dbo].[M_RolePermission]  WITH CHECK ADD CONSTRAINT [FK_M_RolePermission_T_Permission] FOREIGN KEY([PermissionID])
REFERENCES [dbo].[T_Permission] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO