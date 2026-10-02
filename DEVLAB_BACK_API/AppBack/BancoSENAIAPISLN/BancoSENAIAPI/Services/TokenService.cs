using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BancoSENAIAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace BancoSENAIAPI.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var chave = jwtSection["Key"]!;
            var minutos = int.Parse(jwtSection["ExpiraMinutos"] ?? "60");
            var expiraEm = DateTime.UtcNow.AddMinutes(minutos);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NomeUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", usuario.Id.ToString())
            };

            var credenciais = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expiraEm,
                signingCredentials: credenciais);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
        }
    }
}
