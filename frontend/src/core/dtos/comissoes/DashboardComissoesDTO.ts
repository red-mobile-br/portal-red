import ItemGraficoDTO from "../ItemGraficoDTO";

export default interface DashboardComissoesDTO {
    totalComissaoPeriodo: number;
    percentualComissaoPeriodo: number;
    comissoesPeriodo: ItemGraficoDTO[];
    maioresComissoesPeriodo: ItemGraficoDTO[];
}
