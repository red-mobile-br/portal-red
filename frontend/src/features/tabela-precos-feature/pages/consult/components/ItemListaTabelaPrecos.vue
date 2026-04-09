<script lang="ts" setup>
import { PropType } from 'vue';
import ProdutoTabelaPrecosDTO from '@/core/dtos/tabelasPrecos/ProdutoTabelaPrecosDTO';
import { formatarDecimal } from '@/utils/number-functions';

defineProps({
    product: {
        type: Object as PropType<ProdutoTabelaPrecosDTO>,
        required: true
    },
});
</script>

<template>
    <tr>
        <td>{{ product.referencia }}</td>
        <td>
            <p class="text-left">
                {{ product.descricao }}
            </p>
        </td>
        <td>R$ {{ formatarDecimal(product.precoTabela ?? 0) }}</td>
        <td>{{ formatarDecimal(product.percentualIPI ?? 0) }}%</td>
    </tr>
</template>
