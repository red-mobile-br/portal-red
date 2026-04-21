import ItemGraficoDTO from "../ItemGraficoDTO";

export default interface DashboardComissoesDTO {
    totalComissaoPorPeriodo?: number;
    percentualComissaoPorPeriodo?: number;
    comissoesPorPeriodo?: ItemGraficoDTO[];
    maiorComissaoPorPeriodo?: ItemGraficoDTO[];
}
