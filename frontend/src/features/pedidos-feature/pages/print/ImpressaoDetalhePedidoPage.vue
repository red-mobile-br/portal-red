<script lang="ts" setup>
import { onMounted } from 'vue';
import useAppStore from '@/store/app-store';

import {
    RmPrintView,
    RmPrintPage,
    RmText, 
    RmTextField,
    RmDivider 
} from '@/components';
import { useRoute } from 'vue-router';
import { useDetalheProduto } from '@/hooks/detalhe-produto';
import { formatarData, mascaraCnpj, mascaraTelefone } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';

const route = useRoute();
const {
    state,
    obterDetalhePedido,
    interpretarModoFrete,
    statusPedido,
    tipoPedido,
    planoPagamento,
    totalVolumes,
    totalProdutos, 
    totalPesoBruto, 
    totalPesoLiquido
} = useDetalheProduto();
const { nomeUsuario } = useAppStore();

onMounted(() => {
    state.numeroPedido = route.params.id.toString();
    obterDetalhePedido({ isValid: true });
});
</script>

<template>
    <RmPrintView>
        <RmPrintPage v-if="!state.carregando && state.pedidoSelecionado" title="Relatório de pedido">
              
            <div class="w-full">
                        
                <div class="flex items-stretch">
                    <div>
                        <RmText type="headline-small" class="flex-1">
                            Código do Pedido
                        </RmText>
                        <RmText type="display-large">
                            {{ state.pedidoSelecionado.id }}
                        </RmText>
                    </div>
                    <div class="mx-5 w-px bg-gray-300" />
                    <div>
                        <RmText type="headline-small" class="flex-1">
                            Tipo:
                        </RmText>
                        <RmText type="display-large">
                            {{ tipoPedido }}
                        </RmText>
                    </div>
                    <div class="mx-5 w-px bg-gray-300" />
                    <div>
                        <RmText type="headline-small" class="flex-1">
                            Status:
                        </RmText>
                        <RmText type="display-large">
                            {{ statusPedido }}
                        </RmText>
                    </div>
                </div>

                <RmDivider class="my-5" />
                <RmText type="headline-small" class="mb-4">
                    Representante
                </RmText>
                <RmTextField>{{ nomeUsuario }}</RmTextField>

                <RmDivider class="my-5" />

                <!-- Conteúdo -->
                <RmText type="headline-small" class="mb-4">
                    Dados do cliente
                </RmText>

                <!-- Dados do cliente -->
                <div class="grid grid-cols-3 gap-3">
                    <RmTextField label="Identificador">
                        {{ state.pedidoSelecionado.cliente.id }}
                    </RmTextField>
                    <RmTextField label="Razão social" class="col-span-2">
                        {{ state.pedidoSelecionado.cliente.razaoSocial }}
                    </RmTextField>
                    <RmTextField label="CNPJ">
                        {{ mascaraCnpj(state.pedidoSelecionado.cliente.cnpj) }}
                    </RmTextField>
                    <RmTextField label="Nome fantasia" class="col-span-2">
                        {{ state.pedidoSelecionado.cliente.nomeFantasia }}
                    </RmTextField>
                    <RmTextField label="Cep">
                        {{ state.pedidoSelecionado.cliente.endereco.cep }}
                    </RmTextField>
                    <RmTextField label="Logradouro" class="col-span-2">
                        {{ state.pedidoSelecionado.cliente.endereco.logradouro }}
                    </RmTextField>
                    <RmTextField label="Número">
                        {{ state.pedidoSelecionado.cliente.endereco.numero }}
                    </RmTextField>
                    <RmTextField label="Complemento" class="col-span-2">
                        {{ state.pedidoSelecionado.cliente.endereco.complemento || "-" }}
                    </RmTextField>
                    <RmTextField label="Bairro">
                        {{ state.pedidoSelecionado.cliente.endereco.bairro }}
                    </RmTextField>
                    <RmTextField label="Município">
                        {{ state.pedidoSelecionado.cliente.endereco.cidade }}
                    </RmTextField>
                    <RmTextField label="Estado">
                        {{ state.pedidoSelecionado.cliente.endereco.estado }}
                    </RmTextField>
                </div>

                <RmDivider class="my-5" />

                <!-- ========== TIPO DE FRETE =========== -->
                <RmText type="headline-small" class="mb-4">
                    Entrega
                </RmText>

                <!-- Tipo de frete -->
                <div class="max-w-xs mb-5">
                    <RmTextField label="Tipo de frete">
                        {{ interpretarModoFrete(state.pedidoSelecionado.modoFrete ) }}
                    </RmTextField>
                </div>

                <RmDivider class="my-5" />
                <template v-if="state.pedidoSelecionado.enderecoEntrega">
                    <!-- Endereço de entrega -->
                    <RmText type="headline-small" class="mb-4">
                        Endereço de entrega
                    </RmText>
                    <div class="grid grid-cols-3 gap-3">
                        <RmTextField label="Cep">
                            {{ state.pedidoSelecionado.enderecoEntrega.cep }}
                        </RmTextField>
                        <RmTextField label="Logradouro" class="col-span-2">
                            {{ state.pedidoSelecionado.enderecoEntrega.logradouro }}
                        </RmTextField>
                        <RmTextField label="Número">
                            {{ state.pedidoSelecionado.enderecoEntrega.numero }}
                        </RmTextField>
                        <RmTextField label="Complemento" class="col-span-2">
                            {{ state.pedidoSelecionado.enderecoEntrega.complemento || '-' }}
                        </RmTextField>
                        <RmTextField label="Bairro">
                            {{ state.pedidoSelecionado.enderecoEntrega.bairro }}
                        </RmTextField>
                        <RmTextField label="Cidade">
                            {{ state.pedidoSelecionado.enderecoEntrega.cidade }}
                        </RmTextField>
                        <RmTextField label="Estado">
                            {{ state.pedidoSelecionado.enderecoEntrega.estado }}
                        </RmTextField>
                    </div>
                    <RmDivider class="my-5" />
                </template>

                <template v-if="state.pedidoSelecionado.dadosAgendamento">
                    <RmText type="headline-small" class="mb-4">
                        Dados do agendamento
                    </RmText>

                    <div class="flex-1 overflow-auto light-scroll">
                        <div class="grid grid-cols-2 gap-3">
                            <RmTextField label="E-mail do destinatário" class="mb-4">
                                {{ state.pedidoSelecionado.dadosAgendamento.email }}
                            </RmTextField>
                            <RmTextField label="Telefone para contato">
                                {{ mascaraTelefone(state.pedidoSelecionado.dadosAgendamento.telefone) }}
                            </RmTextField>
                        </div>
                    </div>

                    <RmDivider class="my-5" />
                </template>

                <!-- Produtos -->
                <RmText type="headline-small" class="mb-4">
                    Produtos
                </RmText>

                <table class="red-mobile-table --ultra-small">
                    <tr>
                        <th class="w-6">
                            Item
                        </th>
                        <th class="w-12">
                            Cód.
                        </th>
                        <th class="text-left">
                            Produto
                        </th>
                        <th class="w-12">
                            Qtd
                        </th>
                        <th class="w-18">
                            Valor Unit.
                        </th>
                        <th class="w-14">
                            IPI
                        </th>
                        <th class="w-14">
                            ICMS
                        </th>
                        <th class="w-14">
                            ICMS ST
                        </th>
                        <th class="w-18">
                            Total
                        </th>
                    </tr>
                    <tr v-for="(product, index) in state.pedidoSelecionado.produtos" :key="index">
                        <td>{{ index + 1 }}</td>
                        <td>{{ product.id }}</td>
                        <td class="!text-left">
                            {{ product.descricao }}
                        </td>
                        <td>{{ product.quantidade }}</td>
                        <td>R$ {{ formatarDecimal(product.valorUnitario) }}</td>
                        <td>{{ formatarDecimal(product.percentualIPI ?? 0) }}%</td>
                        <td>{{ formatarDecimal(product.percentualICMS ?? 0) }}%</td>
                        <td>{{ formatarDecimal(product.percentualICMSST ?? 0) }}%</td>
                        <td>R$ {{ formatarDecimal(product.valorTotal) }}</td>
                    </tr>
                </table>

                <RmDivider class="mb-5" />

                <!-- ============== FATURAMENTO =========-->
                <RmText type="headline-small" class="mb-4">
                    Fiscal e faturamento
                </RmText>

                <div class="flex-1 overflow-auto light-scroll">
                    <div class="grid grid-cols-3 gap-3">
                        <RmTextField label="Número da OC" class="mb-4">
                            {{ state.pedidoSelecionado.numeroPedidoCompra || '-' }}
                        </RmTextField>
                        <RmTextField label="Condição de pagamento" class="mb-4">
                            {{ planoPagamento }}
                        </RmTextField>
                        <RmTextField label="Data do faturamento" class="mb-4">
                            {{ formatarData(state.pedidoSelecionado.dataLancamento) }}
                        </RmTextField>
                    </div>
                </div>

                <RmTextField label="Informações para a NF" class="mb-4">
                    {{ state.pedidoSelecionado.informacoesNota || '-' }}
                </RmTextField>

                <RmTextField label="Observações do pedido">
                    {{ state.pedidoSelecionado.observacoesPedido || '-' }}
                </RmTextField>

                <RmDivider class="my-5" />

                <!-- ============== FATURAMENTO =========-->
                <RmText type="headline-small" class="mb-4">
                    Totais
                </RmText>

                <div class="flex-1 overflow-auto light-scroll">
                    <div class="grid grid-cols-3 gap-3">
                        <RmTextField label="Data de emissão">
                            {{ formatarData(state.pedidoSelecionado.dataEmissao) }}
                        </RmTextField>
                        <RmTextField label="Total de items">
                            {{ state.pedidoSelecionado.produtos.length }}
                        </RmTextField>
                        <RmTextField label="Total de peças">
                            {{ totalVolumes }}
                        </RmTextField>
                        <RmTextField label="Total produtos">
                            R$ {{ totalProdutos }}
                        </RmTextField>
                        <RmTextField label="Total ICMS">
                            R$ {{ formatarDecimal(state.pedidoSelecionado.valorICMS) }}
                        </RmTextField>
                        <RmTextField label="Total IPI">
                            R$ {{ formatarDecimal(state.pedidoSelecionado.valorIPI) }}
                        </RmTextField>
                        <RmTextField label="Total ICMS ST">
                            R$ {{ formatarDecimal(state.pedidoSelecionado.valorICMSST) }}
                        </RmTextField>
                        <RmTextField label="Peso líquido">
                            {{ totalPesoLiquido }} kg
                        </RmTextField>
                        <RmTextField label="Peso bruto">
                            {{ totalPesoBruto }} kg
                        </RmTextField>
                    </div>
                </div>
            </div>
        </RmPrintPage>
    </RmPrintView>
</template>