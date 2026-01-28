-- =============================================
-- Clean Test Data Script
-- WARNING: This will delete all test data!
-- Use only for development/testing
-- =============================================
-- This script deletes all data from the database
-- It preserves the schema (tables, indexes, constraints)
-- Run this script to reset the database to empty state
-- =============================================

USE [BookingSystemDb];
GO

PRINT '=============================================';
PRINT 'WARNING: This script will delete ALL data from the database!';
PRINT 'Tables will remain, but all records will be deleted.';
PRINT 'Press Ctrl+C to cancel, or wait 5 seconds...';
PRINT '=============================================';
WAITFOR DELAY '00:00:05';

-- Delete in reverse order of dependencies to avoid foreign key violations

-- Delete Waitlists (depends on Users, Classes, UserPackages)
DELETE FROM [dbo].[Waitlists];
PRINT 'Deleted all Waitlists.';

-- Delete Bookings (depends on Users, Classes, UserPackages)
DELETE FROM [dbo].[Bookings];
PRINT 'Deleted all Bookings.';

-- Delete Classes (depends on Countries)
DELETE FROM [dbo].[Classes];
PRINT 'Deleted all Classes.';

-- Delete UserPackages (depends on Users, Packages)
DELETE FROM [dbo].[UserPackages];
PRINT 'Deleted all UserPackages.';

-- Delete Packages (depends on Countries)
DELETE FROM [dbo].[Packages];
PRINT 'Deleted all Packages.';

-- Delete Users (no dependencies)
DELETE FROM [dbo].[Users];
PRINT 'Deleted all Users.';

-- Delete Countries (no dependencies, but referenced by Packages and Classes)
DELETE FROM [dbo].[Countries];
PRINT 'Deleted all Countries.';

PRINT '=============================================';
PRINT 'Database cleanup completed!';
PRINT 'All data has been deleted.';
PRINT 'Schema (tables, indexes, constraints) remains intact.';
PRINT 'Run 01_CreateDatabaseAndTables.sql to seed reference data again.';
PRINT '=============================================';
GO
