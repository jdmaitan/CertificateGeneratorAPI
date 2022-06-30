using CertificateGeneratorAPI.Models.InputModels;
using CertificateGeneratorAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace CertificateGeneratorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        //[ApiExplorerSettings(IgnoreApi = true)] //Uncomment to hide method from swagger UI
        public object Authenticate([FromBody] AuthenticationDataInput authenticationDataInput, [FromServices] SigningConfigurations signingConfigurations, [FromServices] TokenConfigurations tokenConfigurations)
        {
            authenticationDataInput.Username = authenticationDataInput.Username.ToLower();

            if (!authenticationDataInput.Username.Contains(tokenConfigurations.Username)) { return BadRequest("Wrong username"); }
            if (!authenticationDataInput.Password.Contains(tokenConfigurations.Password)) { return BadRequest("Wrong password"); }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                   new GenericIdentity(authenticationDataInput.Username, "Login"),
                   new[]
                   {
                           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                           new Claim(JwtRegisteredClaimNames.UniqueName, authenticationDataInput.Username)
                   });

            DateTime creationDateTime = DateTime.Now;
            DateTime expirationDateTime = creationDateTime + TimeSpan.FromMinutes(tokenConfigurations.Minutes);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                SigningCredentials = signingConfigurations.SigningCredentials,
                Subject = claimsIdentity,
                NotBefore = creationDateTime,
                Expires = expirationDateTime
            });
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return Ok(new
            {
                authenticated = true,
                authenticationDataInput.Username,
                creationDateTime = creationDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                expirationDateTime = expirationDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token
            });
        }
    }
}
