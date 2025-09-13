namespace ValidaNickname.Models
{
    public class LoginRequest
    {
        public string Usuario { get; set; } = string.Empty; // email o nickname
        public string Password { get; set; } = string.Empty;
    }
}
