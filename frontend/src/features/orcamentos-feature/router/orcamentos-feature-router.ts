import { RouteRecordRaw } from "vue-router";

const DashboardPedidosPage = () => import(/* webpackChunkName: "admin" */'../../pedidos-feature/pages/dashboard/DashboardPedidosPage.vue');
const ConsultarPedidoPage = () => import(/* webpackChunkName: "admin" */'../../pedidos-feature/pages/consult/ConsultarPedidoPage.vue');
const CriarPedidoPage = () => import(/* webpackChunkName: "admin" */'../../pedidos-feature/pages/create/CriarPedidoPage.vue');

const orcamentosFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'orcamentos',
        component: DashboardPedidosPage
    },
    {
        path: 'novo',
        name: 'novoOrcamento',
        component: CriarPedidoPage
    },
    {
        path: 'consultar/:id?',
        name: 'consultarOrcamento',
        component: ConsultarPedidoPage
    },
    {
        path: 'editar/:id?',
        name: 'editarOrcamento',
        component: CriarPedidoPage
    }
];

export { orcamentosFeatureRouter };