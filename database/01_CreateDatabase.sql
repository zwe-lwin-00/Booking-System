-- =============================================
-- Create Database Script
-- Booking System Database
-- =============================================

-- Create Database
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
