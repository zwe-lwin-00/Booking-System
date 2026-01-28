# Database Design Diagram

## Entity Relationship Diagram

```
┌─────────────────┐
│     Country     │
├─────────────────┤
│ Id (PK)         │
│ Name            │
│ Code (Unique)   │
│ CreatedAt       │
│ UpdatedAt       │
└────────┬────────┘
         │
         │ 1
         │
         │ *
    ┌────┴────┐
    │         │
    │         │
┌───▼────┐ ┌──▼────────┐
│Package │ │  Class    │
├────────┤ ├───────────┤
│Id (PK) │ │Id (PK)    │
│Name    │ │Name       │
│Country │ │Description│
│Id (FK) │ │CountryId  │
│Credits │ │(FK)       │
│Price   │ │StartTime  │
│Expiry  │ │EndTime    │
│Date    │ │Required   │
│IsActive│ │Credits    │
│Created │ │MaxCapacity│
│At      │ │Current    │
│Updated │ │Bookings   │
│At      │ │CreatedAt  │
└───┬────┘ │UpdatedAt │
    │      └────┬──────┘
    │ 1         │ *
    │           │
    │ *         │
┌───▼───────────▼────┐
│   UserPackage       │
├─────────────────────┤
│ Id (PK)             │
│ UserId (FK)         │
│ PackageId (FK)      │
│ RemainingCredits    │
│ PurchaseDate        │
│ ExpiryDate          │
│ CreatedAt           │
│ UpdatedAt           │
└─────────────────────┘

┌─────────────────┐
│      User       │
├─────────────────┤
│ Id (PK)         │
│ FirstName       │
│ LastName        │
│ Email (Unique)  │
│ PhoneNumber     │
│ PasswordHash    │
│ IsEmailVerified │
│ EmailVerifToken │
│ EmailVerifExpiry│
│ PasswordResetTok│
│ PasswordResetExp│
│ CreatedAt       │
│ UpdatedAt       │
└────────┬────────┘
         │
         │ 1
         │
         │ *
    ┌────┴────┐
    │         │
┌───▼────┐ ┌──▼────────┐
│Booking │ │ Waitlist  │
├────────┤ ├───────────┤
│Id (PK) │ │Id (PK)    │
│UserId  │ │UserId (FK)│
│(FK)    │ │ClassId    │
│ClassId │ │(FK)       │
│(FK)    │ │UserPackage│
│UserPack│ │Id (FK)    │
│ageId   │ │Credits    │
│(FK)    │ │Reserved   │
│Status  │ │Position   │
│Credits │ │IsPromoted │
│Used    │ │CreatedAt  │
│IsCheck │ │UpdatedAt  │
│edIn    │ └───────────┘
│CheckIn │
│Time    │
│Created │
│At      │
│Updated │
│At      │
└────────┘
```

## Relationships

1. **Country → Package** (1 to Many)
   - One country can have many packages
   - Package.CountryId → Country.Id

2. **Country → Class** (1 to Many)
   - One country can have many classes
   - Class.CountryId → Country.Id

3. **User → UserPackage** (1 to Many)
   - One user can have many packages
   - UserPackage.UserId → User.Id

4. **Package → UserPackage** (1 to Many)
   - One package can be purchased by many users
   - UserPackage.PackageId → Package.Id

5. **User → Booking** (1 to Many)
   - One user can have many bookings
   - Booking.UserId → User.Id

6. **Class → Booking** (1 to Many)
   - One class can have many bookings
   - Booking.ClassId → Class.Id

7. **UserPackage → Booking** (1 to Many)
   - One user package can be used for many bookings
   - Booking.UserPackageId → UserPackage.Id

8. **User → Waitlist** (1 to Many)
   - One user can be on many waitlists
   - Waitlist.UserId → User.Id

9. **Class → Waitlist** (1 to Many)
   - One class can have many waitlist entries
   - Waitlist.ClassId → Class.Id

10. **UserPackage → Waitlist** (1 to Many)
    - One user package can be used for many waitlists
    - Waitlist.UserPackageId → UserPackage.Id

## Key Constraints

- **Country.Code**: Unique constraint
- **User.Email**: Unique constraint
- **Booking**: Cannot overlap for same user (enforced in business logic)
- **Waitlist**: FIFO order by Position
- **UserPackage**: Credits cannot go negative (enforced in business logic)
- **Class**: CurrentBookings cannot exceed MaxCapacity (enforced in business logic)

## Indexes

- Country.Code (Unique Index)
- User.Email (Unique Index)
- Booking.UserId (Index for user bookings query)
- Booking.ClassId (Index for class bookings query)
- Waitlist.ClassId + Position (Index for FIFO waitlist queries)
- UserPackage.UserId (Index for user packages query)
