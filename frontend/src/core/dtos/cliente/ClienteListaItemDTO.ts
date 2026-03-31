// 0 p/ 'Sem Compras', 1 p/ 'Ativo' e 2 p/ 'Inativo'
export default interface ClienteListaItemDTO {
    cidade: string;
    cnpj: string;
    diasSemComprar: number;
    id: string;
    ultimaCompra: string;
    nome: string;
    status: 0 | 1 | 2;
    uf: string;
    idRepresentante: string;
    nomeRepresentante: string;
}
