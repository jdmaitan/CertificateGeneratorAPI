using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CertificateGeneratorAPI.Settings
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(string secretKey)
        {

            Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}
