using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Services;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase 
    {
        private static List<Models.Cliente> _clientes = new List<Models.Cliente>()
        {
            new Cliente() { Codigo = 1, Nome = "João Silva", Cpf = "11849572070", NumeroAgencia = 1001, Saldo = 1500.00m },
        };
        private static int _nextId = 2;

        [HttpGet]
        public IActionResult ListarTodos()
        {
            return Ok(_clientes);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente novoCliente)
        {
            if (string.IsNullOrWhiteSpace(novoCliente.Nome)) return BadRequest("O nome do cliente é obrigatório.");
            if (string.IsNullOrWhiteSpace(novoCliente.Cpf)) return BadRequest("O CPF do cliente é obrigatório.");

            // Remove todos os caracteres não numéricos do CPF
            string cpf = new string(novoCliente.Cpf.Where(char.IsDigit).ToArray());

            if (!FormService.ValidarCPF(cpf)) return BadRequest("O CPF do cliente é inválido.");

            novoCliente.Cpf = cpf;
            novoCliente.Codigo = _nextId++;

            _clientes.Add(novoCliente);

            // Retorna Status 201 Created conforme boas práticas REST
            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(a => a.Codigo == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." }); // Status 404

            return Ok(cliente); // Status 200 OK
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = _clientes.FirstOrDefault(a => a.Codigo == codigo);

            if (clienteExistente == null) return NotFound(new { message = "Cliente não encontrado" });

            clienteExistente.Nome = clienteAtualizado.Nome;
            clienteExistente.Cpf = clienteAtualizado.Cpf;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
            clienteExistente.Saldo = clienteAtualizado.Saldo;

            // Retorna Status 204 No Content para atualizações bem-sucedidas
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Codigo == codigo);

            if (cliente == null) return NotFound(new { message = "Cliente não encontrado." });

            _clientes.Remove(cliente);
            return Ok(new { message = "Cliente excluído com sucesso." }); // Status 200
        }
    }
}
