import EnderecoDTO from "../endereco/EnderecoDTO";
import NotaDTO from "./NotaDTO";
import ProdutoDTO from "../produto/ProdutoDTO";
import { ClientePedidoDTO } from "../cliente/ClienteDetalhadoDTO";

export default interface PedidoDTO {
    valorICMS: number;
    valorICMSST: number;
    id:string
    notasFiscais: {codigo: string; urlRastreio: string}[];
    boletos: string[];
    dataLancamento: string;
    informacoesNota: string;
    valorNota: number;
    valorIPI: number;
    margemPedido: number;
    dataEmissao: string;
    notas: NotaDTO[]
    observacoesPedido: string;
    valorProdutos: number;
    numeroPedidoCompra: string;
    modoFrete: 0 | 1
    planoPagamento: string;
    // 0 - Venda | 1 - Bonificação
    tipoPedido: 0 | 1,
    status: '0' | '1' | '2' | '3' | '4' | 'F' | 'P' | 'X' | 'N' | 'M' | 'R' | 'V'
    cliente: ClientePedidoDTO;
    enderecoEntrega: EnderecoDTO;
    dadosAgendamento?: {
        email: string;
        telefone: string;
    },
    produtos: ProdutoDTO[];
    idRepresentante?: string;
    nomeRepresentante?: string;
    idGerente?: string;
    nomeGerente?: string;
    pesoLiquido: number;
    pesoBruto: number;
}
