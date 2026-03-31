import { RouteRecordRaw } from "vue-router";
const ConsultarTabelaPage = () => import(/* webpackChunkName: "admin" */'../pages/consult/ConsultarTabelaPage.vue');

const tabelaPrecosFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'tabelaprecos',
        component: ConsultarTabelaPage
    }
];

export { tabelaPrecosFeatureRouter };