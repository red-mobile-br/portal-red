import { RouteRecordRaw } from "vue-router";
const DashboardFaturamentoPage = () => import('../pages/dashboard/DashboardFaturamentoPage.vue');

function faturamentoFeatureRouter(): RouteRecordRaw[] {
    return [
        {
            path: '',
            name: 'faturamento',
            component: DashboardFaturamentoPage
        }
    ];
}

export { faturamentoFeatureRouter };