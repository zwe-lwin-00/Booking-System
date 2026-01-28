namespace BookingSystem.Application.Common.Settings;

public class ValidationSettings
{
    public const string SectionName = "ValidationSettings";

    public int MaxGuestsPerBooking { get; set; } = 10;
    public int MaxRoomCapacity { get; set; } = 10;
    public int MaxBookingDaysInAdvance { get; set; } = 365;
    public bool AllowSameDayCheckIn { get; set; } = true;
    
    public FieldLengthLimits FieldLengths { get; set; } = new();
    
    public class FieldLengthLimits
    {
        public int RoomNumber { get; set; } = 50;
        public int RoomType { get; set; } = 100;
        public int RoomDescription { get; set; } = 500;
        public int FirstName { get; set; } = 100;
        public int LastName { get; set; } = 100;
        public int Email { get; set; } = 200;
    }
    
    public PhoneValidationSettings PhoneValidation { get; set; } = new();
    
    public class PhoneValidationSettings
    {
        public string Pattern { get; set; } = @"^\+?[1-9]\d{1,14}$";
        public string ErrorMessage { get; set; } = "Invalid phone number format.";
    }
}
