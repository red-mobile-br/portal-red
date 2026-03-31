import { RouteRecordRaw } from "vue-router";
const DashboardTitulosPage = () => import(/* webpackChunkName: "admin" */'../pages/dashboard/DashboardTitulosPage.vue');

const titulosFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'titulos',
        component: DashboardTitulosPage
    }
];

export { titulosFeatureRouter };