# Booking System - Class/Package Booking API

A complete class booking system built with .NET 8.0, following Clean Architecture principles. This system allows users to purchase country-specific credit packages and book fitness classes using those credits.

## 🎯 Features

### User Module
- ✅ User registration with email verification
- ✅ Login with JWT Bearer token
- ✅ Email verification (mock service)
- ✅ Password reset functionality
- ✅ Change password
- ✅ Get user profile
- ✅ Update profile

### Package Module
- ✅ View available packages by country
- ✅ Purchase packages with mock payment
- ✅ View purchased packages with remaining credits
- ✅ Package expiry tracking
- ✅ Country-specific packages (e.g., "Basic Package SG - 5 credits")

### Schedule Module (Booking)
- ✅ View class schedules by country
- ✅ Book classes using country-specific packages
- ✅ **Concurrent booking prevention** using Redis distributed locks
- ✅ **4-hour cancellation rule**: Credits refunded if cancelled 4+ hours before class
- ✅ **Waitlist system**: FIFO queue when class is full
- ✅ **Automatic waitlist promotion**: When someone cancels, next waitlist user is promoted
- ✅ **Automatic waitlist refund**: Hangfire job runs hourly to refund waitlist users after class ends
- ✅ **Check-in functionality**: Users can check in when class time arrives
- ✅ **Overlap prevention**: Users cannot book overlapping class times

## 🏗️ Architecture

This project follows **Clean Architecture** (Onion Architecture) with clear separation of concerns:

- **BookingSystem.Domain**: Core business entities and interfaces (no dependencies)
- **BookingSystem.Application**: Business logic, DTOs, services, and validators
- **BookingSystem.Infrastructure**: Data access (EF Core), repositories, Redis, Hangfire
- **BookingSystem.API**: Web API controllers, authentication, and configuration
- **BookingSystem.UnitTests**: Unit tests for the application

## 🛠️ Technology Stack

### Framework & Architectures
- ✅ **.NET 8.0** (exceeds .NET 6 requirement)
- ✅ **SQL Server (MSSQL)** with Entity Framework Core 8.0
- ✅ **MVC** pattern with API Controllers
- ✅ **Entity Framework Core 8.0.11**
- ✅ **Redis** (StackExchange.Redis 2.10.1) for concurrent booking prevention
- ✅ **Hangfire 1.8.22** for scheduled jobs (waitlist refunds)

### Authentication
- ✅ **Basic Authorization**: Email verification required before login
- ✅ **Bearer Token**: JWT Bearer token authentication after login

### Documentation
- ✅ **Swagger UI** for API documentation (`/swagger`)
- ✅ **Database Design Diagram** (see `DATABASE_DESIGN.md`)

## 📁 Project Structure

```
BookingSystem/
├── src/
│   ├── BookingSystem.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── PackagesController.cs
│   │   │   ├── ClassesController.cs
│   │   │   ├── ScheduleController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── CountriesController.cs
│   │   │   └── HealthController.cs
│   │   ├── Middleware/
│   │   ├── Filters/
│   │   └── Extensions/
│   ├── BookingSystem.Application/
│   │   ├── DTOs/
│   │   ├── Services/
│   │   ├── Validators/
│   │   └── Common/
│   ├── BookingSystem.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Interfaces/
│   └── BookingSystem.Infrastructure/
│       ├── Persistence/
│       ├── Repositories/
│       └── Jobs/
└── tests/
    └── BookingSystem.UnitTests/
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (or LocalDB)
- Redis Server (for concurrent booking prevention)
- Visual Studio 2022 or VS Code

### Setup

1. **Update Connection Strings**

   Edit `src/BookingSystem.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookingSystemDb;Trusted_Connection=true;MultipleActiveResultSets=true",
       "Redis": "localhost:6379"
     },
     "Jwt": {
       "Key": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForSecurity!",
       "Issuer": "BookingSystem",
       "Audience": "BookingSystem"
     }
   }
   ```

2. **Start Redis Server**

   ```bash
   # Windows (if installed)
   redis-server
   
   # Or use Docker
   docker run -d -p 6379:6379 redis
   ```

3. **Run the Application**

   ```bash
   cd src/BookingSystem.API
   dotnet run
   ```

4. **Access Swagger UI**

   Navigate to `https://localhost:5001/swagger`

5. **Access Hangfire Dashboard**

   Navigate to `https://localhost:5001/hangfire`

### Database

The database will be automatically created on first run with seed data:
- Countries: Singapore (SG), Myanmar (MM)
- Sample packages for each country

For production, use migrations:
```bash
dotnet ef migrations add InitialCreate --project src/BookingSystem.Infrastructure --startup-project src/BookingSystem.API
dotnet ef database update --project src/BookingSystem.Infrastructure --startup-project src/BookingSystem.API
```

## 📚 API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register new user (email verification required)
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/verify-email` - Verify email with token
- `POST /api/auth/resend-verification` - Resend verification email
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with token
- `POST /api/auth/change-password` - Change password (requires auth)

### Packages (`/api/packages`)
- `GET /api/packages` - Get all available packages
- `GET /api/packages/{id}` - Get package by ID
- `GET /api/packages/country/{countryId}` - Get packages by country
- `POST /api/packages/purchase` - Purchase a package (requires auth)
- `GET /api/packages/my-packages` - Get user's purchased packages (requires auth)

### Classes (`/api/classes`)
- `GET /api/classes` - Get all upcoming classes
- `GET /api/classes/{id}` - Get class by ID
- `GET /api/classes/country/{countryId}` - Get classes by country

### Schedule/Booking (`/api/schedule`)
- `POST /api/schedule/book` - Book a class (requires auth)
- `POST /api/schedule/cancel/{bookingId}` - Cancel booking (requires auth)
- `POST /api/schedule/waitlist` - Add to waitlist (requires auth)
- `POST /api/schedule/check-in/{bookingId}` - Check in to class (requires auth)
- `GET /api/schedule/my-bookings` - Get user's bookings (requires auth)

### Users (`/api/users`)
- `GET /api/users/profile` - Get user profile (requires auth)
- `PUT /api/users/profile` - Update user profile (requires auth)

### Countries (`/api/countries`)
- `GET /api/countries` - Get all countries
- `GET /api/countries/{id}` - Get country by ID

### Health (`/api/health`)
- `GET /api/health` - Health check endpoint

## 🔐 Business Rules

### Package Rules
- Packages are country-specific (e.g., SG package can only book SG classes)
- Packages have expiry dates
- Credits are deducted when booking classes
- Credits are refunded on cancellation (if 4+ hours before class)

### Booking Rules
- Cannot book overlapping class times
- Must have sufficient credits in package
- Package country must match class country
- Concurrent bookings prevented using Redis locks

### Cancellation Rules
- **4+ hours before class**: Credits refunded to package
- **Within 4 hours**: No refund
- Cancellation triggers waitlist promotion (FIFO)

### Waitlist Rules
- Only available when class is full
- FIFO order (by Position)
- Automatically promoted when space becomes available
- Credits refunded after class ends if not promoted

### Check-in Rules
- Available 15 minutes before class starts
- Must be before class ends
- Cannot check in to cancelled bookings

## 🔧 Technical Implementation

### Concurrent Booking Prevention
Uses Redis distributed locks to prevent overbooking:
1. Acquire lock for class (10-second timeout)
2. Re-check capacity after acquiring lock (double-check pattern)
3. If not full: Deduct credits, create booking, increment count
4. Release lock atomically using Lua script

### Waitlist Refund Flow
Hangfire job runs hourly:
1. Finds all classes where `EndTime < DateTime.UtcNow`
2. For each ended class, finds non-promoted waitlist entries
3. Refunds `CreditsReserved` back to user's package
4. Marks waitlist entry as processed

### 4-Hour Refund Rule
1. Calculate: `hoursUntilClass = (StartTime - Now).TotalHours`
2. If `hoursUntilClass >= 4`: Refund credits
3. If `hoursUntilClass < 4`: No refund

## 🧪 Testing

Run unit tests:
```bash
dotnet test
```

## 📖 Documentation

- **Database Design**: See `DATABASE_DESIGN.md` for complete ERD
- **Technology Stack**: See `TECHNOLOGY_REQUIREMENTS_VERIFICATION.md` for detailed technology verification
- **Test Procedures**: See `TEST_PROCEDURE.md` for comprehensive testing guide
- **API Documentation**: Available at `/swagger` endpoint

## 🔄 Mock Services

### Email Service
- `SendVerifyEmail()` - Returns true (mock)
- `SendPasswordResetEmail()` - Returns true (mock)

### Payment Service
- `AddPaymentCard()` - Returns true (mock)
- `PaymentCharge()` - Returns true (mock)

**Note**: In production, integrate with real email service (SendGrid, AWS SES) and payment gateway (Stripe, PayPal).

## 📦 NuGet Packages

### API Project
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `BCrypt.Net-Next` Version 4.0.3
- `Swashbuckle.AspNetCore`

### Application Project
- `FluentValidation` Version 11.9.0
- `BCrypt.Net-Next` Version 4.0.3
- `System.IdentityModel.Tokens.Jwt` Version 8.2.1
- `Microsoft.IdentityModel.Tokens` Version 8.2.1
- `StackExchange.Redis` Version 2.10.1

### Infrastructure Project
- `Hangfire` Version 1.8.22
- `Hangfire.SqlServer` Version 1.8.22
- `StackExchange.Redis` Version 2.10.1
- `Microsoft.EntityFrameworkCore` Version 8.0.11
- `Microsoft.EntityFrameworkCore.SqlServer` Version 8.0.11
- `Microsoft.EntityFrameworkCore.Design` Version 8.0.11

## ✅ Requirements Coverage

All requirements from the test question are fully implemented:
- ✅ User Module (register, login, email verification, profile, password)
- ✅ Package Module (view, purchase, expired status, remaining credits)
- ✅ Schedule Module (view, book, cancel, waitlist, check-in, overlap, concurrent)
- ✅ All technology requirements (.NET 8, MSSQL, MVC, EF Core, Redis, Hangfire, JWT, Swagger)

## 📝 License

This project is for demonstration purposes.
