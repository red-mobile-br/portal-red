<script lang="ts" setup>
import { computed, PropType } from 'vue';
import { formatarData } from '@/utils/string-functions';
import FaturamentoListaItemDTO from '@/core/dtos/faturamento/FaturamentoListaItemDTO';
import { formatarDecimal } from '@/utils/number-functions';
import { useAutorizacao } from '@/hooks/autorizacao';
import { RmTooltip } from '@/components';

const props = defineProps({
    revenue: {
        type: Object as PropType<FaturamentoListaItemDTO>,
        required: true
    }
});
const { eGerente } = useAutorizacao();

const date = computed(() => formatarData(props.revenue.dataHora ?? ''));
const orderValue = computed(() => formatarDecimal(props.revenue.valorPedido ?? 0));
const baseValue = computed(() => formatarDecimal(props.revenue.valorBase ?? 0));
</script>

<template>
    <tr>
        <td>{{ revenue.pedido }}</td>
        <td class="!text-left">
            {{ revenue.cliente }}
        </td>
        <td>{{ date }}</td>
        <td>{{ revenue.notaFiscal }}</td>
        <td>R$ {{ baseValue }}</td>
        <td>R$ {{ orderValue }}</td>
        <td v-if="eGerente">
            <RmTooltip :text="revenue.nomeRepresentante ?? ''">
                {{ revenue.representante }}
            </RmTooltip>
        </td>
    </tr>
</template>
