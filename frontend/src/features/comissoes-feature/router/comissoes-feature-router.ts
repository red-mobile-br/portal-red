import { RouteRecordRaw } from "vue-router";
const DashboardComissoesPage = () => import(/* webpackChunkName: "admin" */'../pages/dashboard/DashboardComissoesPage.vue');

const comissoesFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'comissoes',
        component: DashboardComissoesPage
    }
];

export { comissoesFeatureRouter };