# Booking System - Class/Package Booking API

A complete class booking system built with .NET 8.0, following Clean Architecture principles. This system allows users to purchase country-specific credit packages and book fitness classes using those credits.

## Architecture

This project follows **Clean Architecture** (Onion Architecture) with clear separation of concerns:

- **BookingSystem.Domain**: Core business entities and interfaces (no dependencies)
- **BookingSystem.Application**: Business logic, DTOs, services, and validators
- **BookingSystem.Infrastructure**: Data access (EF Core), repositories, Redis, Hangfire
- **BookingSystem.API**: Web API controllers, authentication, and configuration
- **BookingSystem.UnitTests**: Unit tests for the application

## Technology Stack

- **.NET 8.0**
- **Entity Framework Core 8.0** with SQL Server
- **Redis** (StackExchange.Redis) for concurrent booking prevention
- **Hangfire** for scheduled jobs (waitlist refunds)
- **JWT Bearer Authentication**
- **BCrypt** for password hashing
- **FluentValidation** for input validation
- **Swagger/OpenAPI** for API documentation
- **xUnit** for testing

## Features Implemented

### ✅ User Module
- User registration with email verification
- Login with JWT Bearer token
- Email verification (mock service)
- Password reset functionality
- Change password
- Get user profile
- Update profile

### ✅ Package Module
- View available packages by country
- Purchase packages with mock payment
- View purchased packages with remaining credits
- Package expiry tracking
- Country-specific packages (e.g., "Basic Package SG - 5 credits")

### ✅ Schedule/Booking Module
- View class schedules by country
- Book classes using country-specific packages
- **Concurrent booking prevention** using Redis distributed locks
- **4-hour cancellation rule**: Credits refunded if cancelled 4+ hours before class
- **Waitlist system**: FIFO queue when class is full
- **Automatic waitlist promotion**: When someone cancels, next waitlist user is promoted
- **Automatic waitlist refund**: Hangfire job runs hourly to refund waitlist users after class ends
- **Check-in functionality**: Users can check in when class time arrives
- **Overlap prevention**: Users cannot book overlapping class times

## Project Structure

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
│   │   ├── Common/
│   │   └── Jobs/
│   ├── BookingSystem.Domain/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Package.cs
│   │   │   ├── UserPackage.cs
│   │   │   ├── Class.cs
│   │   │   ├── Booking.cs
│   │   │   ├── Waitlist.cs
│   │   │   └── Country.cs
│   │   ├── Enums/
│   │   └── Interfaces/
│   └── BookingSystem.Infrastructure/
│       ├── Persistence/
│       ├── Repositories/
│       └── Jobs/
└── tests/
    └── BookingSystem.UnitTests/
```

## Database Design

See [DATABASE_DESIGN.md](./DATABASE_DESIGN.md) for complete entity relationship diagram.

### Core Entities

- **Country**: Countries where classes and packages are available
- **Package**: Credit packages available for purchase (country-specific)
- **UserPackage**: User's purchased packages with remaining credits
- **Class**: Fitness class schedules with capacity and credit requirements
- **Booking**: User's class bookings
- **Waitlist**: Queue for full classes (FIFO)

## API Endpoints

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

## Getting Started

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

## Business Rules

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

## Mock Services

### Email Service
- `SendVerifyEmail()` - Returns true (mock)
- `SendPasswordResetEmail()` - Returns true (mock)

### Payment Service
- `AddPaymentCard()` - Returns true (mock)
- `PaymentCharge()` - Returns true (mock)

**Note**: In production, integrate with real email service (SendGrid, AWS SES) and payment gateway (Stripe, PayPal).

## Testing

Run unit tests:
```bash
dotnet test
```

## Hangfire Jobs

- **WaitlistRefundJob**: Runs hourly to refund credits to waitlist users whose classes have ended

## Security

- **JWT Bearer Authentication**: All protected endpoints require valid JWT token
- **Password Hashing**: BCrypt with salt
- **Email Verification**: Required before login
- **CORS**: Configured for cross-origin requests

## API Documentation

Swagger UI is available at `/swagger` in development mode, providing interactive API documentation.

## License

This project is for demonstration purposes.
