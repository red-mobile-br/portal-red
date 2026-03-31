import { RouteRecordRaw } from "vue-router";
const DashboardClientesPage = () => import(/* webpackChunkName: "customer" */'../pages/dashboard/DashboardClientesPage.vue');
const CadastrarClientePage = () => import(/* webpackChunkName: "customer" */'@/features/clientes-feature/pages/create/CadastrarClientePage.vue');
const ConsultarClientePage = () => import(/* webpackChunkName: "customer" */'@/features/clientes-feature/pages/consult/ConsultarClientePage.vue');
const clientesFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'clientes',
        component: DashboardClientesPage
    },
    {
        path: 'consultar/:cnpj?',
        name: 'consultar-cliente',
        component: ConsultarClientePage
    },
    {
        path: 'cadastrar',
        name: 'cadastrar-cliente',
        component: CadastrarClientePage
    },
];

export { clientesFeatureRouter };