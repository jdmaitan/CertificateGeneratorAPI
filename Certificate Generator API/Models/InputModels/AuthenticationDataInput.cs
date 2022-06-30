using System.ComponentModel.DataAnnotations;

namespace CertificateGeneratorAPI.Models.InputModels
{
    public class AuthenticationDataInput
    {
        [EmailAddress]
        public string Username { get; set; }

        public string Password { get; set; }
    }
}