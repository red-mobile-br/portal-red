<script lang="ts" setup>
import { PropType, computed } from 'vue';

// Components
import { RmDropdown, RmDropdownItem, RmTooltip, RmIconButton } from '@/components';
import * as icons from '@/icons';

// Domain
import PedidoListaItemDTO from '@/core/dtos/pedido/PedidoListaItemDTO';
import statusPedidoEnumParser from '@/core/enums/status-pedido-enum-parser';

// Helpers and functions
import { mascaraCnpj, formatarData } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';

// Hooks
import { useAutorizacao } from '@/hooks/autorizacao';

const props = defineProps({
    order: {
        type: Object as PropType<PedidoListaItemDTO>,
        required: true
    },
    isLast: {
        type: Boolean,
        default: false
    }
});

const emits = defineEmits(['on:select-invoice', 'on:select-slip']);

const tipoPedido = computed(() => statusPedidoEnumParser.get((props.order?.status ?? '').toUpperCase())!);
const icon = computed(() => icons[tipoPedido.value.icon]);
const canEdit = computed(() => ['0', 'M', 'R'].includes(props.order?.status ?? ''));

</script>

<template>
    <tr class="cursor-pointer">
        <td>{{ order?.id ?? '' }}</td>
        <td class="!text-left">
            {{ order?.nome ?? '' }}
        </td>
        <td>{{ mascaraCnpj(order?.cnpj ?? '') }}</td>
        <td>{{ formatarData(order?.dataLancamento ?? '') }}</td>
        <td>R$ {{ formatarDecimal(order?.valorTotal ?? 0) }}</td>
        <td>
            <RmTooltip :text="tipoPedido.titulo">
                <component :is="icon"
                           :style="{'fill': tipoPedido.color}"
                           class="w-6 mx-auto" />
            </RmTooltip>
        </td>
        <td>
            <RmDropdown class="w-6" :position="isLast ? 'top-right' : 'bottom-right'">
                <template #default>
                    <RmIconButton icon="EllipsisIcon"
                                  class="fill-current text-neutral-900 dark:text-white"
                                  size="w-6" 
                                  icon-class="w-5" />
                </template>
                <template #content>
                    <!-- Detalhes -->
                    <RmDropdownItem label="Ver detalhes" icon="ExternalIcon"
                                    @click="$router.replace({ name: 'consultarPedido', params: { id: order?.id ?? '' } })" />

                    <!-- Editar -->
                    <RmDropdownItem v-if="canEdit" label="Editar" icon="EditIcon"
                                    @click="$router.replace({ name: 'editarPedido', params: { id: order?.id ?? '' } })" />

                    <!-- Notas fiscais -->
                    <RmDropdownItem v-for="invoice in (order?.notasFiscais ?? [])"
                                    :key="invoice.codigo" :label="'NF ' + invoice"
                                    icon="InvoiceIcon"
                                    @click="emits('on:select-invoice', {orderId: order?.id ?? '', invoice })" />

                    <!-- Boletos -->
                    <RmDropdownItem v-for="paymentSlip in (order?.boletos ?? [])"
                                    :key="paymentSlip"
                                    :label="'Boleto ' + paymentSlip" icon="ComissoesIcon"
                                    @click="emits('on:select-slip', { orderId: order?.id ?? '', paymentSlip })" />
               
                </template>
            </RmDropdown>
        </td>
    </tr>
</template>