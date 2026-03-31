import { RouteRecordRaw } from "vue-router";

const DashboardPedidosPage = () => import(/* webpackChunkName: "admin" */'../pages/dashboard/DashboardPedidosPage.vue');
const ConsultarPedidoPage = () => import(/* webpackChunkName: "admin" */'../pages/consult/ConsultarPedidoPage.vue');
const CriarPedidoPage = () => import(/* webpackChunkName: "admin" */'../pages/create/CriarPedidoPage.vue');

const pedidosFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'pedidos',
        component: DashboardPedidosPage
    },
    {
        path: 'novo',
        name: 'novopedido',
        component: CriarPedidoPage
    },
    {
        path: 'consultar/:id?',
        name: 'consultarPedido',
        component: ConsultarPedidoPage
    },
    {
        path: 'editar/:id?',
        name: 'editarPedido',
        component: CriarPedidoPage
    }
];

export { pedidosFeatureRouter };