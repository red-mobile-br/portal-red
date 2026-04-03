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
        <td>{{ product.referenciaComercial }}</td>
        <td>
            <p class="text-left">
                {{ product.descricao }}
            </p>
        </td>
        <td>R$ {{ formatarDecimal(product.precoMercado ?? 0) }}</td>
        <td>{{ formatarDecimal(product.percentualIPI ?? 0) }}%</td>
    </tr>
</template>
