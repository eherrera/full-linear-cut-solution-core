
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/03/2018 02:38:53
-- Generated from EDMX file: D:\projects\own\full-linear-cut-solution\full-linear-cut-solution-app\full-linear-cut-solution-core\FullLinearCutSolutionContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [linear-cut-solution-db];
GO
IF SCHEMA_ID(N'linear-cut-solution-db') IS NULL EXECUTE(N'CREATE SCHEMA [linear-cut-solution-db]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[linear-cut-solution-db].[FK_MeasurementUnitParams]', 'F') IS NOT NULL
    ALTER TABLE [linear-cut-solution-db].[Params] DROP CONSTRAINT [FK_MeasurementUnitParams];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[linear-cut-solution-db].[MeasurementUnit]', 'U') IS NOT NULL
    DROP TABLE [linear-cut-solution-db].[MeasurementUnit];
GO
IF OBJECT_ID(N'[linear-cut-solution-db].[BarPatterns]', 'U') IS NOT NULL
    DROP TABLE [linear-cut-solution-db].[BarPatterns];
GO
IF OBJECT_ID(N'[linear-cut-solution-db].[Params]', 'U') IS NOT NULL
    DROP TABLE [linear-cut-solution-db].[Params];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MeasurementUnit'
CREATE TABLE [linear-cut-solution-db].[MeasurementUnit] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'BarPatterns'
CREATE TABLE [linear-cut-solution-db].[BarPatterns] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Diameter] nvarchar(max)  NOT NULL,
    [Length] decimal(18,0)  NOT NULL,
    [MinLengthReusable] decimal(18,0)  NOT NULL
);
GO

-- Creating table 'Params'
CREATE TABLE [linear-cut-solution-db].[Params] (
    [Id] int  NOT NULL,
    [MeasurementUnit_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'MeasurementUnit'
ALTER TABLE [linear-cut-solution-db].[MeasurementUnit]
ADD CONSTRAINT [PK_MeasurementUnit]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BarPatterns'
ALTER TABLE [linear-cut-solution-db].[BarPatterns]
ADD CONSTRAINT [PK_BarPatterns]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Params'
ALTER TABLE [linear-cut-solution-db].[Params]
ADD CONSTRAINT [PK_Params]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MeasurementUnit_Id] in table 'Params'
ALTER TABLE [linear-cut-solution-db].[Params]
ADD CONSTRAINT [FK_MeasurementUnitParams]
    FOREIGN KEY ([MeasurementUnit_Id])
    REFERENCES [linear-cut-solution-db].[MeasurementUnit]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MeasurementUnitParams'
CREATE INDEX [IX_FK_MeasurementUnitParams]
ON [linear-cut-solution-db].[Params]
    ([MeasurementUnit_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------