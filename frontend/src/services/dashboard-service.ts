import { DashboardDTO } from "@/core/dtos/dashboard/dashboard-dto";
import { ObjetoServico, obter } from "./base-service";
import FiltroPadraoQuery from "@/core/query-objects/FiltroPadraoQuery";

const dashboardService = {
    obterDashboard(query: FiltroPadraoQuery): ObjetoServico<DashboardDTO> {
        return obter<DashboardDTO>('/dashboard', query);
    }
};

export { dashboardService };
