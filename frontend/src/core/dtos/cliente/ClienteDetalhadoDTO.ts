import EnderecoDTO from "../endereco/EnderecoDTO";

export interface ArquivoInfoDTO {
    nome?: string;
    conteudoBase64?: string;
}

export interface SocioInfoDTO {
    nome?: string;
    percentual?: string;
    cpf?: string;
}

export interface ReferenciaComercialDTO {
    razaoSocial?: string;
    telefone?: string;
    celular?: string;
    nomeContato?: string;
}

export interface DadosBancariosDTO {
    nomeBanco?: string;
    numeroBanco?: string;
    nomeAgencia?: string;
    numeroAgencia?: string;
    numeroConta?: string;
}

export interface ClientePedidoDTO {
    id?: string;
    nomeFantasia?: string;
    razaoSocial?: string;
    cnpj?: string;
    endereco?: EnderecoDTO;
}

export interface ClienteDetalhadoDTO {
    ramoAtividade?: string;
    endereco?: EnderecoDTO;
    dadosBancarios?: DadosBancariosDTO[];
    telefoneCobranca?: string;
    cnae?: string;
    cnpj?: string;
    referenciasComerciais?: ReferenciaComercialDTO[];
    razaoSocial?: string;
    nomeContato?: string;
    email?: string;
    fax?: string;
    dataFundacao?: string;
    id?: string;
    celular?: string;
    socios?: SocioInfoDTO[];
    telefone?: string;
    comprovantes?: ArquivoInfoDTO[];
    inscricaoEstadual?: string;
    suframa?: string;
    capitalSocial?: string;
    enderecoEntrega?: EnderecoDTO;
    telefoneEntrega?: string;
    emailEntrega?: string;
    nomeContatoEntrega?: string;
    contratoSocial?: ArquivoInfoDTO;
    documentoSintegra?: ArquivoInfoDTO;
    nomeFantasia?: string;
    notasComerciais?: ArquivoInfoDTO[];
    idRepresentante?: string;
    nomeRepresentante?: string;
    website?: string;
    idGerente?: string | null;
    nomeGerente?: string | null;
}
