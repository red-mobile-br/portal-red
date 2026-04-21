import ItemGraficoDTO from "../ItemGraficoDTO";

export default interface DashboardTitulosDTO {
    valorRecebido?: number;
    valorAReceber?: number;
    desempenhoPorPeriodo?: ItemGraficoDTO[];
}
