-- =============================================
-- Drop Database Script
-- WARNING: This will completely remove the database!
-- Use only for development/testing
-- =============================================

USE [master];
GO

PRINT 'WARNING: This script will completely drop the BookingSystemDb database!';
PRINT 'Press Ctrl+C to cancel, or wait 5 seconds...';
WAITFOR DELAY '00:00:05';

-- Close existing connections
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'BookingSystemDb')
BEGIN
    ALTER DATABASE [BookingSystemDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [BookingSystemDb];
    PRINT 'Database BookingSystemDb dropped successfully.';
END
ELSE
BEGIN
    PRINT 'Database BookingSystemDb does not exist.';
END
GO
