import NotaDTO from "./NotaDTO";

export default interface PedidoListaItemDTO {
    idCliente?: string;
    cnpj?: string;
    id?: string;
    notasFiscais?: {codigo?: string; urlRastreio?: string}[]
    boletos?: string[]
    dataLancamento?: string;
    nome?: string;
    notas?: NotaDTO[]
    representante?: string;
    nomeRepresentante?: string;
    status?: '0' | '1' | '2' | '3' | '4' | 'F' | 'P' | 'X' | 'N' | 'M' | 'R' | 'V';
    valorTotal?: number;
}
