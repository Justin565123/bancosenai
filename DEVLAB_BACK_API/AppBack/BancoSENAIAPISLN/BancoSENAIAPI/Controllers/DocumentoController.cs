using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/vi/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetcurrentDirectory(), 
            "ClienteArquivos"
            );

        private static List<Models.DocumentoMetadado>
            _documentosMetadados
            = new List<Models.DocumentoMetadado>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null | | arquivo.Length == 0)
            {
                return BadRequest("Nenhm arquivo foi enviado")
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }
            string extensao = Path.GetExtension(arquivo.FileName);
            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);
            
            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId++,
                Name = nameOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new {mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome})
        }
    }
}
