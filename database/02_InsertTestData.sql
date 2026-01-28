-- =============================================
-- Insert Test Data Script
-- Test data for development and testing purposes
-- =============================================
-- Run this script after 01_CreateDatabaseAndTables.sql
-- This script inserts test users, packages, and classes for testing
-- =============================================

USE [BookingSystemDb];
GO

-- =============================================
-- Seed Test Users
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'test.user@example.com')
BEGIN
    -- Test User 1: Verified user (can login)
    -- Password: Password123! (BCrypt hash)
    INSERT INTO [dbo].[Users] ([Id], [FirstName], [LastName], [Email], [PhoneNumber], [PasswordHash], [IsEmailVerified], [CreatedAt])
    VALUES 
        (NEWID(), N'Test', N'User', N'test.user@example.com', N'+1234567890', 
         '$2a$11$KIXxXxXxXxXxXxXxXxXxOeXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXx', 1, GETUTCDATE()),
        (NEWID(), N'John', N'Doe', N'john.doe@example.com', N'+1234567891', 
         '$2a$11$KIXxXxXxXxXxXxXxXxXxOeXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXx', 1, GETUTCDATE()),
        (NEWID(), N'Jane', N'Smith', N'jane.smith@example.com', N'+1234567892', 
         '$2a$11$KIXxXxXxXxXxXxXxXxXxOeXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXx', 0, GETUTCDATE());
    
    PRINT 'Test users created:';
    PRINT '  - test.user@example.com (verified)';
    PRINT '  - john.doe@example.com (verified)';
    PRINT '  - jane.smith@example.com (unverified)';
    PRINT 'Note: Password hash is placeholder. Use actual BCrypt hash in production.';
    PRINT 'Default password for test: Password123!';
END
ELSE
BEGIN
    PRINT 'Test users already exist. Skipping.';
END
GO

-- =============================================
-- Seed Test User Packages
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[UserPackages] WHERE [RemainingCredits] = 5 AND [PurchaseDate] > DATEADD(DAY, -1, GETUTCDATE()))
BEGIN
    DECLARE @TestUserId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Users] WHERE [Email] = 'test.user@example.com');
    DECLARE @SingaporePackageId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Packages] WHERE [Name] = 'Basic Package SG');
    DECLARE @MyanmarPackageId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[Packages] WHERE [Name] = 'Basic Package MM');
    
    IF @TestUserId IS NOT NULL AND @SingaporePackageId IS NOT NULL AND @MyanmarPackageId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[UserPackages] ([Id], [UserId], [PackageId], [RemainingCredits], [PurchaseDate], [ExpiryDate], [CreatedAt])
        VALUES 
            (NEWID(), @TestUserId, @SingaporePackageId, 5, GETUTCDATE(), DATEADD(MONTH, 3, GETUTCDATE()), GETUTCDATE()),
            (NEWID(), @TestUserId, @MyanmarPackageId, 5, GETUTCDATE(), DATEADD(MONTH, 3, GETUTCDATE()), GETUTCDATE());
        
        PRINT 'Test user packages created for test.user@example.com:';
        PRINT '  - Basic Package SG (5 credits)';
        PRINT '  - Basic Package MM (5 credits)';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Test user or packages not found. Cannot create test user packages.';
    END
END
ELSE
BEGIN
    PRINT 'Test user packages already exist. Skipping.';
END
GO

-- =============================================
-- Seed Test Classes
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Classes] WHERE [Name] LIKE '%Test%' OR [Name] LIKE '%Yoga%')
BEGIN
    DECLARE @SGCountryId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[Countries] WHERE [Code] = 'SG');
    DECLARE @MMCountryId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[Countries] WHERE [Code] = 'MM');
    
    IF @SGCountryId IS NOT NULL AND @MMCountryId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Classes] ([Id], [Name], [Description], [CountryId], [StartTime], [EndTime], [RequiredCredits], [MaxCapacity], [CurrentBookings], [CreatedAt])
        VALUES 
            -- Upcoming classes (SG)
            (NEWID(), N'1 hr Yoga Class (SG)', N'Relaxing yoga session', @SGCountryId, 
             DATEADD(HOUR, 5, GETUTCDATE()), DATEADD(HOUR, 6, GETUTCDATE()), 1, 10, 0, GETUTCDATE()),
            (NEWID(), N'1 hr Pilates Class (SG)', N'Core strengthening', @SGCountryId, 
             DATEADD(DAY, 1, GETUTCDATE()), DATEADD(DAY, 1, DATEADD(HOUR, 1, GETUTCDATE())), 2, 5, 0, GETUTCDATE()),
            (NEWID(), N'1 hr Zumba Class (SG)', N'Dance fitness', @SGCountryId, 
             DATEADD(DAY, 2, GETUTCDATE()), DATEADD(DAY, 2, DATEADD(HOUR, 1, GETUTCDATE())), 1, 15, 0, GETUTCDATE()),
            
            -- Upcoming classes (MM)
            (NEWID(), N'1 hr Yoga Class (MM)', N'Relaxing yoga session', @MMCountryId, 
             DATEADD(HOUR, 6, GETUTCDATE()), DATEADD(HOUR, 7, GETUTCDATE()), 1, 8, 0, GETUTCDATE()),
            (NEWID(), N'1 hr Meditation Class (MM)', N'Mindfulness meditation', @MMCountryId, 
             DATEADD(DAY, 1, GETUTCDATE()), DATEADD(DAY, 1, DATEADD(HOUR, 1, GETUTCDATE())), 1, 12, 0, GETUTCDATE()),
            
            -- Full class (for waitlist testing)
            (NEWID(), N'Full Class Test (SG)', N'Class at full capacity', @SGCountryId, 
             DATEADD(DAY, 3, GETUTCDATE()), DATEADD(DAY, 3, DATEADD(HOUR, 1, GETUTCDATE())), 1, 5, 5, GETUTCDATE()),
            
            -- Past class (for waitlist refund testing)
            (NEWID(), N'Past Class Test (SG)', N'Ended class for refund testing', @SGCountryId, 
             DATEADD(HOUR, -2, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE()), 1, 10, 8, GETUTCDATE());
        
        PRINT 'Test classes created:';
        PRINT '  - Upcoming classes for SG and MM';
        PRINT '  - Full class (for waitlist testing)';
        PRINT '  - Past class (for waitlist refund testing)';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Countries not found. Cannot create test classes.';
    END
END
ELSE
BEGIN
    PRINT 'Test classes already exist. Skipping.';
END
GO

PRINT '=============================================';
PRINT 'Test data seeding completed!';
PRINT 'Test Users: test.user@example.com, john.doe@example.com, jane.smith@example.com';
PRINT 'Test Packages: Created for test.user@example.com';
PRINT 'Test Classes: Various upcoming and test classes created';
PRINT '=============================================';
GO
