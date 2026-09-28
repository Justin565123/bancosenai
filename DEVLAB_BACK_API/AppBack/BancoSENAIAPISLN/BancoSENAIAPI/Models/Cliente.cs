namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        public int Codigo { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public int NumeroAgencia { get; set; }

        public decimal Saldo { get; set; } = 0.0m;
    }
}
