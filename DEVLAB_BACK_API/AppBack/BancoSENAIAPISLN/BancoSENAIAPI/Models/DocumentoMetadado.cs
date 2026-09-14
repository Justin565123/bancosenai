namespace BancoSENAIAPI.Models
{
    public class DocumentoMetadado
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Extensao { get; set;} = string.Empty;

        public string Caminho { get; set;} = string.Empty;

        public int CodigoCliente { get; set;}
    }
}
