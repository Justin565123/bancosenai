const URL_API = 'https://localhost:7081/swagger/index.html'

async function enviarDocumento() {
    const codigoCLiente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCLiente || !arquivo)
    {
        alert("Informe o codigo do cliente e selecione um arquivo");
        return
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    const rensponse = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method = "POST",
        body: dadosArquivo,

    });
    if (response.ok) {
        const codigoCLiente = document.getElementById("codigoCliente").value = "";
        const inputArquivo = document.getElementById("arquivo").value = "";
    } else {
        console.log("Erro");
        const erro = await response.json();
        alert("Erro: ", (erro.mensage || "Falha ao enviar o documento"));
    }

}