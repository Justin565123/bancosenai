// Ajuste a URL para a rota base da sua API (remova o Swagger)
const URL_API = 'https://localhost:7081';

async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o código do cliente e selecione um arquivo");
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    try {
        // Corrigido: 'method: "POST"' com dois pontos
        const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
            method: "POST",
            body: dadosArquivo
        });

        if (response.ok) {
            document.getElementById("codigoCliente").value = "";
            document.getElementById("arquivo").value = "";
            alert("Documento enviado com sucesso!");
        } else {
            const erro = await response.json();
            alert("Erro: " + (erro.message || "Falha ao enviar o documento"));
        }
    } catch (error) {
        console.error("Erro na requisição:", error);
        alert("Erro ao conectar com o servidor.");
    }
}