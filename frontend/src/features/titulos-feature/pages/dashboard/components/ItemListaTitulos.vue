<script lang="ts" setup>
import { PropType, computed } from 'vue';
import { RmTooltip, } from '@/components';
import * as icons from '../../../../../icons';

import TituloListaItemDTO from '@/core/dtos/titulo/TituloListaItemDTO';
import statusTituloEnumParser from '@/core/enums/status-titulo-enum-parser';
import { formatarDecimal } from '@/utils/number-functions';
import { mascaraCnpj, formatarData } from '@/utils/string-functions';
import { useAutorizacao } from '@/hooks/autorizacao';

const props = defineProps({
    title: {
        type: Object as PropType<TituloListaItemDTO>,
        required: true
    }
});
const { eGerente } = useAutorizacao();
const tipoPedido = computed(() => statusTituloEnumParser.get((props.title.status ?? '').toString().toUpperCase())!);
const icon = computed(() => icons[tipoPedido.value.icon]);

</script>

<template>
    <tr>
        <td>{{ title.titulo }}</td>
        <td>{{ title.parcela?.toString().padStart(3, '0') }}</td>
        <td>{{ title.pedido }}</td>
        <td class="!text-left">
            {{ title.nomeCliente }}
        </td>
        <td>{{ mascaraCnpj(title.cnpj ?? '') }}</td>
        <td>{{ title.dataVencimentoOriginal ? formatarData(title.dataVencimentoOriginal) : '-' }}</td>
        <td>{{ title.dataVencimento ? formatarData(title.dataVencimento) : '-' }}</td>
        <td>{{ title.dataPagamento ? formatarData(title.dataPagamento) : '-' }}</td>
        <td>R$ {{ formatarDecimal(title.valorTitulo ?? 0) }}</td>
        <td v-if="eGerente">
            <RmTooltip :text="title.nomeRepresentante ?? ''">
                {{ title.representante }}
            </RmTooltip>
        </td>
        <td>
            <RmTooltip :text="statusTituloEnumParser.get((title.status ?? '').toString())!.titulo">
                <component :is="icon"
                           :style="{'fill': tipoPedido.color}"
                           class="w-6 mx-auto" />
            </RmTooltip>
        </td>
    </tr>
</template>