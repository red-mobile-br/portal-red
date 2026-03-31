<script lang="ts" setup>
import { reactive } from 'vue';
import { useRouter } from 'vue-router';

import AdminHeader from './components/AdminHeader.vue';
import ProfileButton from './components/ProfileButton.vue';
import RmDropdown from '../../components/RmDropdown.vue';
import RmDropdownItem from '../../components/RmDropdownItem.vue';
import MenuItem from './components/MenuItem.vue';

import ConstantesArmazenamento from '../../utils/constantes-armazenamento';
import RmDarkModeToogle from '../../components/RmDarkModeToogle.vue';
import ToogleMenuButton from './components/ToogleMenuButton.vue';
import useAppStore from '../../store/app-store';
import { useAutorizacao } from "@/hooks/autorizacao";

const router = useRouter();
const { nomeUsuario } = useAppStore();
const { eAdmin } = useAutorizacao();

const state = reactive({
    menuOpened: false,
    menuContracted: false,
});

const navigate = (route: string) => {
    state.menuOpened = false;
    router.replace({ name: route });
};

const logout = () => {
    localStorage.removeItem(ConstantesArmazenamento.tokenAcesso);
    localStorage.removeItem(ConstantesArmazenamento.expiraEm);
    router.replace({ name: 'login' });
};
</script>


<template>
    <div class="min-h-screen flex flex-col items-stretch w-full pt-[3.5rem] ">

        <!-- Header -->
        <AdminHeader @menu:pressed="() => state.menuOpened = !state.menuOpened">
            <RmDarkModeToogle />

            <!-- Usuário -->
            <RmDropdown>
                <template #default>
                    <ProfileButton :nome-usuario="nomeUsuario" />
                </template>
                <template #content>
                
                    <RmDropdownItem icon="SignOutIcon"
                                    label="Sair"
                                    @click="logout" />
                </template>
            </RmDropdown>
        </AdminHeader>


        <!-- Menu -->
        <nav class="fixed left-0 top-20 bottom-0 flex flex-col items-stretch transition-all duration-300 z-0 w-menu"
             :class="{'md:w-menu-contract': state.menuContracted}">
            <div class="flex-1">
                <MenuItem :contracted="state.menuContracted"
                          icon="DashboardIcon"
                          label="Dashboard" 
                          :selected="$route.name == 'dashboard'"
                          @click="() => navigate('dashboard')" />

                <MenuItem :contracted="state.menuContracted"
                          icon="PedidosIcon"
                          label="Pedidos"
                          :selected="$route.path.includes('pedidos')"
                          @click="() => navigate('pedidos')" />

                <!--                <MenuItem v-if="eAdmin"-->
                <!--                          :contracted="state.menuContracted"-->
                <!--                          icon="AbacusIcon"-->
                <!--                          label="Orçamentos"-->
                <!--                          :selected="$route.path.includes('orcamentos')"-->
                <!--                          @click="() => navigate('orcamentos')" />-->

                <MenuItem :contracted="state.menuContracted"
                          icon="FaturamentoIcon"
                          label="Faturamento"
                          :selected="$route.path.includes('faturamento')"
                          @click="() => navigate('faturamento')" />

                <MenuItem :contracted="state.menuContracted"
                          icon="ComissoesIcon"
                          label="Comissões"
                          :selected="$route.path.includes('comissoes')"
                          @click="() => navigate('comissoes')" />

                <MenuItem :contracted="state.menuContracted"
                          icon="TitulosIcon"
                          label="Títulos"
                          :selected="$route.path.includes('titulos')"
                          @click="() => navigate('titulos')" />

                <MenuItem :contracted="state.menuContracted"
                          :selected="$route.path.includes('clientes')"
                          icon="ClientesIcon"
                          label="Clientes"
                          @click="() => navigate('clientes')" />

                <MenuItem :contracted="state.menuContracted"
                          icon="TabelaPrecosIcon"
                          label="Tabela de preços"
                          :selected="$route.path.includes('tabelaprecos')"
                          @click="() => navigate('tabelaprecos')" />

                <MenuItem :contracted="state.menuContracted"
                          icon="MetasIcon"
                          label="Metas"
                          :selected="$route.path.includes('metas')"
                          @click="() => navigate('metas')" />
            </div>
            <ToogleMenuButton :contracted="state.menuContracted"
                              class="invisible lg:visible" 
                              @click="() => state.menuContracted = !state.menuContracted" />
        </nav>

        <!-- Container principal -->
        <div class="flex-1 transition-all duration-300 relative z-10 bg-neutral-200 dark:bg-gray-800 flex flex-col items-stretch"
             :class="[
                 state.menuContracted ? 'lg:!ml-menu-contracted' : 'lg:ml-menu',
                 state.menuOpened ? 'max-lg:translate-x-menu': '']">
            <RouterView v-slot="{Component}">
                <transition name="fade"
                            mode="out-in">
                    <component :is="Component" />
                </transition>
            </RouterView>
        </div>
    </div>
</template>