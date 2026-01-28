-- =============================================
-- Master Script - Run All Database Setup Scripts
-- This script runs all setup scripts in order
-- =============================================

PRINT '========================================';
PRINT 'Booking System Database Setup';
PRINT '========================================';
PRINT '';

-- Step 1: Create Database
PRINT 'Step 1: Creating database...';
:r 01_CreateDatabase.sql
GO

-- Step 2: Create Tables
PRINT '';
PRINT 'Step 2: Creating tables...';
:r 02_CreateTables.sql
GO

-- Step 3: Seed Reference Data
PRINT '';
PRINT 'Step 3: Seeding reference data...';
:r 03_SeedReferenceData.sql
GO

-- Step 4: Seed Test Data (Optional - comment out for production)
PRINT '';
PRINT 'Step 4: Seeding test data...';
:r 04_SeedTestData.sql
GO

PRINT '';
PRINT '========================================';
PRINT 'Database setup completed successfully!';
PRINT '========================================';
GO
