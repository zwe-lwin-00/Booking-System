# Database Design Diagram

## Entity Relationship Diagram (ERD)

```
┌─────────────────────────────────────────────────────────────┐
│                         Country                             │
├─────────────────────────────────────────────────────────────┤
│ Id (PK)                    UNIQUEIDENTIFIER                │
│ Name                       NVARCHAR(100)                    │
│ Code (Unique)              NVARCHAR(10)                    │
│ CreatedAt                  DATETIME2                        │
│ UpdatedAt                  DATETIME2                        │
└────────────┬────────────────────────────────────────────────┘
             │
             │ 1
             │
             │ *
        ┌────┴────┐
        │         │
        │         │
┌───────▼────┐ ┌──▼──────────────────────────────────────────┐
│  Package   │ │              Class                          │
├────────────┤ ├──────────────────────────────────────────────┤
│ Id (PK)    │ │ Id (PK)                    UNIQUEIDENTIFIER│
│ Name       │ │ Name                       NVARCHAR(200)     │
│ CountryId  │ │ Description                NVARCHAR(1000)    │
│ (FK)       │ │ CountryId (FK)             UNIQUEIDENTIFIER │
│ Credits    │ │ StartTime                  DATETIME2          │
│ Price      │ │ EndTime                    DATETIME2          │
│ ExpiryDate │ │ RequiredCredits            INT               │
│ IsActive   │ │ MaxCapacity                INT               │
│ CreatedAt  │ │ CurrentBookings            INT               │
│ UpdatedAt  │ │ CreatedAt                  DATETIME2          │
└───────┬────┘ │ UpdatedAt                  DATETIME2          │
        │      └────┬──────────────────────────────────────────┘
        │ 1         │ *
        │           │
        │ *         │
┌───────▼───────────▼────┐
│      UserPackage        │
├─────────────────────────┤
│ Id (PK)                 │
│ UserId (FK)             │
│ PackageId (FK)          │
│ RemainingCredits        │
│ PurchaseDate            │
│ ExpiryDate              │
│ CreatedAt               │
│ UpdatedAt               │
└───────┬──────────────────┘
        │
        │ *
        │
        │
┌───────▼──────────────────────────────────────────────────────┐
│                         User                                 │
├──────────────────────────────────────────────────────────────┤
│ Id (PK)                    UNIQUEIDENTIFIER                  │
│ FirstName                  NVARCHAR(100)                     │
│ LastName                   NVARCHAR(100)                     │
│ Email (Unique)             NVARCHAR(255)                    │
│ PhoneNumber                NVARCHAR(20)                      │
│ PasswordHash               NVARCHAR(255)                     │
│ IsEmailVerified            BIT                              │
│ EmailVerificationToken     NVARCHAR(255)                     │
│ EmailVerificationTokenExpiry DATETIME2                       │
│ PasswordResetToken         NVARCHAR(255)                      │
│ PasswordResetTokenExpiry   DATETIME2                         │
│ CreatedAt                  DATETIME2                         │
│ UpdatedAt                  DATETIME2                         │
└───────┬──────────────────────────────────────────────────────┘
        │
        │ 1
        │
        │ *
┌───────▼──────────────────────────────────────────────────────┐
│                        Booking                               │
├──────────────────────────────────────────────────────────────┤
│ Id (PK)                    UNIQUEIDENTIFIER                  │
│ UserId (FK)                UNIQUEIDENTIFIER                  │
│ ClassId (FK)                UNIQUEIDENTIFIER                  │
│ UserPackageId (FK)          UNIQUEIDENTIFIER                 │
│ Status                     INT (0=Booked, 1=Cancelled,       │
│                             2=Completed, 3=CheckedIn)         │
│ CreditsUsed                INT                               │
│ IsCheckedIn                BIT                               │
│ CheckInTime                DATETIME2                         │
│ CreatedAt                  DATETIME2                         │
│ UpdatedAt                  DATETIME2                         │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                       Waitlist                               │
├──────────────────────────────────────────────────────────────┤
│ Id (PK)                    UNIQUEIDENTIFIER                  │
│ UserId (FK)                UNIQUEIDENTIFIER                  │
│ ClassId (FK)                UNIQUEIDENTIFIER                  │
│ UserPackageId (FK)          UNIQUEIDENTIFIER                 │
│ CreditsReserved            INT                               │
│ Position                   INT (FIFO order)                  │
│ IsPromoted                 BIT                               │
│ CreatedAt                  DATETIME2                         │
│ UpdatedAt                  DATETIME2                         │
└──────────────────────────────────────────────────────────────┘
```

## Relationships

### 1. Country → Package (One-to-Many)
- **Relationship**: One Country can have many Packages
- **Foreign Key**: `Packages.CountryId` → `Countries.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Packages are country-specific (e.g., "Basic Package SG" for Singapore)

### 2. Country → Class (One-to-Many)
- **Relationship**: One Country can have many Classes
- **Foreign Key**: `Classes.CountryId` → `Countries.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Classes are organized by country

### 3. User → UserPackage (One-to-Many)
- **Relationship**: One User can have many UserPackages
- **Foreign Key**: `UserPackages.UserId` → `Users.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Users can purchase multiple packages

### 4. Package → UserPackage (One-to-Many)
- **Relationship**: One Package can be purchased by many Users
- **Foreign Key**: `UserPackages.PackageId` → `Packages.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Multiple users can purchase the same package

### 5. User → Booking (One-to-Many)
- **Relationship**: One User can have many Bookings
- **Foreign Key**: `Bookings.UserId` → `Users.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Users can book multiple classes

### 6. Class → Booking (One-to-Many)
- **Relationship**: One Class can have many Bookings
- **Foreign Key**: `Bookings.ClassId` → `Classes.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Multiple users can book the same class (up to MaxCapacity)

### 7. UserPackage → Booking (One-to-Many)
- **Relationship**: One UserPackage can be used for many Bookings
- **Foreign Key**: `Bookings.UserPackageId` → `UserPackages.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Users can use the same package for multiple bookings

### 8. User → Waitlist (One-to-Many)
- **Relationship**: One User can have many Waitlist entries
- **Foreign Key**: `Waitlists.UserId` → `Users.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Users can join multiple waitlists

### 9. Class → Waitlist (One-to-Many)
- **Relationship**: One Class can have many Waitlist entries
- **Foreign Key**: `Waitlists.ClassId` → `Classes.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Full classes can have multiple users in waitlist

### 10. UserPackage → Waitlist (One-to-Many)
- **Relationship**: One UserPackage can be used for many Waitlist entries
- **Foreign Key**: `Waitlists.UserPackageId` → `UserPackages.Id`
- **Constraint**: `ON DELETE NO ACTION`
- **Business Rule**: Users can use packages for waitlist entries

## Key Constraints

### Primary Keys
- All tables use `UNIQUEIDENTIFIER` (GUID) as Primary Key
- Primary Key fields: `Id` in all tables

### Unique Constraints
- `Countries.Code` - Unique country code (e.g., "SG", "MM")
- `Users.Email` - Unique email address for each user

### Foreign Key Constraints
- All foreign keys use `ON DELETE NO ACTION` to prevent accidental data loss
- Foreign keys ensure referential integrity

## Indexes

### Performance Indexes
- `IX_Countries_Code` - Unique index on `Countries.Code`
- `IX_Users_Email` - Unique index on `Users.Email`
- `IX_Bookings_UserId` - Index on `Bookings.UserId` for user booking queries
- `IX_Bookings_ClassId` - Index on `Bookings.ClassId` for class booking queries
- `IX_Waitlists_ClassId_Position` - Composite index on `Waitlists.ClassId, Position` for FIFO waitlist queries
- `IX_UserPackages_UserId` - Index on `UserPackages.UserId` for user package queries
- `IX_Classes_CountryId` - Index on `Classes.CountryId` for country-based class queries
- `IX_Classes_StartTime` - Index on `Classes.StartTime` for time-based queries

## Business Rules Enforced by Schema

1. **Country-Specific Packages**: Packages must belong to a country (`Packages.CountryId` FK)
2. **Country-Specific Classes**: Classes must belong to a country (`Classes.CountryId` FK)
3. **Package Purchase Tracking**: UserPackages link Users to Packages with credit tracking
4. **Booking Requirements**: Bookings require User, Class, and UserPackage (all FKs)
5. **Waitlist FIFO**: Waitlist uses `Position` field for FIFO ordering
6. **Credit Management**: 
   - `UserPackages.RemainingCredits` tracks available credits
   - `Bookings.CreditsUsed` records credits deducted
   - `Waitlists.CreditsReserved` reserves credits for waitlist
7. **Capacity Management**: 
   - `Classes.MaxCapacity` sets maximum bookings
   - `Classes.CurrentBookings` tracks current count
   - `Classes.IsFull` computed property prevents overbooking
8. **Email Verification**: `Users.IsEmailVerified` enforces email verification before login
9. **Booking Status**: `Bookings.Status` enum tracks booking lifecycle (Booked, Cancelled, Completed, CheckedIn)
10. **Check-in Tracking**: `Bookings.IsCheckedIn` and `Bookings.CheckInTime` track user check-ins

## Data Types Summary

| Field Type | SQL Type | Purpose |
|------------|----------|---------|
| Primary Keys | `UNIQUEIDENTIFIER` | GUID for distributed systems |
| Names/Text | `NVARCHAR(100-255)` | Unicode text fields |
| Email | `NVARCHAR(255)` | Email addresses |
| Phone | `NVARCHAR(20)` | Phone numbers |
| Credits | `INT` | Credit counts |
| Price | `DECIMAL(18,2)` | Monetary values |
| Dates/Times | `DATETIME2` | UTC timestamps |
| Booleans | `BIT` | True/False flags |
| Status | `INT` | Enum values (BookingStatus) |
| Tokens | `NVARCHAR(255)` | Verification/reset tokens |

## Entity Summary

| Entity | Purpose | Key Fields |
|--------|---------|------------|
| **Country** | Reference data for countries | Code (unique), Name |
| **User** | User accounts and authentication | Email (unique), PasswordHash, IsEmailVerified |
| **Package** | Available credit packages for purchase | CountryId, Credits, Price, ExpiryDate |
| **UserPackage** | User's purchased packages | UserId, PackageId, RemainingCredits, ExpiryDate |
| **Class** | Fitness class schedules | CountryId, StartTime, EndTime, RequiredCredits, MaxCapacity |
| **Booking** | User class bookings | UserId, ClassId, UserPackageId, Status, CreditsUsed |
| **Waitlist** | Waitlist entries for full classes | UserId, ClassId, UserPackageId, Position, IsPromoted |
