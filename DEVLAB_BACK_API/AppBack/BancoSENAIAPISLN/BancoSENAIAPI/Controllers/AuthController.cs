using BancoSENAIAPI.Data;
using BancoSENAIAPI.Dtos;
using BancoSENAIAPI.Models;
using BancoSENAIAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/vi/[controller]")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("registar")]
        public async Task<IActionResult> Registrar([FromBody] RegisterRequest dto)
        { 
           if (await _context.Usuarios.AnyAsync(u => u.NomeUsuario == dto.NomeUsuario))
            {
                return BadRequest(new { message = "Este nome de usuario já existe" });
            }

            var usuario = new Usuario
            {
                NomeUsuario = dto.NomeUsuario,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Created("", new { usuario.Id, usuario.NomeUsuario });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.NomeUsuario == dto.NomeUsuario);
               if (usuario == null|| !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            {
                return Unauthorized(new {message = "Usuario ou senha invalidos" });
            }
            var (token, expiraEm) = _tokenService.GerarToken(usuario);

            return Ok(new LoginResponseDto { Token = token, ExpiraEm = expiraEm});
        }
    }
}
