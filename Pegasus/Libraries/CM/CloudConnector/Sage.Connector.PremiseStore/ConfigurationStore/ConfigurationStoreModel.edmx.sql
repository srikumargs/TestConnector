
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server Compact Edition
-- --------------------------------------------------
-- Date Created: 04/25/2014 11:07:39
-- Generated from EDMX file: C:\tfs\Sage Cloud\CloudConnector\Dev\vNextConWip\Net451Nuget\Libraries\CRE\CloudConnector\Sage.Connector.PremiseStore\ConfigurationStore\ConfigurationStoreModel.edmx
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- NOTE: if the constraint does not exist, an ignorable error will be reported.
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- NOTE: if the table does not exist, an ignorable error will be reported.
-- --------------------------------------------------

    DROP TABLE [PremiseConfigurations];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'PremiseConfigurations'
CREATE TABLE [PremiseConfigurations] (
    [Id] uniqueidentifier  NOT NULL,
    [BackOfficeCompanyUniqueId] nvarchar(4000)  NOT NULL,
    [PremiseAgent] nvarchar(4000)  NOT NULL,
    [CloudTenantId] nvarchar(1000)  NOT NULL,
    [CloudPremiseKey] nvarchar(1000)  NOT NULL,
    [SiteAddress] nvarchar(4000)  NOT NULL,
    [BackOfficeConnectionEnabledToReceive] bit  NOT NULL,
    [CloudConnectionEnabledToReceive] bit  NOT NULL,
    [CloudCompanyUrl] nvarchar(4000)  NULL,
    [CloudCompanyName] nvarchar(1000)  NULL,
    [BackOfficeCompanyName] nvarchar(1000)  NULL,
    [SentDocumentStoragePolicy] smallint  NOT NULL,
    [SentDocumentStorageDays] smallint  NOT NULL,
    [SentDocumentStorageMBs] bigint  NOT NULL,
    [SentDocumentFolderName] nvarchar(4000)  NOT NULL,
    [BackOfficeAllowableConcurrentExecutions] smallint  NOT NULL,
    [MinCommunicationFailureRetryInterval] int  NOT NULL,
    [MaxCommunicationFailureRetryInterval] int  NOT NULL,
    [CloudConnectionEnabledToSend] bit  NOT NULL,
    [ConnectorPluginId] nvarchar(200)  NOT NULL,
    [BackOfficeProductName] nvarchar(200)  NOT NULL,
    [BackOfficeConnectionCredentials] nvarchar(4000)  NOT NULL,
    [BackOfficePluginProductId] nvarchar(200)  NOT NULL,
    [CloudTenantClaim] nvarchar(4000)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'PremiseConfigurations'
ALTER TABLE [PremiseConfigurations]
ADD CONSTRAINT [PK_PremiseConfigurations]
    PRIMARY KEY ([Id] );
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------