<script lang="ts" setup>
import ComissaoListaItemDTO from '@/core/dtos/comissoes/ComissaoListaItemDTO';
import { PropType } from 'vue';
import { formatarData } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';
import { useAutorizacao } from '@/hooks/autorizacao';
import { RmTooltip } from '@/components';

defineProps({
    commission: {
        type: Object as PropType<ComissaoListaItemDTO>,
        required: true
    }
});

const { eGerente } = useAutorizacao();
</script>

<template>
    <tr>
        <td>{{ commission.pedido }}</td>
        <td class="!text-left">
            {{ commission.nomeCliente }}
        </td>
        <td>{{ commission.titulo }}</td>
        <td>{{ commission.parcela }}</td>
        <td>{{ commission.dataVencimento ? formatarData(commission.dataVencimento) : '-' }}</td>
        <td>{{ commission.dataBaixa ? formatarData(commission.dataBaixa) : '-' }}</td>
        <td>R$ {{ formatarDecimal(commission.valorTitulo) }}</td>
        <td>R$ {{ formatarDecimal(commission.valorBase) }}</td>
        <td>{{ formatarDecimal(commission.percentualComissao) }}%</td>
        <td v-if="eGerente">
            <RmTooltip :text="commission.nomeRepresentante">
                {{ commission.representante }}
            </RmTooltip>
        </td>
        <td>R$ {{ formatarDecimal(commission.valorComissao) }}</td>
    </tr>
</template>