export default interface CriarPedidoDTO {
    idCliente: string;
    modoFrete: 0 | 1;
    dadosAgendamento?: {
        email: string;
        telefone: string
    };
    enderecoEntrega: {
        logradouro: string;
        numero: number | '';
        complemento: string;
        bairro: string;
        cidade: string;
        uf: string;
        cep: string
    };
    produtos: {
        id: string;
        quantidade: number;
        valorUnitario: number;
        comissao: number
    }[];
    numeroPedidoCompra: string;
    dataEmissao: string;
    informacoesNota: string;
    observacoesPedido: string;
    // 0 - Venda | 1 - Bonificação
    tipoPedido: 0 | 1;
    planoPagamento: string;
    idRepresentante: string | null;
    idGerente: string | null;
}
