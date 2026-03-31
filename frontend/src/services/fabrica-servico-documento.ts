import { excluir, obter, publicar, atualizar } from './base-service';
import CriarPedidoDTO from '../core/dtos/pedido/CriarPedidoDTO';
import RespostaCriarPedidoDTO from '../core/dtos/pedido/RespostaCriarPedidoDTO';
import DashboardPedidosDTO from '../core/dtos/pedido/DashboardPedidosDTO';
import RespostaPaginadaDTO from '../core/dtos/RespostaPaginadaDTO';
import RespostaPadraoDTO from '../core/interfaces/RespostaPadraoDTO';
import FiltroPadraoQuery from '../core/query-objects/FiltroPadraoQuery';

export const normalizarId = (id: string) => id.padStart(6, '0');

export function criarServicoDocumento<TLista, TDetalhe>(caminhoBase: '/pedido' | '/orcamento') {
    return {
        criar: (documento: CriarPedidoDTO) =>
            publicar<CriarPedidoDTO, RespostaCriarPedidoDTO>(caminhoBase, documento),

        obterLista: (filtro: FiltroPadraoQuery) =>
            obter<RespostaPaginadaDTO<TLista>>(caminhoBase, filtro),

        obterDashboard: (filtro: FiltroPadraoQuery) =>
            obter<DashboardPedidosDTO>(`${caminhoBase}/dashboard`, filtro),

        obterPorId: (id: string) =>
            obter<TDetalhe>(`${caminhoBase}/${normalizarId(id)}`),

        editar: (id: string, documento: CriarPedidoDTO) =>
            atualizar<CriarPedidoDTO, RespostaPadraoDTO>(`${caminhoBase}/${normalizarId(id)}`, documento),

        excluir: (id: string) =>
            excluir<RespostaPadraoDTO>(`${caminhoBase}/${normalizarId(id)}`),

        aprovar: (id: string) =>
            publicar<void, RespostaPadraoDTO>(`${caminhoBase}/${normalizarId(id)}/aprovar`, undefined),

        recusar: (id: string) =>
            publicar<void, RespostaPadraoDTO>(`${caminhoBase}/${normalizarId(id)}/recusar`, undefined),
    };
}
