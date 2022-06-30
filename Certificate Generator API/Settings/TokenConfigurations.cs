namespace CertificateGeneratorAPI.Settings
{
    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Minutes { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}