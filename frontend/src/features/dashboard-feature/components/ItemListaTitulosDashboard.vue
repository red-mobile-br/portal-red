<script lang="ts" setup>
import { PropType } from 'vue';
import TituloListaItemDTO from '@/core/dtos/titulo/TituloListaItemDTO';

defineProps({
    title: {
        type: Object as PropType<TituloListaItemDTO>,
        required: true
    }
});

</script>

<template>
    <tr>
        <td>{{ title.titulo }}</td>

        <td class="!text-left">
            {{ title.nomeCliente }}
        </td>
     
    </tr>
</template>