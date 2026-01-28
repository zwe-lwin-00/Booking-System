# Test Procedure Guide

This document provides step-by-step testing procedures for all API endpoints and business logic scenarios.

## Prerequisites

1. **Start the Application**
   ```bash
   cd src/BookingSystem.API
   dotnet run
   ```

2. **Start Redis Server** (required for concurrent booking tests)
   ```bash
   redis-server
   # Or using Docker:
   docker run -d -p 6379:6379 redis
   ```

3. **Access Swagger UI**
   - Navigate to `https://localhost:5001/swagger`
   - Use Swagger UI for interactive testing

4. **Database Setup**
   - Database will be auto-created on first run
   - Seed data includes: Singapore (SG), Myanmar (MM) countries and sample packages

---

## Test Workflow Overview

```
1. Register User → 2. Verify Email → 3. Login → 4. Get Countries → 
5. View Packages → 6. Purchase Package → 7. View Classes → 
8. Book Class → 9. Test Cancellation → 10. Test Waitlist → 
11. Test Check-in → 12. Test Concurrent Booking
```

---

## 1. User Module Tests

### 1.1 User Registration

**Endpoint**: `POST /api/auth/register`

**Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**Expected Response**: `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-02-04T10:00:00Z",
  "user": {
    "id": "guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
  }
}
```

**Test Cases**:
- ✅ Valid registration
- ✅ Duplicate email (should return error)
- ✅ Password mismatch (should return error)
- ✅ Invalid email format (should return validation error)
- ✅ Missing required fields (should return validation error)

**Validation**:
- Check that `IsEmailVerified` is `false` in database
- Check that `EmailVerificationToken` is generated
- Check that password is hashed (not plain text)

---

### 1.2 Email Verification

**Endpoint**: `POST /api/auth/verify-email`

**Request Body**:
```json
{
  "email": "john.doe@example.com",
  "token": "verification-token-from-registration"
}
```

**Note**: In development, check the database `Users` table for the `EmailVerificationToken` value.

**Expected Response**: `200 OK`
```json
{
  "message": "Email verified successfully"
}
```

**Test Cases**:
- ✅ Valid token (should succeed)
- ✅ Invalid token (should return error)
- ✅ Expired token (should return error)
- ✅ Already verified email (should return error)

**Validation**:
- Check that `IsEmailVerified` is now `true` in database
- Check that `EmailVerificationToken` is cleared

---

### 1.3 Resend Verification Email

**Endpoint**: `POST /api/auth/resend-verification`

**Request Body**:
```json
"john.doe@example.com"
```

**Expected Response**: `200 OK`
```json
{
  "message": "Verification email sent"
}
```

**Test Cases**:
- ✅ Resend to unverified email (should succeed)
- ✅ Resend to already verified email (should return error)
- ✅ Invalid email (should return error)

---

### 1.4 User Login

**Endpoint**: `POST /api/auth/login`

**Request Body**:
```json
{
  "email": "john.doe@example.com",
  "password": "Password123!"
}
```

**Expected Response**: `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-02-04T10:00:00Z",
  "user": {
    "id": "guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890"
  }
}
```

**Test Cases**:
- ✅ Valid credentials with verified email (should succeed)
- ✅ Valid credentials but unverified email (should return error: "Email not verified")
- ✅ Invalid password (should return error: "Invalid email or password")
- ✅ Invalid email (should return error: "Invalid email or password")

**Important**: Save the JWT token for subsequent authenticated requests.

---

### 1.5 Get User Profile

**Endpoint**: `GET /api/users/profile`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Expected Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1234567890"
}
```

**Test Cases**:
- ✅ Valid token (should succeed)
- ✅ Missing token (should return 401 Unauthorized)
- ✅ Invalid/expired token (should return 401 Unauthorized)

---

### 1.6 Update User Profile

**Endpoint**: `PUT /api/users/profile`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Request Body**:
```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "phoneNumber": "+9876543210"
}
```

**Expected Response**: `200 OK`
```json
{
  "id": "guid",
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "phoneNumber": "+9876543210"
}
```

**Test Cases**:
- ✅ Valid update (should succeed)
- ✅ Duplicate email (should return error)
- ✅ Missing required fields (should return validation error)

---

### 1.7 Change Password

**Endpoint**: `POST /api/auth/change-password`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Request Body**:
```json
{
  "currentPassword": "Password123!",
  "newPassword": "NewPassword456!",
  "confirmPassword": "NewPassword456!"
}
```

**Expected Response**: `200 OK`
```json
{
  "message": "Password changed successfully"
}
```

**Test Cases**:
- ✅ Valid current password (should succeed)
- ✅ Invalid current password (should return error)
- ✅ Password mismatch (should return error)
- ✅ Missing token (should return 401)

**Validation**:
- Try logging in with old password (should fail)
- Try logging in with new password (should succeed)

---

### 1.8 Forgot Password

**Endpoint**: `POST /api/auth/forgot-password`

**Request Body**:
```json
{
  "email": "john.doe@example.com"
}
```

**Expected Response**: `200 OK`
```json
{
  "message": "Password reset email sent if account exists"
}
```

**Note**: Check database `Users` table for `PasswordResetToken`.

---

### 1.9 Reset Password

**Endpoint**: `POST /api/auth/reset-password?token={reset-token}`

**Request Body**:
```json
"NewPassword123!"
```

**Expected Response**: `200 OK`
```json
{
  "message": "Password reset successfully"
}
```

**Test Cases**:
- ✅ Valid token (should succeed)
- ✅ Invalid token (should return error)
- ✅ Expired token (should return error)

---

## 2. Package Module Tests

### 2.1 Get All Countries

**Endpoint**: `GET /api/countries`

**Expected Response**: `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Singapore",
    "code": "SG",
    "createdAt": "2026-01-28T10:00:00Z"
  },
  {
    "id": "guid",
    "name": "Myanmar",
    "code": "MM",
    "createdAt": "2026-01-28T10:00:00Z"
  }
]
```

**Test Cases**:
- ✅ Should return all countries
- ✅ Note the country IDs for subsequent tests

---

### 2.2 Get Packages by Country

**Endpoint**: `GET /api/packages/country/{countryId}`

**Expected Response**: `200 OK`
```json
[
  {
    "id": "guid",
    "name": "Basic Package SG",
    "countryId": "guid",
    "countryName": "Singapore",
    "countryCode": "SG",
    "credits": 5,
    "price": 50.00,
    "expiryDate": "2026-04-28T10:00:00Z",
    "isActive": true
  },
  {
    "id": "guid",
    "name": "Premium Package SG",
    "countryId": "guid",
    "countryName": "Singapore",
    "countryCode": "SG",
    "credits": 10,
    "price": 90.00,
    "expiryDate": "2026-07-28T10:00:00Z",
    "isActive": true
  }
]
```

**Test Cases**:
- ✅ Get packages for Singapore (should return SG packages)
- ✅ Get packages for Myanmar (should return MM packages)
- ✅ Invalid country ID (should return empty array or 404)

---

### 2.3 Purchase Package

**Endpoint**: `POST /api/packages/purchase`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Request Body**:
```json
{
  "packageId": "package-guid",
  "cardNumber": "4111111111111111",
  "cardHolderName": "John Doe",
  "expiryDate": "12/25",
  "cvv": "123"
}
```

**Expected Response**: `200 OK`
```json
{
  "id": "guid",
  "packageId": "guid",
  "packageName": "Basic Package SG",
  "countryName": "Singapore",
  "countryCode": "SG",
  "remainingCredits": 5,
  "totalCredits": 5,
  "purchaseDate": "2026-01-28T10:00:00Z",
  "expiryDate": "2026-04-28T10:00:00Z",
  "isExpired": false
}
```

**Test Cases**:
- ✅ Valid purchase (should succeed, mock payment returns true)
- ✅ Invalid package ID (should return 404)
- ✅ Inactive package (should return error)
- ✅ Missing token (should return 401)

**Validation**:
- Check `UserPackages` table for new entry
- Check `RemainingCredits` equals package credits

---

### 2.4 Get My Packages

**Endpoint**: `GET /api/packages/my-packages`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Expected Response**: `200 OK`
```json
[
  {
    "id": "guid",
    "packageId": "guid",
    "packageName": "Basic Package SG",
    "countryName": "Singapore",
    "countryCode": "SG",
    "remainingCredits": 5,
    "totalCredits": 5,
    "purchaseDate": "2026-01-28T10:00:00Z",
    "expiryDate": "2026-04-28T10:00:00Z",
    "isExpired": false
  }
]
```

**Test Cases**:
- ✅ User with packages (should return packages)
- ✅ User without packages (should return empty array)
- ✅ Expired packages should show `isExpired: true`

---

## 3. Schedule/Booking Module Tests

### 3.1 Get Classes by Country

**Endpoint**: `GET /api/classes/country/{countryId}`

**Expected Response**: `200 OK`
```json
[
  {
    "id": "guid",
    "name": "1 hr Yoga Class",
    "description": "Relaxing yoga session",
    "countryId": "guid",
    "countryName": "Singapore",
    "countryCode": "SG",
    "startTime": "2026-02-01T10:00:00Z",
    "endTime": "2026-02-01T11:00:00Z",
    "requiredCredits": 1,
    "maxCapacity": 10,
    "currentBookings": 0,
    "availableSlots": 10,
    "isFull": false
  }
]
```

**Test Cases**:
- ✅ Get classes for Singapore
- ✅ Get classes for Myanmar
- ✅ Classes should only show upcoming classes (StartTime > Now)

**Note**: If no classes exist, create test classes via database or add a create endpoint.

---

### 3.2 Book Class

**Endpoint**: `POST /api/schedule/book`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Request Body**:
```json
{
  "classId": "class-guid",
  "userPackageId": "user-package-guid"
}
```

**Expected Response**: `200 OK`
```json
{
  "id": "guid",
  "userId": "guid",
  "userName": "John Doe",
  "classId": "guid",
  "className": "1 hr Yoga Class",
  "checkInDate": "2026-02-01T10:00:00Z",
  "checkOutDate": "2026-02-01T11:00:00Z",
  "numberOfGuests": 1,
  "totalPrice": 0,
  "status": "Booked",
  "creditsUsed": 1,
  "isCheckedIn": false,
  "checkInTime": null,
  "createdAt": "2026-01-28T10:00:00Z"
}
```

**Test Cases**:
- ✅ Valid booking (should succeed)
- ✅ Insufficient credits (should return error)
- ✅ Package country mismatch (SG package booking MM class should fail)
- ✅ Class is full (should return error)
- ✅ Overlapping booking (should return error)
- ✅ Expired package (should return error)
- ✅ Missing token (should return 401)

**Validation**:
- Check `RemainingCredits` decreased by `RequiredCredits`
- Check `CurrentBookings` increased
- Check booking created in database

---

### 3.3 Test 4-Hour Cancellation Refund Rule

**Endpoint**: `POST /api/schedule/cancel/{bookingId}`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Test Scenario 1: Cancel 4+ Hours Before Class**
1. Create a class with `StartTime = Now + 5 hours`
2. Book the class
3. Note the `RemainingCredits` before cancellation
4. Cancel the booking
5. Check `RemainingCredits` increased (refunded)

**Test Scenario 2: Cancel Within 4 Hours**
1. Create a class with `StartTime = Now + 2 hours`
2. Book the class
3. Note the `RemainingCredits` before cancellation
4. Cancel the booking
5. Check `RemainingCredits` unchanged (no refund)

**Expected Response**: `200 OK`
```json
{
  "message": "Booking cancelled successfully"
}
```

**Validation**:
- Check database for refund logic
- Check `CurrentBookings` decreased
- Check waitlist promotion (if applicable)

---

### 3.4 Test Waitlist Functionality

**Prerequisites**: Create a class with `MaxCapacity = 2` and book it to full capacity.

**Step 1: Add to Waitlist**
**Endpoint**: `POST /api/schedule/waitlist`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Request Body**:
```json
{
  "classId": "full-class-guid",
  "userPackageId": "user-package-guid"
}
```

**Expected Response**: `200 OK`
```json
{
  "message": "Added to waitlist successfully"
}
```

**Test Cases**:
- ✅ Add to full class (should succeed)
- ✅ Add to non-full class (should return error: "Class is not full")
- ✅ Already in waitlist (should return error)
- ✅ Insufficient credits (should return error)
- ✅ Package country mismatch (should return error)

**Step 2: Test Waitlist Promotion**
1. Cancel a booking from the full class
2. Check that next waitlist user is automatically promoted
3. Verify booking created for waitlist user
4. Verify credits deducted from waitlist user's package

**Validation**:
- Check `Waitlists` table for entry
- Check `Position` is assigned (FIFO)
- Check `CreditsReserved` equals class `RequiredCredits`

---

### 3.5 Test Waitlist Refund (Scheduled Job)

**Prerequisites**: Waitlist users exist for ended classes.

**Manual Test**:
1. Create a class with `EndTime = Now - 1 hour` (already ended)
2. Add users to waitlist (non-promoted)
3. Manually trigger Hangfire job or wait for hourly schedule
4. Check that waitlist users' credits are refunded

**Via Hangfire Dashboard**:
1. Navigate to `https://localhost:5001/hangfire`
2. Find `WaitlistRefundJob.ProcessWaitlistRefunds`
3. Trigger manually or wait for scheduled execution
4. Check logs and database for refunds

**Validation**:
- Check `RemainingCredits` increased for waitlist users
- Check `IsPromoted` set to true (or waitlist marked as processed)

---

### 3.6 Test Check-In

**Endpoint**: `POST /api/schedule/check-in/{bookingId}`

**Headers**:
```
Authorization: Bearer {jwt-token}
```

**Test Scenario 1: Valid Check-In**
1. Create a class with `StartTime = Now - 10 minutes` (class started)
2. Book the class
3. Check in

**Expected Response**: `200 OK`
```json
{
  "message": "Checked in successfully"
}
```

**Test Cases**:
- ✅ Check-in 15 minutes before class (should succeed)
- ✅ Check-in during class time (should succeed)
- ✅ Check-in too early (before 15 minutes) (should return error)
- ✅ Check-in after class ended (should return error)
- ✅ Check-in to cancelled booking (should return error)
- ✅ Wrong user's booking (should return 404)

**Validation**:
- Check `IsCheckedIn` is `true`
- Check `CheckInTime` is recorded
- Check `Status` is `CheckedIn`

---

### 3.7 Test Overlap Prevention

**Test Scenario**:
1. Book Class A: `StartTime = 10:00, EndTime = 11:00`
2. Try to book Class B: `StartTime = 10:30, EndTime = 11:30` (overlaps)

**Expected Response**: `400 Bad Request`
```json
{
  "error": "You already have a booking that overlaps with this class time"
}
```

**Test Cases**:
- ✅ Overlapping start time (should fail)
- ✅ Overlapping end time (should fail)
- ✅ Completely overlapping (should fail)
- ✅ Non-overlapping (should succeed)
- ✅ Cancelled booking should not prevent overlap

---

### 3.8 Test Concurrent Booking Prevention

**Prerequisites**: Redis server must be running.

**Test Scenario**: Multiple users trying to book the last slot simultaneously.

**Manual Test**:
1. Create a class with `MaxCapacity = 1, CurrentBookings = 0` (1 slot available)
2. Use multiple API clients (Postman, curl, or Swagger) to book simultaneously
3. Only 1 should succeed, others should get error

**Expected Error**: `400 Bad Request`
```json
{
  "error": "Class is being booked by another user. Please try again."
}
```

**Automated Test** (using multiple threads/requests):
```bash
# Using curl in parallel
for i in {1..5}; do
  curl -X POST "https://localhost:5001/api/schedule/book" \
    -H "Authorization: Bearer {token}" \
    -H "Content-Type: application/json" \
    -d '{"classId":"guid","userPackageId":"guid"}' &
done
wait
```

**Validation**:
- Only 1 booking should be created
- `CurrentBookings` should be 1 (not 5)
- Redis lock should prevent overbooking
- Other requests should receive lock error

---

## 4. Integration Test Scenarios

### 4.1 Complete User Journey

1. **Register** → Get verification token
2. **Verify Email** → Email verified
3. **Login** → Get JWT token
4. **Get Countries** → Get Singapore ID
5. **Get Packages** → View SG packages
6. **Purchase Package** → Get UserPackage ID
7. **Get Classes** → View SG classes
8. **Book Class** → Booking created
9. **Get My Bookings** → See booking
10. **Cancel Booking** (4+ hours before) → Credits refunded
11. **Check-In** (when class time arrives) → Checked in

### 4.2 Waitlist Journey

1. **Book class to full capacity** (as User 1)
2. **Add to waitlist** (as User 2)
3. **Cancel booking** (as User 1)
4. **Verify waitlist promotion** (User 2 automatically booked)
5. **Wait for class to end**
6. **Verify waitlist refund** (via Hangfire job)

### 4.3 Country-Specific Package Test

1. **Purchase Singapore package** (SG)
2. **Try to book Myanmar class** (MM) → Should fail
3. **Book Singapore class** (SG) → Should succeed

---

## 5. Error Handling Tests

### 5.1 Validation Errors
- Missing required fields
- Invalid email format
- Password too short
- Invalid date formats

### 5.2 Business Logic Errors
- Insufficient credits
- Class full
- Package expired
- Country mismatch
- Overlapping bookings

### 5.3 Authentication Errors
- Missing JWT token
- Invalid/expired token
- Unauthorized access

---

## 6. Performance Tests

### 6.1 Concurrent Booking Load Test
- Use load testing tool (e.g., Apache JMeter, k6)
- Simulate 100 concurrent booking requests
- Verify only correct number of bookings created
- Verify Redis locks prevent overbooking

### 6.2 Database Query Performance
- Test pagination with large datasets
- Test filtering and search performance
- Monitor EF Core query execution

---

## 7. Testing Tools

### Recommended Tools:
1. **Swagger UI** - Interactive API testing
2. **Postman** - API collection and testing
3. **curl** - Command-line testing
4. **Hangfire Dashboard** - Monitor scheduled jobs
5. **SQL Server Management Studio** - Database verification
6. **Redis CLI** - Verify Redis locks

### Postman Collection Example:
```json
{
  "info": {
    "name": "Booking System API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Register",
      "request": {
        "method": "POST",
        "url": "{{baseUrl}}/api/auth/register",
        "body": {
          "mode": "raw",
          "raw": "{\n  \"firstName\": \"John\",\n  \"lastName\": \"Doe\",\n  \"email\": \"john@example.com\",\n  \"password\": \"Password123!\",\n  \"confirmPassword\": \"Password123!\"\n}"
        }
      }
    }
  ]
}
```

---

## 8. Test Data Setup

### Seed Data (Auto-created):
- **Countries**: Singapore (SG), Myanmar (MM)
- **Packages**: 
  - Basic Package SG (5 credits, $50)
  - Premium Package SG (10 credits, $90)
  - Basic Package MM (5 credits, $30)

### Manual Test Data:
Create test classes via database or add admin endpoints:
```sql
INSERT INTO Classes (Id, Name, Description, CountryId, StartTime, EndTime, RequiredCredits, MaxCapacity, CurrentBookings, CreatedAt, UpdatedAt)
VALUES 
  (NEWID(), 'Yoga Class SG', '1 hour yoga', @SingaporeId, DATEADD(hour, 5, GETUTCDATE()), DATEADD(hour, 6, GETUTCDATE()), 1, 10, 0, GETUTCDATE(), GETUTCDATE()),
  (NEWID(), 'Pilates Class SG', '1 hour pilates', @SingaporeId, DATEADD(hour, 24, GETUTCDATE()), DATEADD(hour, 25, GETUTCDATE()), 2, 5, 0, GETUTCDATE(), GETUTCDATE());
```

---

## 9. Checklist

### User Module
- [ ] Register user
- [ ] Verify email
- [ ] Login with verified email
- [ ] Login with unverified email (should fail)
- [ ] Get profile
- [ ] Update profile
- [ ] Change password
- [ ] Forgot password
- [ ] Reset password

### Package Module
- [ ] Get countries
- [ ] Get packages by country
- [ ] Purchase package
- [ ] Get my packages
- [ ] Verify expired package status

### Schedule Module
- [ ] Get classes by country
- [ ] Book class
- [ ] Cancel booking (4+ hours before - refund)
- [ ] Cancel booking (within 4 hours - no refund)
- [ ] Add to waitlist (full class)
- [ ] Waitlist promotion (on cancellation)
- [ ] Waitlist refund (after class ends)
- [ ] Check-in (valid time)
- [ ] Check-in (invalid time - should fail)
- [ ] Overlap prevention
- [ ] Concurrent booking prevention

### Integration
- [ ] Complete user journey
- [ ] Waitlist journey
- [ ] Country-specific package validation

---

## 10. Troubleshooting

### Common Issues:

1. **"Invalid object name 'Countries'"**
   - Solution: Database not created. Check connection string and ensure `EnsureCreatedAsync()` runs.

2. **"Class is being booked by another user"**
   - Solution: Redis lock is working. Retry the request.

3. **"Email not verified"**
   - Solution: Verify email first using the token from database.

4. **"Package country does not match class country"**
   - Solution: Use package and class from the same country.

5. **Hangfire job not running**
   - Solution: Check Hangfire dashboard, verify SQL Server connection, check job schedule.

---

## Notes

- All timestamps are in UTC
- JWT tokens expire after 7 days
- Email verification tokens expire after 24 hours
- Password reset tokens expire after 1 hour
- Redis locks expire after 10 seconds
- Hangfire waitlist refund job runs hourly
