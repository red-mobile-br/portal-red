export const useDownloader = () => {

    let baixador = document.querySelector("#downloader") as HTMLAnchorElement;

    if(baixador == null){
        baixador = document.createElement('a') ;
        baixador!.style.display = 'none';
        document.body.appendChild(baixador);
    }

    const baixarBase64 = async (dados: { base64: string; nomeArquivo: string; tipoMime: string }) => {

        const { base64, nomeArquivo, tipoMime } = dados;

        // Fazer o encode do base64
        const resposta = await fetch(`data:${tipoMime};base64,${base64}`);
        const bytes = await resposta.blob();
        const arquivo = new File([bytes], nomeArquivo, { type: tipoMime });

        // Criar o link de download virtual

        baixador.download = arquivo.name;
        baixador.href = URL.createObjectURL(arquivo);

        baixador.click();
    };
    return { baixarBase64 };
};
