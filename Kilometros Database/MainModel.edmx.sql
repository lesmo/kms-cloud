
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/17/2014 02:52:29
-- Generated from EDMX file: F:\Sharp Dynamics\Kilometros\Kilometros Database\MainModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [KmsCloud];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ApiKeyToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TokenSet] DROP CONSTRAINT [FK_ApiKeyToken];
GO
IF OBJECT_ID(N'[dbo].[FK_UserToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TokenSet] DROP CONSTRAINT [FK_UserToken];
GO
IF OBJECT_ID(N'[dbo].[FK_TipCategoryTipCategoryGlobalization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TipCategoryGlobalizationSet] DROP CONSTRAINT [FK_TipCategoryTipCategoryGlobalization];
GO
IF OBJECT_ID(N'[dbo].[FK_TipCategoryTip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TipSet] DROP CONSTRAINT [FK_TipCategoryTip];
GO
IF OBJECT_ID(N'[dbo].[FK_TipTipGlobalization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TipGlobalizationSet] DROP CONSTRAINT [FK_TipTipGlobalization];
GO
IF OBJECT_ID(N'[dbo].[FK_TipTipMotionLevel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MotionLevelSet] DROP CONSTRAINT [FK_TipTipMotionLevel];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserBody]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserBodySet] DROP CONSTRAINT [FK_UserUserBody];
GO
IF OBJECT_ID(N'[dbo].[FK_TipMotionLevelTipMotionLevelGlobalization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MotionLevelGlobalizationSet] DROP CONSTRAINT [FK_TipMotionLevelTipMotionLevelGlobalization];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardRewardGift]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftSet] DROP CONSTRAINT [FK_RewardRewardGift];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardRewardGlobalization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGlobalizationSet] DROP CONSTRAINT [FK_RewardRewardGlobalization];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardRewardRegionalization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardRegionalizationSet] DROP CONSTRAINT [FK_RewardRewardRegionalization];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardGiftRewardGiftGlobalization]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftGlobalizationSet] DROP CONSTRAINT [FK_RewardGiftRewardGiftGlobalization];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardGiftRewardGiftToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftTokenSet] DROP CONSTRAINT [FK_RewardGiftRewardGiftToken];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardGiftTokenUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftTokenSet] DROP CONSTRAINT [FK_RewardGiftTokenUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserMotionLevelHistory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMotionLevelHistorySet] DROP CONSTRAINT [FK_UserUserMotionLevelHistory];
GO
IF OBJECT_ID(N'[dbo].[FK_UserMotionLevelHistoryMotionLevel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserMotionLevelHistorySet] DROP CONSTRAINT [FK_UserMotionLevelHistoryMotionLevel];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserEarnedReward]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserEarnedRewardSet] DROP CONSTRAINT [FK_UserUserEarnedReward];
GO
IF OBJECT_ID(N'[dbo].[FK_UserEarnedRewardReward]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserEarnedRewardSet] DROP CONSTRAINT [FK_UserEarnedRewardReward];
GO
IF OBJECT_ID(N'[dbo].[FK_ContactInfoUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContactInfoSet] DROP CONSTRAINT [FK_ContactInfoUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserShippingInformation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ShippingInformationSet] DROP CONSTRAINT [FK_UserShippingInformation];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardGiftRewardGiftImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftImageSet] DROP CONSTRAINT [FK_RewardGiftRewardGiftImage];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSocialIdentity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SocialIdentitySet] DROP CONSTRAINT [FK_UserSocialIdentity];
GO
IF OBJECT_ID(N'[dbo].[FK_RegionRegionSubdivision]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegionSubdivisionSet] DROP CONSTRAINT [FK_RegionRegionSubdivision];
GO
IF OBJECT_ID(N'[dbo].[FK_ApiKeyHistory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ApiKeySet] DROP CONSTRAINT [FK_ApiKeyHistory];
GO
IF OBJECT_ID(N'[dbo].[FK_UserData]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DataSet] DROP CONSTRAINT [FK_UserData];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardGiftUserClaimedUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftUserClaimedSet] DROP CONSTRAINT [FK_RewardGiftUserClaimedUser];
GO
IF OBJECT_ID(N'[dbo].[FK_RewardGiftUserClaimedRewardGift]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RewardGiftUserClaimedSet] DROP CONSTRAINT [FK_RewardGiftUserClaimedRewardGift];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[TokenSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TokenSet];
GO
IF OBJECT_ID(N'[dbo].[ApiKeySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ApiKeySet];
GO
IF OBJECT_ID(N'[dbo].[TipSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipSet];
GO
IF OBJECT_ID(N'[dbo].[MotionLevelSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MotionLevelSet];
GO
IF OBJECT_ID(N'[dbo].[TipCategorySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipCategorySet];
GO
IF OBJECT_ID(N'[dbo].[TipCategoryGlobalizationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipCategoryGlobalizationSet];
GO
IF OBJECT_ID(N'[dbo].[TipGlobalizationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TipGlobalizationSet];
GO
IF OBJECT_ID(N'[dbo].[DataSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DataSet];
GO
IF OBJECT_ID(N'[dbo].[UserBodySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserBodySet];
GO
IF OBJECT_ID(N'[dbo].[MotionLevelGlobalizationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MotionLevelGlobalizationSet];
GO
IF OBJECT_ID(N'[dbo].[RewardSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardSet];
GO
IF OBJECT_ID(N'[dbo].[RewardGiftSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardGiftSet];
GO
IF OBJECT_ID(N'[dbo].[RewardGlobalizationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardGlobalizationSet];
GO
IF OBJECT_ID(N'[dbo].[RewardGiftTokenSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardGiftTokenSet];
GO
IF OBJECT_ID(N'[dbo].[RewardRegionalizationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardRegionalizationSet];
GO
IF OBJECT_ID(N'[dbo].[RewardGiftGlobalizationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardGiftGlobalizationSet];
GO
IF OBJECT_ID(N'[dbo].[UserMotionLevelHistorySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserMotionLevelHistorySet];
GO
IF OBJECT_ID(N'[dbo].[UserEarnedRewardSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserEarnedRewardSet];
GO
IF OBJECT_ID(N'[dbo].[ContactInfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContactInfoSet];
GO
IF OBJECT_ID(N'[dbo].[ShippingInformationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShippingInformationSet];
GO
IF OBJECT_ID(N'[dbo].[RewardGiftImageSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardGiftImageSet];
GO
IF OBJECT_ID(N'[dbo].[SocialIdentitySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SocialIdentitySet];
GO
IF OBJECT_ID(N'[dbo].[RegionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegionSet];
GO
IF OBJECT_ID(N'[dbo].[RegionSubdivisionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegionSubdivisionSet];
GO
IF OBJECT_ID(N'[dbo].[RewardGiftUserClaimedSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RewardGiftUserClaimedSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Guid] uniqueidentifier  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Password] varbinary(max)  NULL,
    [RegionCode] nchar(5)  NULL,
    [PreferredCultureCode] nvarchar(16)  NULL
);
GO

-- Creating table 'TokenSet'
CREATE TABLE [dbo].[TokenSet] (
    [CreationDate] datetime  NOT NULL,
    [ExpirationDate] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [LastUseDate] datetime  NULL,
    [ApiKey_Guid] uniqueidentifier  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ApiKeySet'
CREATE TABLE [dbo].[ApiKeySet] (
    [CreationDate] datetime  NOT NULL,
    [Platform] nvarchar(64)  NOT NULL,
    [Description] nvarchar(64)  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [Secret] uniqueidentifier  NOT NULL,
    [TokenUpgradeRequired] nvarchar(max)  NOT NULL,
    [DebugEnabled] bit  NOT NULL,
    [ApiKeyNext_Guid] uniqueidentifier  NULL
);
GO

-- Creating table 'TipSet'
CREATE TABLE [dbo].[TipSet] (
    [Guid] uniqueidentifier  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [DistanceTrigger] nvarchar(max)  NULL,
    [DaysTrigger] nvarchar(max)  NOT NULL,
    [Source] nvarchar(max)  NULL,
    [TipCategory_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'MotionLevelSet'
CREATE TABLE [dbo].[MotionLevelSet] (
    [Guid] uniqueidentifier  NOT NULL,
    [DistanceThresholdStart] int  NOT NULL,
    [DistanceThresholdEnd] int  NOT NULL,
    [DaysThresholdStart] smallint  NOT NULL,
    [DaysThresholdEnd] smallint  NOT NULL,
    [Tip_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'TipCategorySet'
CREATE TABLE [dbo].[TipCategorySet] (
    [Guid] uniqueidentifier  NOT NULL,
    [Icon] varbinary(max)  NOT NULL,
    [IconMimeType] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'DataSet'
CREATE TABLE [dbo].[DataSet] (
    [Timestamp] datetime  NOT NULL,
    [Steps] int  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserBodySet'
CREATE TABLE [dbo].[UserBodySet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Age] smallint  NOT NULL,
    [Height] smallint  NOT NULL,
    [Weight] int  NOT NULL,
    [Sex] nchar(1)  NOT NULL,
    [LastEditDate] datetime  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RewardSet'
CREATE TABLE [dbo].[RewardSet] (
    [Guid] uniqueidentifier  NOT NULL,
    [DistanceTrigger] nvarchar(max)  NOT NULL,
    [DaysTrigger] nvarchar(max)  NOT NULL,
    [Source] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RewardGiftSet'
CREATE TABLE [dbo].[RewardGiftSet] (
    [Guid] uniqueidentifier  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [Stock] int  NOT NULL,
    [Reward_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RewardGlobalizationSet'
CREATE TABLE [dbo].[RewardGlobalizationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CultureCode] nchar(16)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [Source] nvarchar(max)  NULL,
    [Reward_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RewardGiftTokenSet'
CREATE TABLE [dbo].[RewardGiftTokenSet] (
    [Guid] uniqueidentifier  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [ExpirationDate] datetime  NULL,
    [RedeemCode] nvarchar(max)  NOT NULL,
    [RedeemGraphic] varbinary(max)  NULL,
    [RedeemGraphicMimeType] nvarchar(max)  NULL,
    [RewardGift_Guid] uniqueidentifier  NOT NULL,
    [RedeemedByUser_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RewardRegionalizationSet'
CREATE TABLE [dbo].[RewardRegionalizationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RegionCode] nchar(2)  NOT NULL,
    [Reward_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RewardGiftGlobalizationSet'
CREATE TABLE [dbo].[RewardGiftGlobalizationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CultureCode] nvarchar(16)  NOT NULL,
    [NameSingular] nvarchar(140)  NOT NULL,
    [NamePlural] nvarchar(140)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [RewardGift_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserMotionLevelHistorySet'
CREATE TABLE [dbo].[UserMotionLevelHistorySet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL,
    [MotionLevel_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserEarnedRewardSet'
CREATE TABLE [dbo].[UserEarnedRewardSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL,
    [Reward_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContactInfoSet'
CREATE TABLE [dbo].[ContactInfoSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [HomePhone] nvarchar(16)  NULL,
    [MobilePhone] nvarchar(16)  NULL,
    [WorkPhone] nvarchar(max)  NULL,
    [LastEditDate] datetime  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ShippingInformationSet'
CREATE TABLE [dbo].[ShippingInformationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RegionCode] nchar(5)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Latitude] float  NOT NULL,
    [Longitude] float  NOT NULL,
    [LastEditDate] datetime  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RewardGiftImageSet'
CREATE TABLE [dbo].[RewardGiftImageSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [Image] varbinary(max)  NOT NULL,
    [ImageMimeType] nvarchar(max)  NOT NULL,
    [RewardGift_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SocialIdentitySet'
CREATE TABLE [dbo].[SocialIdentitySet] (
    [Guid] uniqueidentifier  NOT NULL,
    [FacebookUid] nvarchar(max)  NOT NULL,
    [FacebookToken] nvarchar(max)  NOT NULL,
    [FacebookTokenIsInvalid] bit  NOT NULL,
    [TwitterUid] nvarchar(max)  NOT NULL,
    [TwitterToken] nvarchar(max)  NOT NULL,
    [TwitterTokenSecret] nvarchar(max)  NOT NULL,
    [TwitterTokenIsInvalid] bit  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RegionSet'
CREATE TABLE [dbo].[RegionSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [TwoLetterIsoCode] nvarchar(max)  NOT NULL,
    [ThreeLetterIsoCode] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RegionSubdivisionSet'
CREATE TABLE [dbo].[RegionSubdivisionSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [IsoCode] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [UtcOffset] smallint  NOT NULL,
    [Region_Id] bigint  NOT NULL
);
GO

-- Creating table 'RewardGiftUserClaimedSet'
CREATE TABLE [dbo].[RewardGiftUserClaimedSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [ClaimLocation] geography  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL,
    [RewardGift_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserTipHistorySet'
CREATE TABLE [dbo].[UserTipHistorySet] (
    [Guid] uniqueidentifier  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [User_Guid] uniqueidentifier  NOT NULL,
    [Tip_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'TipCategoryGlobalizationSet'
CREATE TABLE [dbo].[TipCategoryGlobalizationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CultureCode] nvarchar(16)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [CreationDate] datetime  NOT NULL,
    [TipCategory_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'MotionLevelGlobalizationSet'
CREATE TABLE [dbo].[MotionLevelGlobalizationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CultureCode] nvarchar(16)  NOT NULL,
    [Tag] nvarchar(max)  NOT NULL,
    [MotionLevel_Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'IGlobalizationSet'
CREATE TABLE [dbo].[IGlobalizationSet] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CultureCode] nvarchar(16)  NOT NULL,
    [CreationDate] datetime  NOT NULL
);
GO

-- Creating table 'IGlobalizationSet_TipGlobalization'
CREATE TABLE [dbo].[IGlobalizationSet_TipGlobalization] (
    [Text] nvarchar(max)  NOT NULL,
    [Source] nvarchar(max)  NULL,
    [Id] bigint  NOT NULL,
    [Tip_Guid] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Guid] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'TokenSet'
ALTER TABLE [dbo].[TokenSet]
ADD CONSTRAINT [PK_TokenSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'ApiKeySet'
ALTER TABLE [dbo].[ApiKeySet]
ADD CONSTRAINT [PK_ApiKeySet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'TipSet'
ALTER TABLE [dbo].[TipSet]
ADD CONSTRAINT [PK_TipSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'MotionLevelSet'
ALTER TABLE [dbo].[MotionLevelSet]
ADD CONSTRAINT [PK_MotionLevelSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'TipCategorySet'
ALTER TABLE [dbo].[TipCategorySet]
ADD CONSTRAINT [PK_TipCategorySet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Timestamp] in table 'DataSet'
ALTER TABLE [dbo].[DataSet]
ADD CONSTRAINT [PK_DataSet]
    PRIMARY KEY CLUSTERED ([Timestamp] ASC);
GO

-- Creating primary key on [Id] in table 'UserBodySet'
ALTER TABLE [dbo].[UserBodySet]
ADD CONSTRAINT [PK_UserBodySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Guid] in table 'RewardSet'
ALTER TABLE [dbo].[RewardSet]
ADD CONSTRAINT [PK_RewardSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Guid] in table 'RewardGiftSet'
ALTER TABLE [dbo].[RewardGiftSet]
ADD CONSTRAINT [PK_RewardGiftSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Id] in table 'RewardGlobalizationSet'
ALTER TABLE [dbo].[RewardGlobalizationSet]
ADD CONSTRAINT [PK_RewardGlobalizationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Guid] in table 'RewardGiftTokenSet'
ALTER TABLE [dbo].[RewardGiftTokenSet]
ADD CONSTRAINT [PK_RewardGiftTokenSet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Id] in table 'RewardRegionalizationSet'
ALTER TABLE [dbo].[RewardRegionalizationSet]
ADD CONSTRAINT [PK_RewardRegionalizationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RewardGiftGlobalizationSet'
ALTER TABLE [dbo].[RewardGiftGlobalizationSet]
ADD CONSTRAINT [PK_RewardGiftGlobalizationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserMotionLevelHistorySet'
ALTER TABLE [dbo].[UserMotionLevelHistorySet]
ADD CONSTRAINT [PK_UserMotionLevelHistorySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserEarnedRewardSet'
ALTER TABLE [dbo].[UserEarnedRewardSet]
ADD CONSTRAINT [PK_UserEarnedRewardSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContactInfoSet'
ALTER TABLE [dbo].[ContactInfoSet]
ADD CONSTRAINT [PK_ContactInfoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ShippingInformationSet'
ALTER TABLE [dbo].[ShippingInformationSet]
ADD CONSTRAINT [PK_ShippingInformationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RewardGiftImageSet'
ALTER TABLE [dbo].[RewardGiftImageSet]
ADD CONSTRAINT [PK_RewardGiftImageSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Guid] in table 'SocialIdentitySet'
ALTER TABLE [dbo].[SocialIdentitySet]
ADD CONSTRAINT [PK_SocialIdentitySet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Id] in table 'RegionSet'
ALTER TABLE [dbo].[RegionSet]
ADD CONSTRAINT [PK_RegionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegionSubdivisionSet'
ALTER TABLE [dbo].[RegionSubdivisionSet]
ADD CONSTRAINT [PK_RegionSubdivisionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RewardGiftUserClaimedSet'
ALTER TABLE [dbo].[RewardGiftUserClaimedSet]
ADD CONSTRAINT [PK_RewardGiftUserClaimedSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Guid] in table 'UserTipHistorySet'
ALTER TABLE [dbo].[UserTipHistorySet]
ADD CONSTRAINT [PK_UserTipHistorySet]
    PRIMARY KEY CLUSTERED ([Guid] ASC);
GO

-- Creating primary key on [Id] in table 'TipCategoryGlobalizationSet'
ALTER TABLE [dbo].[TipCategoryGlobalizationSet]
ADD CONSTRAINT [PK_TipCategoryGlobalizationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MotionLevelGlobalizationSet'
ALTER TABLE [dbo].[MotionLevelGlobalizationSet]
ADD CONSTRAINT [PK_MotionLevelGlobalizationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IGlobalizationSet'
ALTER TABLE [dbo].[IGlobalizationSet]
ADD CONSTRAINT [PK_IGlobalizationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IGlobalizationSet_TipGlobalization'
ALTER TABLE [dbo].[IGlobalizationSet_TipGlobalization]
ADD CONSTRAINT [PK_IGlobalizationSet_TipGlobalization]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ApiKey_Guid] in table 'TokenSet'
ALTER TABLE [dbo].[TokenSet]
ADD CONSTRAINT [FK_ApiKeyToken]
    FOREIGN KEY ([ApiKey_Guid])
    REFERENCES [dbo].[ApiKeySet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ApiKeyToken'
CREATE INDEX [IX_FK_ApiKeyToken]
ON [dbo].[TokenSet]
    ([ApiKey_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'TokenSet'
ALTER TABLE [dbo].[TokenSet]
ADD CONSTRAINT [FK_UserToken]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserToken'
CREATE INDEX [IX_FK_UserToken]
ON [dbo].[TokenSet]
    ([User_Guid]);
GO

-- Creating foreign key on [TipCategory_Guid] in table 'TipCategoryGlobalizationSet'
ALTER TABLE [dbo].[TipCategoryGlobalizationSet]
ADD CONSTRAINT [FK_TipCategoryTipCategoryGlobalization]
    FOREIGN KEY ([TipCategory_Guid])
    REFERENCES [dbo].[TipCategorySet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TipCategoryTipCategoryGlobalization'
CREATE INDEX [IX_FK_TipCategoryTipCategoryGlobalization]
ON [dbo].[TipCategoryGlobalizationSet]
    ([TipCategory_Guid]);
GO

-- Creating foreign key on [TipCategory_Guid] in table 'TipSet'
ALTER TABLE [dbo].[TipSet]
ADD CONSTRAINT [FK_TipCategoryTip]
    FOREIGN KEY ([TipCategory_Guid])
    REFERENCES [dbo].[TipCategorySet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TipCategoryTip'
CREATE INDEX [IX_FK_TipCategoryTip]
ON [dbo].[TipSet]
    ([TipCategory_Guid]);
GO

-- Creating foreign key on [Tip_Guid] in table 'IGlobalizationSet_TipGlobalization'
ALTER TABLE [dbo].[IGlobalizationSet_TipGlobalization]
ADD CONSTRAINT [FK_TipTipGlobalization]
    FOREIGN KEY ([Tip_Guid])
    REFERENCES [dbo].[TipSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TipTipGlobalization'
CREATE INDEX [IX_FK_TipTipGlobalization]
ON [dbo].[IGlobalizationSet_TipGlobalization]
    ([Tip_Guid]);
GO

-- Creating foreign key on [Tip_Guid] in table 'MotionLevelSet'
ALTER TABLE [dbo].[MotionLevelSet]
ADD CONSTRAINT [FK_TipTipMotionLevel]
    FOREIGN KEY ([Tip_Guid])
    REFERENCES [dbo].[TipSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TipTipMotionLevel'
CREATE INDEX [IX_FK_TipTipMotionLevel]
ON [dbo].[MotionLevelSet]
    ([Tip_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'UserBodySet'
ALTER TABLE [dbo].[UserBodySet]
ADD CONSTRAINT [FK_UserUserBody]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserBody'
CREATE INDEX [IX_FK_UserUserBody]
ON [dbo].[UserBodySet]
    ([User_Guid]);
GO

-- Creating foreign key on [MotionLevel_Guid] in table 'MotionLevelGlobalizationSet'
ALTER TABLE [dbo].[MotionLevelGlobalizationSet]
ADD CONSTRAINT [FK_TipMotionLevelTipMotionLevelGlobalization]
    FOREIGN KEY ([MotionLevel_Guid])
    REFERENCES [dbo].[MotionLevelSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TipMotionLevelTipMotionLevelGlobalization'
CREATE INDEX [IX_FK_TipMotionLevelTipMotionLevelGlobalization]
ON [dbo].[MotionLevelGlobalizationSet]
    ([MotionLevel_Guid]);
GO

-- Creating foreign key on [Reward_Guid] in table 'RewardGiftSet'
ALTER TABLE [dbo].[RewardGiftSet]
ADD CONSTRAINT [FK_RewardRewardGift]
    FOREIGN KEY ([Reward_Guid])
    REFERENCES [dbo].[RewardSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardRewardGift'
CREATE INDEX [IX_FK_RewardRewardGift]
ON [dbo].[RewardGiftSet]
    ([Reward_Guid]);
GO

-- Creating foreign key on [Reward_Guid] in table 'RewardGlobalizationSet'
ALTER TABLE [dbo].[RewardGlobalizationSet]
ADD CONSTRAINT [FK_RewardRewardGlobalization]
    FOREIGN KEY ([Reward_Guid])
    REFERENCES [dbo].[RewardSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardRewardGlobalization'
CREATE INDEX [IX_FK_RewardRewardGlobalization]
ON [dbo].[RewardGlobalizationSet]
    ([Reward_Guid]);
GO

-- Creating foreign key on [Reward_Guid] in table 'RewardRegionalizationSet'
ALTER TABLE [dbo].[RewardRegionalizationSet]
ADD CONSTRAINT [FK_RewardRewardRegionalization]
    FOREIGN KEY ([Reward_Guid])
    REFERENCES [dbo].[RewardSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardRewardRegionalization'
CREATE INDEX [IX_FK_RewardRewardRegionalization]
ON [dbo].[RewardRegionalizationSet]
    ([Reward_Guid]);
GO

-- Creating foreign key on [RewardGift_Guid] in table 'RewardGiftGlobalizationSet'
ALTER TABLE [dbo].[RewardGiftGlobalizationSet]
ADD CONSTRAINT [FK_RewardGiftRewardGiftGlobalization]
    FOREIGN KEY ([RewardGift_Guid])
    REFERENCES [dbo].[RewardGiftSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardGiftRewardGiftGlobalization'
CREATE INDEX [IX_FK_RewardGiftRewardGiftGlobalization]
ON [dbo].[RewardGiftGlobalizationSet]
    ([RewardGift_Guid]);
GO

-- Creating foreign key on [RewardGift_Guid] in table 'RewardGiftTokenSet'
ALTER TABLE [dbo].[RewardGiftTokenSet]
ADD CONSTRAINT [FK_RewardGiftRewardGiftToken]
    FOREIGN KEY ([RewardGift_Guid])
    REFERENCES [dbo].[RewardGiftSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardGiftRewardGiftToken'
CREATE INDEX [IX_FK_RewardGiftRewardGiftToken]
ON [dbo].[RewardGiftTokenSet]
    ([RewardGift_Guid]);
GO

-- Creating foreign key on [RedeemedByUser_Guid] in table 'RewardGiftTokenSet'
ALTER TABLE [dbo].[RewardGiftTokenSet]
ADD CONSTRAINT [FK_RewardGiftTokenUser]
    FOREIGN KEY ([RedeemedByUser_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardGiftTokenUser'
CREATE INDEX [IX_FK_RewardGiftTokenUser]
ON [dbo].[RewardGiftTokenSet]
    ([RedeemedByUser_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'UserMotionLevelHistorySet'
ALTER TABLE [dbo].[UserMotionLevelHistorySet]
ADD CONSTRAINT [FK_UserUserMotionLevelHistory]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserMotionLevelHistory'
CREATE INDEX [IX_FK_UserUserMotionLevelHistory]
ON [dbo].[UserMotionLevelHistorySet]
    ([User_Guid]);
GO

-- Creating foreign key on [MotionLevel_Guid] in table 'UserMotionLevelHistorySet'
ALTER TABLE [dbo].[UserMotionLevelHistorySet]
ADD CONSTRAINT [FK_UserMotionLevelHistoryMotionLevel]
    FOREIGN KEY ([MotionLevel_Guid])
    REFERENCES [dbo].[MotionLevelSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserMotionLevelHistoryMotionLevel'
CREATE INDEX [IX_FK_UserMotionLevelHistoryMotionLevel]
ON [dbo].[UserMotionLevelHistorySet]
    ([MotionLevel_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'UserEarnedRewardSet'
ALTER TABLE [dbo].[UserEarnedRewardSet]
ADD CONSTRAINT [FK_UserUserEarnedReward]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserEarnedReward'
CREATE INDEX [IX_FK_UserUserEarnedReward]
ON [dbo].[UserEarnedRewardSet]
    ([User_Guid]);
GO

-- Creating foreign key on [Reward_Guid] in table 'UserEarnedRewardSet'
ALTER TABLE [dbo].[UserEarnedRewardSet]
ADD CONSTRAINT [FK_UserEarnedRewardReward]
    FOREIGN KEY ([Reward_Guid])
    REFERENCES [dbo].[RewardSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserEarnedRewardReward'
CREATE INDEX [IX_FK_UserEarnedRewardReward]
ON [dbo].[UserEarnedRewardSet]
    ([Reward_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'ContactInfoSet'
ALTER TABLE [dbo].[ContactInfoSet]
ADD CONSTRAINT [FK_ContactInfoUser]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContactInfoUser'
CREATE INDEX [IX_FK_ContactInfoUser]
ON [dbo].[ContactInfoSet]
    ([User_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'ShippingInformationSet'
ALTER TABLE [dbo].[ShippingInformationSet]
ADD CONSTRAINT [FK_UserShippingInformation]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserShippingInformation'
CREATE INDEX [IX_FK_UserShippingInformation]
ON [dbo].[ShippingInformationSet]
    ([User_Guid]);
GO

-- Creating foreign key on [RewardGift_Guid] in table 'RewardGiftImageSet'
ALTER TABLE [dbo].[RewardGiftImageSet]
ADD CONSTRAINT [FK_RewardGiftRewardGiftImage]
    FOREIGN KEY ([RewardGift_Guid])
    REFERENCES [dbo].[RewardGiftSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardGiftRewardGiftImage'
CREATE INDEX [IX_FK_RewardGiftRewardGiftImage]
ON [dbo].[RewardGiftImageSet]
    ([RewardGift_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'SocialIdentitySet'
ALTER TABLE [dbo].[SocialIdentitySet]
ADD CONSTRAINT [FK_UserSocialIdentity]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSocialIdentity'
CREATE INDEX [IX_FK_UserSocialIdentity]
ON [dbo].[SocialIdentitySet]
    ([User_Guid]);
GO

-- Creating foreign key on [Region_Id] in table 'RegionSubdivisionSet'
ALTER TABLE [dbo].[RegionSubdivisionSet]
ADD CONSTRAINT [FK_RegionRegionSubdivision]
    FOREIGN KEY ([Region_Id])
    REFERENCES [dbo].[RegionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RegionRegionSubdivision'
CREATE INDEX [IX_FK_RegionRegionSubdivision]
ON [dbo].[RegionSubdivisionSet]
    ([Region_Id]);
GO

-- Creating foreign key on [ApiKeyNext_Guid] in table 'ApiKeySet'
ALTER TABLE [dbo].[ApiKeySet]
ADD CONSTRAINT [FK_ApiKeyHistory]
    FOREIGN KEY ([ApiKeyNext_Guid])
    REFERENCES [dbo].[ApiKeySet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ApiKeyHistory'
CREATE INDEX [IX_FK_ApiKeyHistory]
ON [dbo].[ApiKeySet]
    ([ApiKeyNext_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'DataSet'
ALTER TABLE [dbo].[DataSet]
ADD CONSTRAINT [FK_UserData]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserData'
CREATE INDEX [IX_FK_UserData]
ON [dbo].[DataSet]
    ([User_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'RewardGiftUserClaimedSet'
ALTER TABLE [dbo].[RewardGiftUserClaimedSet]
ADD CONSTRAINT [FK_RewardGiftUserClaimedUser]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardGiftUserClaimedUser'
CREATE INDEX [IX_FK_RewardGiftUserClaimedUser]
ON [dbo].[RewardGiftUserClaimedSet]
    ([User_Guid]);
GO

-- Creating foreign key on [RewardGift_Guid] in table 'RewardGiftUserClaimedSet'
ALTER TABLE [dbo].[RewardGiftUserClaimedSet]
ADD CONSTRAINT [FK_RewardGiftUserClaimedRewardGift]
    FOREIGN KEY ([RewardGift_Guid])
    REFERENCES [dbo].[RewardGiftSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RewardGiftUserClaimedRewardGift'
CREATE INDEX [IX_FK_RewardGiftUserClaimedRewardGift]
ON [dbo].[RewardGiftUserClaimedSet]
    ([RewardGift_Guid]);
GO

-- Creating foreign key on [User_Guid] in table 'UserTipHistorySet'
ALTER TABLE [dbo].[UserTipHistorySet]
ADD CONSTRAINT [FK_UserTipHistoryUser]
    FOREIGN KEY ([User_Guid])
    REFERENCES [dbo].[UserSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTipHistoryUser'
CREATE INDEX [IX_FK_UserTipHistoryUser]
ON [dbo].[UserTipHistorySet]
    ([User_Guid]);
GO

-- Creating foreign key on [Tip_Guid] in table 'UserTipHistorySet'
ALTER TABLE [dbo].[UserTipHistorySet]
ADD CONSTRAINT [FK_UserTipHistoryTip]
    FOREIGN KEY ([Tip_Guid])
    REFERENCES [dbo].[TipSet]
        ([Guid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTipHistoryTip'
CREATE INDEX [IX_FK_UserTipHistoryTip]
ON [dbo].[UserTipHistorySet]
    ([Tip_Guid]);
GO

-- Creating foreign key on [Id] in table 'IGlobalizationSet_TipGlobalization'
ALTER TABLE [dbo].[IGlobalizationSet_TipGlobalization]
ADD CONSTRAINT [FK_TipGlobalization_inherits_IGlobalization]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[IGlobalizationSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------