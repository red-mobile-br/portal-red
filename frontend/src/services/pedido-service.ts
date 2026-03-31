import { obter, publicar } from './base-service';
import { criarServicoDocumento, normalizarId } from './fabrica-servico-documento';
import PedidoDTO from '../core/dtos/pedido/PedidoDTO';
import PedidoListaItemDTO from '../core/dtos/pedido/PedidoListaItemDTO';

const pedidoService = {
    ...criarServicoDocumento<PedidoListaItemDTO, PedidoDTO>('/pedido'),

    obterNotaFiscal: (idPedido: string, idNota: string) =>
        obter<{ data: string }>(`/pedido/${normalizarId(idPedido)}/notaFiscal/${idNota}`),

    obterBoleto: (idPedido: string, idNota: string) =>
        obter<{ data: string }>(`/pedido/${normalizarId(idPedido)}/boleto/${idNota}`),

    enviarEmail: (idPedido: string, destinatario: string) =>
        publicar<{ destinatario: string }, { message: string }>(`/pedido/${normalizarId(idPedido)}/email`, { destinatario }),
};

export default pedidoService;
