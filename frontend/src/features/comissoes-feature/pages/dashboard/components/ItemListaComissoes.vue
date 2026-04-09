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
        <td>{{ commission?.numeroPedido ?? '' }}</td>
        <td class="!text-left">
            {{ commission?.nomeCliente ?? '' }}
        </td>
        <td>{{ commission?.numeroTitulo ?? '' }}</td>
        <td>{{ commission?.parcela ?? '' }}</td>
        <td>{{ commission?.dataVencimento ? formatarData(commission.dataVencimento) : '-' }}</td>
        <td>{{ commission?.dataBaixa ? formatarData(commission.dataBaixa) : '-' }}</td>
        <td>R$ {{ formatarDecimal(commission?.valorTitulo ?? 0) }}</td>
        <td>R$ {{ formatarDecimal(commission?.valorBase ?? 0) }}</td>
        <td>{{ formatarDecimal(commission?.percentualComissao ?? 0) }}%</td>
        <td v-if="eGerente">
            <RmTooltip :text="commission?.nomeRepresentante ?? ''">
                {{ commission?.idRepresentante ?? '' }}
            </RmTooltip>
        </td>
        <td>R$ {{ formatarDecimal(commission?.valorComissao ?? 0) }}</td>
    </tr>
</template>