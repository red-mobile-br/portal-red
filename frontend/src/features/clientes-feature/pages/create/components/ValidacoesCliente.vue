<script lang="ts" setup>
import { ClienteDetalhadoDTO } from '@/core/dtos/cliente/ClienteDetalhadoDTO';
import { useFormStatus } from '../hooks/form-status';

import { PropType } from 'vue';
import CampoValidacaoCliente from './CampoValidacaoCliente.vue';

const props = defineProps({
    form: {
        type: Object as PropType<ClienteDetalhadoDTO>,
        required: true
    },
    validate: {
        type: Boolean,
        default: true,
    },
    receipts: {
        type: Array,
        required: true,
    },
    socialContractDocument: {
        type: Array,
        required: true,
    },
    tradeNotes: {
        type: Array,
        required: true,
    },
    sintegraDocument: {
        type: Array,
        required: true,
    },
});

const {   
    customerDataStatus,
    contactsDataStatus,
    billingAddressStatus,
    partnersStatus,
    shippingStatus,
    referencesStatus,
} = useFormStatus(props.form);

</script>

<template>
    <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 2xl:grid-cols-1 p-4">
        <CampoValidacaoCliente :status="customerDataStatus">
            Dados do cliente
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="contactsDataStatus">
            Contatos
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="billingAddressStatus">
            Endereço de faturamento
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="partnersStatus">
            Sócios
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="shippingStatus">
            Dados da entrega
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="referencesStatus">
            Referências comerciais
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="socialContractDocument.length ? 'done' : 'pending'">
            Cópia de contrato social
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="sintegraDocument.length ? 'done' : 'pending'">
            Cópia da tela do sintegra
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="receipts.length ? 'done' : 'pending'">
            Cópia de NF's
        </CampoValidacaoCliente>

        <CampoValidacaoCliente :status="tradeNotes.length ? 'done' : 'pending'">
            Cópia de duplicatas pagas
        </CampoValidacaoCliente>
    </div>
</template>