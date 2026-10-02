using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string NomeUsuario { get; set; }
        [Required]
        public required string Senha {  get; set; }
    }
}
