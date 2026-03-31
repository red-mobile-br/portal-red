import { RouteRecordRaw } from 'vue-router';
const MetaAtualPage = () => import(/* webpackChunkName: "admin" */'@/features/metas-feature/pages/meta-atual/MetaAtualPage.vue');
const HistoricoMetasPage = () => import(/* webpackChunkName: "admin" */'@/features/metas-feature/pages/historico-metas/HistoricoMetasPage.vue');

const metasFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'metas',
        component: MetaAtualPage
    },
    {
        path: 'historico',
        name: 'historico-metas',
        component: HistoricoMetasPage
    },
];
export { metasFeatureRouter };