import OrcamentosFeature from "@/features/orcamentos-feature/OrcamentosFeature.vue";
import { orcamentosFeatureRouter } from "@/features/orcamentos-feature/router/orcamentos-feature-router";
import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import useAppStore from '../store/app-store';

const AdminFeature = () => import('@/features/admin-feature/AdminFeature.vue');

const AutenticacaoFeature = () => import(/* webpackChunkName: "auth" */'@/features/autenticacao-feature/AutenticacaoFeature.vue');
import { autenticacaoFeatureRouter } from '@/features/autenticacao-feature/router/autenticacao-feature-router';

const PedidosFeature = () => import(/* webpackChunkName: "admin" */'@/features/pedidos-feature/PedidosFeature.vue');
import { pedidosFeatureRouter } from '@/features/pedidos-feature/router/pedidos-feature-router';

const FaturamentoFeature = () => import(/* webpackChunkName: "admin" */'@/features/faturamento-feature/FaturamentoFeature.vue');
import { faturamentoFeatureRouter } from '@/features/faturamento-feature/router/faturamento-feature-router';

const ComissoesFeature = () => import(/* webpackChunkName: "admin" */'@/features/comissoes-feature/ComissoesFeature.vue');
import { comissoesFeatureRouter } from '@/features/comissoes-feature/router/comissoes-feature-router';

const DashboardFeature = () => import(/* webpackChunkName: "admin" */'@/features/dashboard-feature/DashboardFeature.vue');

const TitulosFeature = () => import(/* webpackChunkName: "admin" */'@/features/titulos-feature/TitulosFeature.vue');
import { titulosFeatureRouter } from '@/features/titulos-feature/router/titulos-feature-router';

const ClientesFeature = () => import(/* webpackChunkName: "customer" */'@/features/clientes-feature/ClientesFeature.vue');
import { clientesFeatureRouter } from '@/features/clientes-feature/router/clientes-feature-router';

const TabelaPrecosFeature = () => import(/* webpackChunkName: "admin" */'@/features/tabela-precos-feature/TabelaPrecosFeature.vue');
import { tabelaPrecosFeatureRouter } from '@/features/tabela-precos-feature/router/tabela-precos-feature-router';

const MetasFeature = () => import(/* webpackChunkName: "admin" */'@/features/metas-feature/MetasFeature.vue');
import { metasFeatureRouter } from '@/features/metas-feature/router/metas-feature-router';

// Print
const ImpressaoDashboardPedidosPage = () => import('@/features/pedidos-feature/pages/print/ImpressaoDashboardPedidosPage.vue');
const ImpressaoDashboardFaturamentoPage = () => import('@/features/faturamento-feature/pages/print/ImpressaoDashboardFaturamentoPage.vue');
const ImpressaoDashboardComissoesPage = () => import('@/features/comissoes-feature/pages/print/ImpressaoDashboardComissoesPage.vue');
const ImpressaoDashboardTitulosPage = () => import('@/features/titulos-feature/pages/print/ImpressaoDashboardTitulosPage.vue');
const ImpressaoDashboardClientesPage = () => import('@/features/clientes-feature/pages/print/ImpressaoDashboardClientesPage.vue');
const ImpressaoTabelaPrecosPage = () => import('@/features/tabela-precos-feature/pages/print/ImpressaoTabelaPrecosPage.vue');
const ImpressaoDetalhePedidoCompactoPage = () => import('@/features/pedidos-feature/pages/print/ImpressaoDetalhePedidoCompactoPage.vue');

const routes: Array<RouteRecordRaw> = [
    {
        path: '/',
        component: AdminFeature,
        children: [
            {
                path: '',
                name: 'dashboard',
                component: DashboardFeature
            },
            // ======== Pedidos =============
            {
                path: 'pedidos',
                component: PedidosFeature,
                children: pedidosFeatureRouter
            },
            // ======== Orçamentos =============
            {
                path: 'orcamentos',
                component: OrcamentosFeature,
                children: orcamentosFeatureRouter
            },

            // ========= Faturamento =========
            {
                path: 'faturamento',
                component: FaturamentoFeature,
                children: faturamentoFeatureRouter()
            },
            // ========= Comissões =========
            {
                path: 'comissoes',
                component: ComissoesFeature,
                children: comissoesFeatureRouter
            },
            // ========= Títulos =========
            {
                path: 'titulos',
                component: TitulosFeature,
                children: titulosFeatureRouter
            },
            // ========= Clientes =========
            {
                path: 'clientes',
                component: ClientesFeature,
                children: clientesFeatureRouter
            },
            // ========= Tabela de preços =========
            {
                path: 'tabelaprecos',
                component: TabelaPrecosFeature,
                children: tabelaPrecosFeatureRouter
            },

            // ========= Metas =========
            {
                path: 'metas',
                component: MetasFeature,
                children: metasFeatureRouter
            },
        ]
    },
    {
        path: '/login',
        component: AutenticacaoFeature,
        children: autenticacaoFeatureRouter
    },

    // Impressão pedidos
    {
        path: '/impressao/pedidos',
        name: 'impressaoPedidos',
        component: ImpressaoDashboardPedidosPage
    },
    {
        path: '/impressao/orcamentos',
        name: 'impressaoOrcamentos',
        component: ImpressaoDashboardPedidosPage
    },
    {
        path: '/impressao/pedido/:id',
        name: 'impressaoPedido',
        component: ImpressaoDetalhePedidoCompactoPage
    },
    // // Impressão faturamento
    {
        path: '/impressao/faturamento',
        name: 'impressaoFaturamento',
        component: ImpressaoDashboardFaturamentoPage
    },
    // Impressão titulos
    {
        path: '/impressao/titulos',
        name: 'impressaoTitulos',
        component: ImpressaoDashboardTitulosPage
    },
    // Impressão clientes
    {
        path: '/impressao/clientes',
        name: 'impressaoClientes',
        component: ImpressaoDashboardClientesPage
    },
    // Impressão tabela
    {
        path: '/impressao/tabela',
        name: 'impressaoTabela',
        component: ImpressaoTabelaPrecosPage
    },
    // Impressão comissoes
    {
        path: '/impressao/comissoes',
        name: 'impressaoComissoes',
        component: ImpressaoDashboardComissoesPage
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

let substituindo = false;
router.beforeEach((to, from , next) => {
    const { verificarDadosUsuario } = useAppStore();

    // Se for uma rota pública
    if(to.meta.isPublic) {
        next();
        return;
    }

    // Verificar se está autenticado
    const autenticado = verificarDadosUsuario();
    if(!autenticado) {
        next({ name: 'login' });
        return;
    }

    // Verificar se existe filtros na sessão
    const filtrosSessao = sessionStorage.getItem(to.name!.toString());
    if(filtrosSessao && !substituindo) {
        substituindo = true;
        const query = JSON.parse(filtrosSessao);
        to.query = query;
        next(to);
        return;
    }

    substituindo = false;
    next();
});

export default router;
