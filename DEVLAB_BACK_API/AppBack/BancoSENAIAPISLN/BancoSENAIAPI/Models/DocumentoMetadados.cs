namespace BancoSENAIAPI.Models
{
    public class DocumentoMetadados
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Extensao { get; set; }

        public string Caminho { get; set; }

        public int CodigoCliente { get; set; }
    }
}

