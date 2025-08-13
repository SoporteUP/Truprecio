using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Truprecio.Server.Models;
using Truprecio.Shared;

namespace Truprecio.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TruPreciosContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(TruPreciosContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            // Buscar usuario por correo y password
            var user = await _context.UsuariosPros
                .FirstOrDefaultAsync(u => u.Correo == request.Correo && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Credenciales inválidas");

            var token = GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                Correo = user.Correo ?? string.Empty,
                Nombre = user.Correo ?? string.Empty // Usamos el mismo correo porque no tienes campo nombre
            };
        }

        private string GenerateToken(UsuariosPro user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Correo ?? string.Empty),
                new Claim("usuarioId", user.UsuarioId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
