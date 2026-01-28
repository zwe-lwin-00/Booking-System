# DataSeeder Explanation

## What is DataSeeder?

**DataSeeder** is used to populate the database with **initial/reference data** that the application needs to function properly. It's **NOT mock/test data** - it's essential reference data.

## What Data Does It Seed?

### 1. Countries (Reference Data)
- **Singapore (SG)** - Required for the system to work
- **Myanmar (MM)** - Required for the system to work

**Why needed?**
- Users can only purchase packages for specific countries
- Classes are organized by country
- Packages are country-specific
- Without countries, the system cannot function

### 2. Packages (Initial Product Data)
- **Basic Package SG** - 5 credits, $50.00, expires in 3 months
- **Premium Package SG** - 10 credits, $90.00, expires in 6 months  
- **Basic Package MM** - 5 credits, $30.00, expires in 3 months

**Why needed?**
- Users need packages to purchase before they can book classes
- These are the initial products available in the system
- Without packages, users cannot make purchases

## When Does It Run?

**Location**: `src/BookingSystem.API/Program.cs:75`

```csharp
// Only runs in Development environment
if (app.Environment.IsDevelopment())
{
    // ... database setup ...
    await DataSeeder.SeedInitialDataAsync(context);
}
```

**Conditions**:
- ✅ Only runs in **Development** environment
- ✅ Runs **automatically** on first application startup
- ✅ Only seeds if data doesn't already exist (idempotent)
- ✅ Runs after database and tables are created

## How Does It Work?

1. **Check if data exists** - Prevents duplicate seeding
2. **Seed Countries first** - Creates Singapore and Myanmar
3. **Seed Packages second** - Creates packages linked to countries
4. **Error handling** - Gracefully handles missing tables or connection issues

## Is This Mock Data?

**NO** - This is **initial/reference data**, not mock/test data.

### Difference:

| Type | Purpose | Example |
|------|---------|---------|
| **Reference Data** | Required for system to function | Countries, Package types |
| **Mock Data** | For testing/development | Fake users, test bookings |
| **Seed Data** | Initial data to start using the system | Initial countries and packages |

**DataSeeder provides:**
- ✅ **Reference data** (Countries - required for system)
- ✅ **Initial product data** (Packages - needed for users to purchase)

**It does NOT provide:**
- ❌ Test users
- ❌ Mock bookings
- ❌ Sample classes
- ❌ Fake transactions

## Why Is It Needed?

Without DataSeeder, when you first run the application:
- ❌ No countries exist → Users can't see any countries
- ❌ No packages exist → Users can't purchase anything
- ❌ System cannot function → No data to work with

With DataSeeder:
- ✅ Countries are available → Users can see Singapore and Myanmar
- ✅ Packages are available → Users can purchase packages
- ✅ System is ready to use → Can start testing immediately

## Can You Remove It?

**For Development**: Not recommended - you need initial data to test the system

**For Production**: 
- You can remove it and seed data manually via SQL scripts
- Or use EF Core Migrations with seed data
- Or use a separate data seeding tool

## Summary

**DataSeeder = Initial Reference Data Seeder**

- **Purpose**: Populate database with essential reference data (Countries, Packages)
- **When**: Runs automatically on first startup (Development only)
- **What**: Creates Singapore, Myanmar, and 3 initial packages
- **Why**: System needs this data to function
- **Type**: Reference/Initial data (NOT mock/test data)
