using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Dtos
{
    public class RegisterRequest
    {
        [Required]
        public required string NomeUsuario { get; set; }
        [Required]
        [MinLength(6)]
        public required string Senha { get; set; }
    }
}
