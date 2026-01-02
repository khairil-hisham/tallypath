namespace Tallypath.Data
{
    public class RegisterDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Fullname { get; set; } = "";
        public string Email { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Dob { get; set; } = "";
    }

    public class LoginDto
    {
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDeviceRequest
    {
        public string DeviceId { get; set; } = null!;
        public string Platform { get; set; } = null!;
        public string FcmToken { get; set; } = null!;
    }
    public class DeactivateDeviceRequest
    {
        public string? DeviceId { get; set; }
        public string? FcmToken { get; set; }
    }
}
