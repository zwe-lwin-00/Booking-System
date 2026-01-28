# Booking System

A clean architecture-based booking system built with .NET 8.0, following Domain-Driven Design principles.

## Architecture

This project follows **Clean Architecture** (Onion Architecture) with clear separation of concerns:

- **BookingSystem.Domain**: Core business entities and interfaces (no dependencies)
- **BookingSystem.Application**: Business logic, DTOs, and services
- **BookingSystem.Infrastructure**: Data access, repositories, and external services
- **BookingSystem.API**: Web API controllers and configuration
- **BookingSystem.UnitTests**: Unit tests for the application

## Project Structure

```
BookingSystem/
├── src/
│   ├── BookingSystem.API/
│   │   ├── Controllers/
│   │   │   ├── BookingsController.cs
│   │   │   ├── RoomsController.cs
│   │   │   └── UsersController.cs
│   │   └── Program.cs
│   ├── BookingSystem.Application/
│   │   ├── DTOs/
│   │   └── Services/
│   ├── BookingSystem.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Interfaces/
│   └── BookingSystem.Infrastructure/
│       ├── Persistence/
│       └── Repositories/
└── tests/
    └── BookingSystem.UnitTests/
```

## Features

- **Bookings Management**: Create, read, update, and delete bookings
- **Rooms Management**: Manage room inventory and availability
- **Users Management**: Handle user accounts
- **Availability Checking**: Automatically check room availability for date ranges
- **Status Management**: Track booking status (Pending, Confirmed, Cancelled, Completed)

## Technology Stack

- .NET 8.0
- Entity Framework Core 8.0
- SQL Server (LocalDB for development)
- Swagger/OpenAPI
- xUnit (for testing)

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (or LocalDB)

### Setup

1. **Update Connection String**

   Edit `src/BookingSystem.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Your connection string here"
     }
   }
   ```

2. **Run the Application**

   ```bash
   cd src/BookingSystem.API
   dotnet run
   ```

3. **Access Swagger UI**

   Navigate to `https://localhost:5001/swagger` (or the port shown in console)

### Database

The database will be automatically created on first run (using `EnsureCreated()`). For production, use migrations:

```bash
dotnet ef migrations add InitialCreate --project src/BookingSystem.Infrastructure --startup-project src/BookingSystem.API
dotnet ef database update --project src/BookingSystem.Infrastructure --startup-project src/BookingSystem.API
```

## API Endpoints

### Bookings

- `GET /api/bookings` - Get all bookings
- `GET /api/bookings/{id}` - Get booking by ID
- `GET /api/bookings/user/{userId}` - Get bookings by user
- `POST /api/bookings` - Create new booking
- `PUT /api/bookings/{id}/status` - Update booking status
- `DELETE /api/bookings/{id}` - Delete booking

### Rooms

- `GET /api/rooms` - Get all rooms
- `GET /api/rooms/{id}` - Get room by ID
- `GET /api/rooms/available?checkIn={date}&checkOut={date}` - Get available rooms
- `POST /api/rooms` - Create new room
- `PUT /api/rooms/{id}` - Update room
- `DELETE /api/rooms/{id}` - Delete room

### Users

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/email/{email}` - Get user by email
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

## Domain Model

### Entities

- **User**: Customer information
- **Room**: Room details and pricing
- **Booking**: Reservation linking User and Room with dates and status

### Enums

- **BookingStatus**: Pending, Confirmed, Cancelled, Completed

## Testing

Run unit tests:

```bash
dotnet test
```

## Future Enhancements

- Authentication and Authorization
- Payment processing
- Email notifications
- Advanced search and filtering
- Caching with Redis
- Background jobs with Hangfire
- API versioning
- Request validation
- Logging and monitoring

## License

This project is for demonstration purposes.
