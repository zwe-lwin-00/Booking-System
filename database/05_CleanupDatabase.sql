-- =============================================
-- Cleanup Database Script
-- WARNING: This will delete all data!
-- Use only for development/testing
-- =============================================

USE [BookingSystemDb];
GO

PRINT 'WARNING: This script will delete all data from the database!';
PRINT 'Press Ctrl+C to cancel, or wait 5 seconds...';
WAITFOR DELAY '00:00:05';

-- Delete in reverse order of dependencies
DELETE FROM [dbo].[Waitlists];
PRINT 'Deleted all Waitlists.';

DELETE FROM [dbo].[Bookings];
PRINT 'Deleted all Bookings.';

DELETE FROM [dbo].[Classes];
PRINT 'Deleted all Classes.';

DELETE FROM [dbo].[UserPackages];
PRINT 'Deleted all UserPackages.';

DELETE FROM [dbo].[Packages];
PRINT 'Deleted all Packages.';

DELETE FROM [dbo].[Users];
PRINT 'Deleted all Users.';

DELETE FROM [dbo].[Countries];
PRINT 'Deleted all Countries.';

PRINT 'Database cleanup completed!';
GO
