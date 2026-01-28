# Database Scripts

This folder contains SQL scripts for database setup, seeding, and maintenance.

## Scripts Overview

### 1. `01_CreateDatabase.sql`
Creates the `BookingSystemDb` database if it doesn't exist.

**Usage:**
```sql
-- Run in SQL Server Management Studio or sqlcmd
sqlcmd -S localhost -i 01_CreateDatabase.sql
```

### 2. `02_CreateTables.sql`
Creates all tables, foreign keys, and indexes for the booking system.

**Tables Created:**
- `Countries` - Country reference data
- `Users` - User accounts
- `Packages` - Credit packages available for purchase
- `UserPackages` - User's purchased packages
- `Classes` - Fitness class schedules
- `Bookings` - User class bookings
- `Waitlists` - Waitlist entries for full classes

**Indexes Created:**
- Unique indexes on `Countries.Code` and `Users.Email`
- Performance indexes on foreign keys and frequently queried columns

**Usage:**
```sql
sqlcmd -S localhost -d BookingSystemDb -i 02_CreateTables.sql
```

### 3. `03_SeedReferenceData.sql`
Seeds essential reference data required for the system to function.

**Data Seeded:**
- **Countries**: Singapore (SG), Myanmar (MM)
- **Packages**: 
  - Basic Package SG (5 credits, $50)
  - Premium Package SG (10 credits, $90)
  - Basic Package MM (5 credits, $30)

**Usage:**
```sql
sqlcmd -S localhost -d BookingSystemDb -i 03_SeedReferenceData.sql
```

### 4. `04_SeedTestData.sql`
Seeds test data for development and testing purposes.

**Data Seeded:**
- **Test Users**: 
  - test.user@example.com (verified)
  - john.doe@example.com (verified)
  - jane.smith@example.com (unverified)
- **Test User Packages**: Sample packages for test user
- **Test Classes**: 
  - Upcoming classes for both countries
  - Full class (for waitlist testing)
  - Past class (for waitlist refund testing)

**Usage:**
```sql
sqlcmd -S localhost -d BookingSystemDb -i 04_SeedTestData.sql
```

### 5. `05_CleanupDatabase.sql`
Deletes all data from all tables (keeps schema).

**WARNING:** This will delete all data!

**Usage:**
```sql
sqlcmd -S localhost -d BookingSystemDb -i 05_CleanupDatabase.sql
```

### 6. `06_DropDatabase.sql`
Completely drops the database.

**WARNING:** This will permanently delete the entire database!

**Usage:**
```sql
sqlcmd -S localhost -i 06_DropDatabase.sql
```

## Quick Setup Guide

### Option 1: Full Setup (Recommended for First Time)

```bash
# 1. Create database
sqlcmd -S localhost -i 01_CreateDatabase.sql

# 2. Create tables
sqlcmd -S localhost -d BookingSystemDb -i 02_CreateTables.sql

# 3. Seed reference data
sqlcmd -S localhost -d BookingSystemDb -i 03_SeedReferenceData.sql

# 4. Seed test data (optional)
sqlcmd -S localhost -d BookingSystemDb -i 04_SeedTestData.sql
```

### Option 2: Using SQL Server Management Studio (SSMS)

1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Open each script file in order (01 → 02 → 03 → 04)
4. Execute each script (F5)

### Option 3: Using Azure Data Studio

1. Open Azure Data Studio
2. Connect to your SQL Server
3. Open each script file
4. Run each script

## Script Execution Order

**For Initial Setup:**
```
01_CreateDatabase.sql
  ↓
02_CreateTables.sql
  ↓
03_SeedReferenceData.sql
  ↓
04_SeedTestData.sql (optional)
```

**For Cleanup:**
```
05_CleanupDatabase.sql (deletes data, keeps schema)
  OR
06_DropDatabase.sql (deletes everything)
```

## Connection String Examples

### LocalDB (Default)
```
Server=(localdb)\mssqllocaldb;Database=BookingSystemDb;Trusted_Connection=true;
```

### SQL Server Express
```
Server=localhost\SQLEXPRESS;Database=BookingSystemDb;Trusted_Connection=true;
```

### SQL Server (Named Instance)
```
Server=localhost\INSTANCENAME;Database=BookingSystemDb;Trusted_Connection=true;
```

### SQL Server with Authentication
```
Server=localhost;Database=BookingSystemDb;User Id=sa;Password=YourPassword;
```

## Verification Queries

### Check Database Exists
```sql
SELECT name FROM sys.databases WHERE name = 'BookingSystemDb';
```

### Check Tables Created
```sql
USE BookingSystemDb;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

### Check Reference Data
```sql
USE BookingSystemDb;
SELECT * FROM Countries;
SELECT * FROM Packages;
```

### Check Test Data
```sql
USE BookingSystemDb;
SELECT COUNT(*) as UserCount FROM Users;
SELECT COUNT(*) as ClassCount FROM Classes;
SELECT COUNT(*) as BookingCount FROM Bookings;
```

## Troubleshooting

### Error: "Database already exists"
- This is normal if database was previously created
- Script will skip creation and continue

### Error: "Table already exists"
- This is normal if tables were previously created
- Script will skip creation and continue

### Error: "Foreign key constraint"
- Make sure you run scripts in order
- Reference data (Countries) must exist before Packages
- Users must exist before UserPackages

### Error: "Invalid object name"
- Run `02_CreateTables.sql` first
- Check that you're connected to the correct database

## Notes

- All scripts are **idempotent** - safe to run multiple times
- Scripts check for existence before creating/inserting
- Timestamps use `GETUTCDATE()` for UTC time
- GUIDs are auto-generated using `NEWID()`
- Foreign keys use `ON DELETE NO ACTION` to prevent accidental deletions

## Production Considerations

For production environments:
1. Use EF Core Migrations instead of these scripts
2. Remove test data script (`04_SeedTestData.sql`)
3. Use proper authentication (not Trusted_Connection)
4. Backup database before running any scripts
5. Test scripts in staging environment first
