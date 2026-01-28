-- =============================================
-- Seed Reference Data Script
-- Initial data required for system to function
-- =============================================

USE [BookingSystemDb];
GO

-- =============================================
-- Seed Countries
-- =============================================
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

-- =============================================
-- Seed Packages
-- =============================================
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

PRINT 'Reference data seeding completed!';
GO
