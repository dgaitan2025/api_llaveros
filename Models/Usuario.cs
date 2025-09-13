namespace ValidaNicknameApi.Models
{
    public class Usuario
    {
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Nickname { get; set; }
        public string PasswordHash { get; set; }
        public byte[]? Fotografia { get; set; }
        public string? FotografiaMime { get; set; }
        public byte[]? Fotografia2 { get; set; }
        public string? Fotografia2Mime { get; set; }
        public byte RolId { get; set; }
    }
}