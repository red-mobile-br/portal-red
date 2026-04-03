import ItemGraficoDTO from "../ItemGraficoDTO";

export default interface DashboardFaturamentoDTO {
    produtosMaisVendidos?: {
        urlImagem?: string;
        nome?: string;
        unidades?: number;
    }[];
    vendasPorCategoria?: ItemGraficoDTO[];
    vendasPorDia?: ItemGraficoDTO[];
    quantidadeVendas?: number;
    totalBase?: number;
    totalFaturado?: number;
}
