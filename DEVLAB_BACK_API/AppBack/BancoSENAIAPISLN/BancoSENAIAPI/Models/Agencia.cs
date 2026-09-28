using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class Agencia
    {
        [Key]
        public int NumeroAgencia { get; set; }
        [Required]
        public string? Cidade { get; set; }
        [Required]
        public string? SiglaEstado { get; set; }

    }
}
