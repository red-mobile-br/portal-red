import { RouteRecordRaw } from 'vue-router';
const LoginPage = () => import (/* webpackChunkName: "auth" */'@/features/autenticacao-feature/pages/LoginPage.vue');
const RedefinirSenhaPage = () => import (/* webpackChunkName: "auth" */'@/features/autenticacao-feature/pages/RedefinirSenhaPage.vue');

export const autenticacaoFeatureRouter: RouteRecordRaw[] = [
    {
        path: '',
        name: 'login',
        meta: {
            isPublic: true
        },
        component: LoginPage
    },
    {
        path: 'redefinirsenha',
        name: 'redefinirSenha',
        meta: {
            isPublic: true
        },
        component: RedefinirSenhaPage
    }
];