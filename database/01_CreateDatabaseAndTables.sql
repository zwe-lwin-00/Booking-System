-- =============================================
-- Create Database and Tables Script
-- Booking System - All-in-One Database Setup
-- =============================================
-- This script creates the database, all tables, indexes, and reference data
-- Run this script to set up a fresh database ready for use
-- =============================================

-- =============================================
-- Step 1: Create Database
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BookingSystemDb')
BEGIN
    CREATE DATABASE [BookingSystemDb]
    COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT 'Database BookingSystemDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database BookingSystemDb already exists.';
END
GO

USE [BookingSystemDb];
GO

-- =============================================
-- Step 2: Create Tables
-- =============================================

-- =============================================
-- Table: Countries
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Countries] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL,
        [Code] NVARCHAR(10) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [UQ_Countries_Code] UNIQUE ([Code])
    );
    PRINT 'Table Countries created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Countries already exists.';
END
GO

-- =============================================
-- Table: Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [FirstName] NVARCHAR(100) NOT NULL,
        [LastName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NULL,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [IsEmailVerified] BIT NOT NULL DEFAULT 0,
        [EmailVerificationToken] NVARCHAR(255) NULL,
        [EmailVerificationTokenExpiry] DATETIME2 NULL,
        [PasswordResetToken] NVARCHAR(255) NULL,
        [PasswordResetTokenExpiry] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
    );
    PRINT 'Table Users created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Users already exists.';
END
GO

-- =============================================
-- Table: Packages
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Packages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Packages] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(200) NOT NULL,
        [CountryId] UNIQUEIDENTIFIER NOT NULL,
        [Credits] INT NOT NULL,
        [Price] DECIMAL(18, 2) NOT NULL,
        [ExpiryDate] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Packages_Countries] FOREIGN KEY ([CountryId]) 
            REFERENCES [dbo].[Countries]([Id]) ON DELETE NO ACTION
    );
    PRINT 'Table Packages created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Packages already exists.';
END
GO

-- =============================================
-- Table: UserPackages
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPackages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserPackages] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [PackageId] UNIQUEIDENTIFIER NOT NULL,
        [RemainingCredits] INT NOT NULL,
        [PurchaseDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ExpiryDate] DATETIME2 NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_UserPackages_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserPackages_Packages] FOREIGN KEY ([PackageId]) 
            REFERENCES [dbo].[Packages]([Id]) ON DELETE NO ACTION
    );
    PRINT 'Table UserPackages created successfully.';
END
ELSE
BEGIN
    PRINT 'Table UserPackages already exists.';
END
GO

-- =============================================
-- Table: Classes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Classes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Classes] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(1000) NULL,
        [CountryId] UNIQUEIDENTIFIER NOT NULL,
        [StartTime] DATETIME2 NOT NULL,
        [EndTime] DATETIME2 NOT NULL,
        [RequiredCredits] INT NOT NULL,
        [MaxCapacity] INT NOT NULL,
        [CurrentBookings] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Classes_Countries] FOREIGN KEY ([CountryId]) 
            REFERENCES [dbo].[Countries]([Id]) ON DELETE NO ACTION
    );
    PRINT 'Table Classes created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Classes already exists.';
END
GO

-- =============================================
-- Table: Bookings
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Bookings] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [ClassId] UNIQUEIDENTIFIER NOT NULL,
        [UserPackageId] UNIQUEIDENTIFIER NOT NULL,
        [Status] INT NOT NULL DEFAULT 0, -- 0=Booked, 1=Cancelled, 2=Completed, 3=CheckedIn
        [CreditsUsed] INT NOT NULL,
        [IsCheckedIn] BIT NOT NULL DEFAULT 0,
        [CheckInTime] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Bookings_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Classes] FOREIGN KEY ([ClassId]) 
            REFERENCES [dbo].[Classes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_UserPackages] FOREIGN KEY ([UserPackageId]) 
            REFERENCES [dbo].[UserPackages]([Id]) ON DELETE NO ACTION
    );
    PRINT 'Table Bookings created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Bookings already exists.';
END
GO

-- =============================================
-- Table: Waitlists
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Waitlists]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Waitlists] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [ClassId] UNIQUEIDENTIFIER NOT NULL,
        [UserPackageId] UNIQUEIDENTIFIER NOT NULL,
        [CreditsReserved] INT NOT NULL,
        [Position] INT NOT NULL,
        [IsPromoted] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        CONSTRAINT [FK_Waitlists_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Waitlists_Classes] FOREIGN KEY ([ClassId]) 
            REFERENCES [dbo].[Classes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Waitlists_UserPackages] FOREIGN KEY ([UserPackageId]) 
            REFERENCES [dbo].[UserPackages]([Id]) ON DELETE NO ACTION
    );
    PRINT 'Table Waitlists created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Waitlists already exists.';
END
GO

-- =============================================
-- Step 3: Create Indexes for Performance
-- =============================================

-- Indexes for Countries (only if table has expected columns)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Countries_Code' AND object_id = OBJECT_ID('dbo.Countries'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Countries' AND COLUMN_NAME = 'Code')
BEGIN
    CREATE UNIQUE INDEX [IX_Countries_Code] ON [dbo].[Countries]([Code]);
    PRINT 'Index IX_Countries_Code created.';
END
GO

-- Indexes for Users (only if table has expected columns)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('dbo.Users'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users' AND COLUMN_NAME = 'Email')
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
    PRINT 'Index IX_Users_Email created.';
END
GO

-- Indexes for Bookings (only if table has expected columns)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookings_UserId' AND object_id = OBJECT_ID('dbo.Bookings'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'UserId')
BEGIN
    CREATE INDEX [IX_Bookings_UserId] ON [dbo].[Bookings]([UserId]);
    PRINT 'Index IX_Bookings_UserId created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookings_ClassId' AND object_id = OBJECT_ID('dbo.Bookings'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'ClassId')
BEGIN
    CREATE INDEX [IX_Bookings_ClassId] ON [dbo].[Bookings]([ClassId]);
    PRINT 'Index IX_Bookings_ClassId created.';
END
GO

-- Indexes for Waitlists (only if table has expected columns)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Waitlists_ClassId_Position' AND object_id = OBJECT_ID('dbo.Waitlists'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Waitlists' AND COLUMN_NAME = 'ClassId')
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Waitlists' AND COLUMN_NAME = 'Position')
BEGIN
    CREATE INDEX [IX_Waitlists_ClassId_Position] ON [dbo].[Waitlists]([ClassId], [Position]);
    PRINT 'Index IX_Waitlists_ClassId_Position created.';
END
GO

-- Indexes for UserPackages
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserPackages_UserId' AND object_id = OBJECT_ID('dbo.UserPackages'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'UserPackages' AND COLUMN_NAME = 'UserId')
BEGIN
    CREATE INDEX [IX_UserPackages_UserId] ON [dbo].[UserPackages]([UserId]);
    PRINT 'Index IX_UserPackages_UserId created.';
END
GO

-- Indexes for Classes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Classes_CountryId' AND object_id = OBJECT_ID('dbo.Classes'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Classes' AND COLUMN_NAME = 'CountryId')
BEGIN
    CREATE INDEX [IX_Classes_CountryId] ON [dbo].[Classes]([CountryId]);
    PRINT 'Index IX_Classes_CountryId created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Classes_StartTime' AND object_id = OBJECT_ID('dbo.Classes'))
   AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Classes' AND COLUMN_NAME = 'StartTime')
BEGIN
    CREATE INDEX [IX_Classes_StartTime] ON [dbo].[Classes]([StartTime]);
    PRINT 'Index IX_Classes_StartTime created.';
END
GO

-- =============================================
-- Step 4: Seed Reference Data (Essential Data)
-- =============================================

-- Seed Countries
IF NOT EXISTS (SELECT 1 FROM [dbo].[Countries])
BEGIN
    INSERT INTO [dbo].[Countries] ([Id], [Name], [Code], [CreatedAt])
    VALUES 
        (NEWID(), N'Singapore', N'SG', GETUTCDATE()),
        (NEWID(), N'Myanmar', N'MM', GETUTCDATE());
    
    PRINT 'Countries seeded: Singapore (SG), Myanmar (MM)';
END
ELSE
BEGIN
    PRINT 'Countries already exist. Skipping seed.';
END
GO

-- Seed Packages
IF NOT EXISTS (SELECT 1 FROM [dbo].[Packages])
BEGIN
    DECLARE @SingaporeId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[Countries] WHERE [Code] = 'SG');
    DECLARE @MyanmarId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[Countries] WHERE [Code] = 'MM');
    
    IF @SingaporeId IS NOT NULL AND @MyanmarId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Packages] ([Id], [Name], [CountryId], [Credits], [Price], [ExpiryDate], [IsActive], [CreatedAt])
        VALUES 
            (NEWID(), N'Basic Package SG', @SingaporeId, 5, 50.00, DATEADD(MONTH, 3, GETUTCDATE()), 1, GETUTCDATE()),
            (NEWID(), N'Premium Package SG', @SingaporeId, 10, 90.00, DATEADD(MONTH, 6, GETUTCDATE()), 1, GETUTCDATE()),
            (NEWID(), N'Basic Package MM', @MyanmarId, 5, 30.00, DATEADD(MONTH, 3, GETUTCDATE()), 1, GETUTCDATE());
        
        PRINT 'Packages seeded: Basic Package SG, Premium Package SG, Basic Package MM';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Countries not found. Cannot seed packages.';
    END
END
ELSE
BEGIN
    PRINT 'Packages already exist. Skipping seed.';
END
GO

PRINT '=============================================';
PRINT 'Database setup completed successfully!';
PRINT 'Database: BookingSystemDb';
PRINT 'Tables: Countries, Users, Packages, UserPackages, Classes, Bookings, Waitlists';
PRINT 'Reference Data: Countries (SG, MM) and Packages seeded';
PRINT '=============================================';
GO
