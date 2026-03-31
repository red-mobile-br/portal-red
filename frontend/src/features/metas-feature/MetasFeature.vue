<script lang="ts" setup>
import RmAdminPage from '../../components/RmAdminPage.vue';
import RmTab from '../../components/RmTab.vue';
import RmText from '../../components/RmText.vue';

</script>

<template>
    <RmAdminPage class="flex flex-col">

        <!-- Metas -->
        <RmText type="display-medium" class="mb-2">
            Metas
        </RmText>

        <!-- Tabs -->
        <div class="border-b border-gray-300 dark:border-gray-600 mb-4">
            <RmTab label="Meta atual" 
                   :selected="$route.name == 'metas'" 
                   @click="$router.replace({ name: 'metas' })" />
            <RmTab label="Histórico de metas" 
                   :selected="$route.name == 'historico-metas'" 
                   @click="$router.replace({ name: 'historico-metas' })" />
        </div>

        <!-- Conteúdo -->
        <RouterView v-slot="{ Component }">
            <transition name="fade" mode="out-in">
                <component :is="Component" />
            </transition>
        </RouterView>

    </RmAdminPage>
</template>