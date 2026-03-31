import { criarServicoDocumento } from './fabrica-servico-documento';
import OrcamentoDTO from '../core/dtos/pedido/OrcamentoDTO';
import OrcamentoListaItemDTO from '../core/dtos/pedido/OrcamentoListaItemDTO';

const orcamentoService = {
    ...criarServicoDocumento<OrcamentoListaItemDTO, OrcamentoDTO>('/orcamento'),
};

export default orcamentoService;
