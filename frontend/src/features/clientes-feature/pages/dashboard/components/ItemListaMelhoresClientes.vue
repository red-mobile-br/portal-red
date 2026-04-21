<script lang="ts" setup>
import { PropType, computed } from 'vue';
import { formatarDecimal } from '@/utils/number-functions';
import MelhorClienteDTO from '@/core/dtos/cliente/MelhorClienteDTO';
import { trophyBronze, trophyGold, trophySilver, trophySmall } from '@/assets/img';

const props = defineProps({
    index: {
        type: Number,
        required: true
    },
    isRevenue: {
        type: Boolean,
        required: true
    },
    client: {
        type: Object as PropType<MelhorClienteDTO>,
        required: true
    }
});
const trophy = computed(() => {
    switch (props.index) {
        case 0:
            return trophyGold;
        case 1:
            return trophySilver;
        case 2:
            return trophyBronze;
        default:
            return trophySmall;
    }
});
</script>

<template>
    <tr class="cursor-pointer">
        <td>
            <div class="flex items-center">
                <img :src="trophy" class="w-6">
                <p class="px-2 text-left text-xs">
                    {{ client.nome ?? '' }}
                </p>
            </div>
        </td>
        <td class="font-bold opacity-80">
            {{ isRevenue ? `R$ ${formatarDecimal(client.valor ?? 0)}` : client.valor ?? 0 }}
        </td>
    </tr>
</template>