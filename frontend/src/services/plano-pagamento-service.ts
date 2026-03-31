import PlanoPagamentoDTO from "../core/dtos/planosPagamento/PlanoPagamentoDTO";
import { obter } from "./base-service";

const planoPagamentoService = {
    obterPlanosPagamento: () => {
        return obter<PlanoPagamentoDTO[]>('/planoPagamento');
    }
};

export default planoPagamentoService;
