using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/vi/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(), 
            "ClienteArquivos"
            );

        private static List<Models.DocumentoMetadado>
            _documentosMetadados
            = new List<Models.DocumentoMetadado>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhm arquivo foi enviado");
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }
            string extensao = Path.GetExtension(arquivo.FileName);
            string nameOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nameOriginal}_{Guid.NewGuid()}{extensao}";
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

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });
        }

        [HttpGet("v1/documento/listar/{codigoCliente}")]

        public async Task<IActionResult> listagem(int codigoCliente)
        {
            var documentos = _documentosMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

            if (!documentos.Any())
            {
                return NotFound(new {mensagem = $"Nenhum documento foi encontrado, verifique seu cadastro. {codigoCliente}"});
            }

            return Ok(documentos);
        }
        [HttpGet("v1/docimento/download/{id}")]

        public async Task<IActionResult> Download(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new { mensagem = "Documento não encontrado." });
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound(new { mensagem = "O arquivo físico não foi encontrado no servidor." });
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(documento.Caminho);

            string nomeArquivoCompleto = $"{documento.Name}{documento.Extensao}";

            return File(fileBytes, "application/octet-stream", nomeArquivoCompleto);
        }
    }
}
