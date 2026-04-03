<script lang="ts" setup>
import { computed, PropType } from 'vue';
import { RmTooltip } from '@/components';
import { useAutorizacao } from '@/hooks/autorizacao';
import ClienteListaItemDTO from '@/core/dtos/cliente/ClienteListaItemDTO';
import { formatarData, mascaraCnpj, formatarTitulo } from '@/utils/string-functions';

const props = defineProps({
    client: {
        type: Object as PropType<ClienteListaItemDTO>,
        required: true
    }
});

const { eGerente } = useAutorizacao();

const status = computed(() => {
    const allStatus: Record<number, string> = {
        0: 'Sem compras',
        1: 'Ativo',
        2: 'Inativo'
    };
    return allStatus[props.client.status ?? 0];
});
</script>

<template>
    <tr class="cursor-pointer">
        <td>{{ (client.id ?? '').substr(0,6) }}</td>
        <td>{{ (client.id ?? '').substr(6,2) }}</td>
        <td class="!text-left">
            {{ client.nome ?? '' }}
        </td>
        <td>{{ mascaraCnpj(client.cnpj ?? '') }}</td>
        <td>{{ formatarTitulo(client.cidade ?? '') }}</td>
        <td>{{ client.uf ?? '' }}</td>
        <td>{{ client.ultimaCompra ? formatarData(client.ultimaCompra) : '-' }}</td>
        <td>{{ client.diasSemComprar ?? 0 }}</td>
        <td>{{ status }}</td>
        <td v-if="eGerente">
            <RmTooltip :text="client.nomeRepresentante ?? ''">
                {{ client.idRepresentante ?? '' }}
            </RmTooltip>
        </td>
    </tr>
</template>