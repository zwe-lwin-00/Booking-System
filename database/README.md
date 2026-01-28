# Database Scripts

This folder contains consolidated SQL scripts for database setup, testing, and cleanup.

## Scripts Overview

### 1. `01_CreateDatabaseAndTables.sql` ⭐ **START HERE**
**All-in-one database setup script** - Creates database, tables, indexes, and reference data.

**What it does:**
- Creates `BookingSystemDb` database
- Creates all 7 tables (Countries, Users, Packages, UserPackages, Classes, Bookings, Waitlists)
- Creates all performance indexes
- Seeds reference data:
  - **Countries**: Singapore (SG), Myanmar (MM)
  - **Packages**: Basic Package SG, Premium Package SG, Basic Package MM

**Usage:**
```bash
sqlcmd -S localhost -i database/01_CreateDatabaseAndTables.sql
```

**Or in SQL Server Management Studio:**
1. Open `01_CreateDatabaseAndTables.sql`
2. Execute (F5)

---

### 2. `02_InsertTestData.sql`
**Test data for development and testing** - Inserts test users, packages, and classes.

**What it does:**
- Creates test users:
  - `test.user@example.com` (verified)
  - `john.doe@example.com` (verified)
  - `jane.smith@example.com` (unverified)
- Creates test user packages for test.user@example.com
- Creates test classes:
  - Upcoming classes for SG and MM
  - Full class (for waitlist testing)
  - Past class (for waitlist refund testing)

**Usage:**
```bash
sqlcmd -S localhost -d BookingSystemDb -i database/02_InsertTestData.sql
```

**Note:** Run this **after** `01_CreateDatabaseAndTables.sql`

---

### 3. `03_CleanTestData.sql`
**Clean all data** - Deletes all data from all tables (preserves schema).

**WARNING:** This will delete **ALL** data from the database!

**What it does:**
- Deletes all records from all tables
- Preserves schema (tables, indexes, constraints remain)
- Includes 5-second warning delay

**Usage:**
```bash
sqlcmd -S localhost -d BookingSystemDb -i database/03_CleanTestData.sql
```

**Note:** After cleanup, run `01_CreateDatabaseAndTables.sql` again to restore reference data.

---

## Quick Setup Guide

### First Time Setup

```bash
# Step 1: Create database, tables, and reference data
sqlcmd -S localhost -i database/01_CreateDatabaseAndTables.sql

# Step 2: Insert test data (optional, for development)
sqlcmd -S localhost -d BookingSystemDb -i database/02_InsertTestData.sql
```

### Reset Database (Clean and Start Fresh)

```bash
# Step 1: Clean all data
sqlcmd -S localhost -d BookingSystemDb -i database/03_CleanTestData.sql

# Step 2: Restore reference data
sqlcmd -S localhost -d BookingSystemDb -i database/01_CreateDatabaseAndTables.sql

# Step 3: Insert test data (optional)
sqlcmd -S localhost -d BookingSystemDb -i database/02_InsertTestData.sql
```

### Using SQL Server Management Studio (SSMS)

1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Open `01_CreateDatabaseAndTables.sql` → Execute (F5)
4. (Optional) Open `02_InsertTestData.sql` → Execute (F5)

---

## Script Execution Order

**For Initial Setup:**
```
01_CreateDatabaseAndTables.sql  (creates everything)
  ↓
02_InsertTestData.sql            (optional - adds test data)
```

**For Cleanup:**
```
03_CleanTestData.sql             (deletes all data, keeps schema)
  ↓
01_CreateDatabaseAndTables.sql   (restores reference data)
```

---

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

---

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

---

## Troubleshooting

### Error: "Database already exists"
- ✅ **Normal** - Script will skip creation and continue
- Database will be used if it already exists

### Error: "Table already exists"
- ✅ **Normal** - Script will skip creation and continue
- Tables will be used if they already exist

### Error: "Column name 'ClassId' does not exist"
- This happens if existing tables have different schema (e.g., from EF migrations)
- Script now includes column-existence checks to prevent this error
- Indexes will only be created if columns exist

### Error: "Foreign key constraint"
- Make sure you run `01_CreateDatabaseAndTables.sql` first
- Reference data (Countries) must exist before Packages

### Error: "Invalid object name"
- Run `01_CreateDatabaseAndTables.sql` first
- Check that you're connected to the correct database

---

## Notes

- ✅ All scripts are **idempotent** - safe to run multiple times
- ✅ Scripts check for existence before creating/inserting
- ✅ Index creation includes column-existence checks for compatibility
- ✅ Timestamps use `GETUTCDATE()` for UTC time
- ✅ GUIDs are auto-generated using `NEWID()`
- ✅ Foreign keys use `ON DELETE NO ACTION` to prevent accidental deletions

---

## Production Considerations

For production environments:
1. Use EF Core Migrations instead of these scripts
2. Do **NOT** run `02_InsertTestData.sql` in production
3. Use proper authentication (not Trusted_Connection)
4. Backup database before running any scripts
5. Test scripts in staging environment first
6. Review and customize reference data in `01_CreateDatabaseAndTables.sql`
