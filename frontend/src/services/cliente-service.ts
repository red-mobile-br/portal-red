import DashboardClienteDTO from "@/core/dtos/cliente/DashboardClienteDTO";
import ClienteListaItemDTO from "@/core/dtos/cliente/ClienteListaItemDTO";
import ClienteQuery from "@/core/query-objects/ClienteQuery";
import { removerMascaraCnpj } from "@/utils/string-functions";
import { obter, publicar } from "./base-service";
import PaginationResponseModel from '@/core/dtos/RespostaPaginadaDTO';
import ProdutoQuery from "@/core/query-objects/ProdutoQuery";
import ProdutoDTO from "@/core/dtos/produto/ProdutoDTO";
import ImpostoProdutoDTO from "@/core/dtos/produto/ImpostoProdutoDTO";
import ConsultaImpostoDTO from "@/core/dtos/produto/ConsultaImpostoDTO";
import ConsultaImpostoLoteDTO from "@/core/dtos/produto/ConsultaImpostoLoteDTO";
import RespostaImpostoLoteDTO from "@/core/dtos/produto/RespostaImpostoLoteDTO";
import { ClienteDetalhadoDTO } from "@/core/dtos/cliente/ClienteDetalhadoDTO";
import PedidoListaItemDTO from "@/core/dtos/pedido/PedidoListaItemDTO";
import $http from './http_client/http';
import FiltroPadraoQuery from "@/core/query-objects/FiltroPadraoQuery";
import RepresentanteListaDTO from "@/core/dtos/representante/RepresentanteListaDTO";

export const clienteService = {
    obterClientes: (query: ClienteQuery) => {
        return obter<PaginationResponseModel<ClienteListaItemDTO>>('/cliente', query);
    },
    buscarClientes: (query: FiltroPadraoQuery) => {
        return obter<PaginationResponseModel<RepresentanteListaDTO>>('/cliente', query);
    },
    criarCliente: async (dados: FormData) => {
        const resp = await  $http.postForm('/cliente', dados);
        return resp.data;
    },
    obterDetalheCliente: (cnpj: string) => {
        return obter<ClienteDetalhadoDTO>(`/cliente/${removerMascaraCnpj(cnpj)}`);
    },
    obterDashboardClientes: (query: ClienteQuery) => {
        return obter<DashboardClienteDTO>('/cliente/dashboard', query);
    },
    obterPedidosCliente: (cnpj: string, query: ClienteQuery) => {
        return obter<PaginationResponseModel<PedidoListaItemDTO>>(`/cliente/${cnpj}/pedidos`, query);
    },
    obterProdutosCliente: (idCliente: string, query: ProdutoQuery) => {
        return obter<PaginationResponseModel<ProdutoDTO>>(`/cliente/${idCliente}/produtos`, query);
    },
    obterDetalheProduto: (idCliente: string, idProduto: string) => {
        return obter<ProdutoDTO>(`/cliente/${idCliente}/produto/${idProduto.padStart(6, '0')}`);
    },
    obterImpostoProduto: (idCliente: string, codigoProduto: string, dados: ConsultaImpostoDTO) => {
        return publicar<ConsultaImpostoDTO, ImpostoProdutoDTO>(`/cliente/${idCliente}/produto/${codigoProduto}`, dados);
    },
    obterImpostosLote: (dados: ConsultaImpostoLoteDTO) => {
        return publicar<ConsultaImpostoLoteDTO, RespostaImpostoLoteDTO>('/cliente/impostos/lote', dados);
    }
};
