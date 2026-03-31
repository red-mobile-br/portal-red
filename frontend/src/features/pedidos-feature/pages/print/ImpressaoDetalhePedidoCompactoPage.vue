<script lang="ts" setup>
import { onMounted, computed, watch } from 'vue';
import useAppStore from '@/store/app-store';
import { RmPrintView, RmPrintPage } from '@/components';
import { useRoute } from 'vue-router';
import { useDetalheProduto } from '@/hooks/detalhe-produto';
import { formatarData, mascaraCnpj, mascaraTelefone, mascaraCep, mascaraCodigoCliente } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';
import statusPedidoEnumParser from '@/core/enums/status-pedido-enum-parser';

const route = useRoute();
const {
    state,
    obterDetalhePedido,
    interpretarModoFrete,
    tipoPedido,
    planoPagamento,
    totalVolumes,
    totalProdutos,
    totalPesoBruto,
    totalPesoLiquido
} = useDetalheProduto();
const { nomeUsuario } = useAppStore();

const statusInfo = computed(() => {
    const status = state.pedidoSelecionado?.status || '';
    return statusPedidoEnumParser.get(status) || { titulo: 'Desconhecido', color: '#9E9E9E' };
});

watch(() => state.pedidoSelecionado, (order) => {
    if (order?.id) {
        document.title = `Portal Red - Pedido #${order.id}`;
    }
});

onMounted(() => {
    state.numeroPedido = route.params.id.toString();
    obterDetalhePedido({ isValid: true });
});
</script>

<template>
    <RmPrintView>
        <RmPrintPage v-if="!state.carregando && state.pedidoSelecionado">
            <div class="w-full compact-print">

                <!-- Header -->
                <div class="header">
                    <span class="order-number">Pedido #{{ state.pedidoSelecionado.id }}</span>
                </div>

                <!-- Info Bar -->
                <div class="info-bar">
                    <div class="info-item">
                        <span class="info-label">Tipo:</span>
                        <span class="info-value">{{ tipoPedido }}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Status:</span>
                        <span class="status-badge" :style="{ backgroundColor: statusInfo.color }">{{ statusInfo.titulo }}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Emissão:</span>
                        <span class="info-value">{{ formatarData(state.pedidoSelecionado.dataEmissao) }}</span>
                    </div>
                    <div class="info-item">
                        <span class="info-label">Representante:</span>
                        <span class="info-value">{{ nomeUsuario }}</span>
                    </div>
                </div>

                <!-- Cliente -->
                <div class="section">
                    <h3 class="section-title">Cliente</h3>
                    <div class="section-content">
                        <div class="row">
                            <div class="field w-28"><span class="field-label"><b>Código:</b></span><span class="field-value">{{ mascaraCodigoCliente(state.pedidoSelecionado.cliente.id) }}</span></div>
                            <div class="field"><span class="field-label"><b>CNPJ:</b></span><span class="field-value">{{ mascaraCnpj(state.pedidoSelecionado.cliente.cnpj) }}</span></div>
                            <div class="field flex-1 ml-4"><span class="field-label"><b>Razão Social:</b></span><span class="field-value">{{ state.pedidoSelecionado.cliente.razaoSocial }}</span></div>
                        </div>
                        <div class="row">
                            <div class="field flex-1"><span class="field-label"><b>Fantasia:</b></span><span class="field-value">{{ state.pedidoSelecionado.cliente.nomeFantasia }}</span></div>
                        </div>
                        <div class="row">
                            <div class="field w-24"><span class="field-label"><b>CEP:</b></span><span class="field-value">{{ mascaraCep(state.pedidoSelecionado.cliente.endereco.cep) }}</span></div>
                            <div class="field flex-1">
                                <span class="field-label"><b>End:</b></span>
                                <span class="field-value">
                                    {{ state.pedidoSelecionado.cliente.endereco.logradouro }}, {{ state.pedidoSelecionado.cliente.endereco.numero }}
                                    <template v-if="state.pedidoSelecionado.cliente.endereco.complemento"> - {{ state.pedidoSelecionado.cliente.endereco.complemento }}</template>
                                </span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="field"><span class="field-label"><b>Bairro:</b></span><span class="field-value">{{ state.pedidoSelecionado.cliente.endereco.bairro }}</span></div>
                            <div class="field ml-4"><span class="field-label"><b>Cidade:</b></span><span class="field-value">{{ state.pedidoSelecionado.cliente.endereco.cidade }}</span></div>
                            <div class="field ml-4"><span class="field-label"><b>UF:</b></span><span class="field-value">{{ state.pedidoSelecionado.cliente.endereco.estado }}</span></div>
                        </div>
                    </div>
                </div>

                <!-- Agendamento e Faturamento -->
                <div class="section-row">
                    <div class="section half" v-if="state.pedidoSelecionado.dadosAgendamento?.email">
                        <h3 class="section-title">Agendamento</h3>
                        <div class="section-content">
                            <div class="field"><span class="field-label"><b>E-mail:</b></span><span class="field-value">{{ state.pedidoSelecionado.dadosAgendamento.email }}</span></div>
                            <div class="field"><span class="field-label"><b>Tel:</b></span><span class="field-value">{{ mascaraTelefone(state.pedidoSelecionado.dadosAgendamento.telefone) }}</span></div>
                        </div>
                    </div>
                    <div class="section half">
                        <h3 class="section-title">Faturamento</h3>
                        <div class="section-content">
                            <div class="field"><span class="field-label"><b>Frete:</b></span><span class="field-value">{{ interpretarModoFrete(state.pedidoSelecionado.modoFrete) }}</span></div>
                            <div class="field"><span class="field-label"><b>Prazo:</b></span><span class="field-value">{{ planoPagamento }}</span></div>
                            <div class="field"><span class="field-label"><b>Data:</b></span><span class="field-value">{{ formatarData(state.pedidoSelecionado.dataLancamento) }}</span></div>
                            <div class="field"><span class="field-label"><b>OC:</b></span><span class="field-value">{{ state.pedidoSelecionado.numeroPedidoCompra || '-' }}</span></div>
                        </div>
                    </div>
                </div>

                <!-- Endereço de Entrega -->
                <div class="section" v-if="state.pedidoSelecionado.enderecoEntrega?.cep">
                    <h3 class="section-title">Endereço de Entrega</h3>
                    <div class="section-content">
                        <div class="row">
                            <div class="field w-24"><span class="field-label"><b>CEP:</b></span><span class="field-value">{{ mascaraCep(state.pedidoSelecionado.enderecoEntrega.cep) }}</span></div>
                            <div class="field flex-1">
                                <span class="field-label"><b>End:</b></span>
                                <span class="field-value">
                                    {{ state.pedidoSelecionado.enderecoEntrega.logradouro }}, {{ state.pedidoSelecionado.enderecoEntrega.numero }}
                                    <template v-if="state.pedidoSelecionado.enderecoEntrega.complemento"> - {{ state.pedidoSelecionado.enderecoEntrega.complemento }}</template>
                                </span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="field"><span class="field-label"><b>Bairro:</b></span><span class="field-value">{{ state.pedidoSelecionado.enderecoEntrega.bairro }}</span></div>
                            <div class="field ml-4"><span class="field-label"><b>Cidade:</b></span><span class="field-value">{{ state.pedidoSelecionado.enderecoEntrega.cidade }}</span></div>
                            <div class="field ml-4"><span class="field-label"><b>UF:</b></span><span class="field-value">{{ state.pedidoSelecionado.enderecoEntrega.estado }}</span></div>
                        </div>
                    </div>
                </div>

                <!-- Produtos -->
                <div class="section">
                    <h3 class="section-title">Produtos ({{ state.pedidoSelecionado.produtos.length }} itens)</h3>
                    <table class="products-table">
                        <thead>
                            <tr>
                                <th style="width: 22px;">#</th>
                                <th style="width: 80px;">Cód</th>
                                <th class="text-left">Descrição</th>
                                <th style="width: 32px;">Qtd</th>
                                <th class="text-right" style="width: 72px;">Unit.</th>
                                <th style="width: 32px;">IPI</th>
                                <th style="width: 38px;">ICMS</th>
                                <th style="width: 32px;">ST</th>
                                <th class="text-right" style="width: 85px;">Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(product, index) in state.pedidoSelecionado.produtos" :key="index">
                                <td>{{ index + 1 }}</td>
                                <td>{{ product.id }}</td>
                                <td class="text-left">{{ product.descricao }}</td>
                                <td>{{ product.quantidade }}</td>
                                <td class="text-right">R$ {{ formatarDecimal(product.valorUnitario) }}</td>
                                <td>{{ formatarDecimal(product.percentualIPI ?? 0) }}%</td>
                                <td>{{ formatarDecimal(product.percentualICMS ?? 0) }}%</td>
                                <td>{{ formatarDecimal(product.percentualICMSST ?? 0) }}%</td>
                                <td class="text-right">R$ {{ formatarDecimal(product.valorTotal) }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <!-- Totais e Resumo Financeiro -->
                <div class="section-row">
                    <div class="section half">
                        <h3 class="section-title">Totais</h3>
                        <div class="section-content">
                            <div class="totals-grid">
                                <div class="total-item"><span class="field-label"><b>Itens:</b></span><span class="field-value">{{ state.pedidoSelecionado.produtos.length }}</span></div>
                                <div class="total-item"><span class="field-label"><b>Peças:</b></span><span class="field-value">{{ totalVolumes }}</span></div>
                                <div class="total-item"><span class="field-label"><b>P.Líq:</b></span><span class="field-value">{{ totalPesoLiquido }}kg</span></div>
                                <div class="total-item"><span class="field-label"><b>P.Bruto:</b></span><span class="field-value">{{ totalPesoBruto }}kg</span></div>
                            </div>
                        </div>
                    </div>
                    <div class="financial-section">
                        <h3 class="section-title financial-title">Resumo Financeiro</h3>
                        <div class="financial-grid">
                            <div class="financial-taxes">
                                <span class="taxes-label">Impostos</span>
                                <div class="taxes-grid">
                                    <div class="tax-item"><span class="tax-name">ICMS:</span><span class="tax-value">R$ {{ formatarDecimal(state.pedidoSelecionado.valorICMS) }}</span></div>
                                    <div class="tax-item"><span class="tax-name">ST:</span><span class="tax-value">R$ {{ formatarDecimal(state.pedidoSelecionado.valorICMSST) }}</span></div>
                                    <div class="tax-item"><span class="tax-name">IPI:</span><span class="tax-value">R$ {{ formatarDecimal(state.pedidoSelecionado.valorIPI) }}</span></div>
                                </div>
                            </div>
                            <div class="financial-item large"><span class="financial-label">Produtos</span><span class="financial-value">R$ {{ totalProdutos }}</span></div>
                            <div class="financial-item large total"><span class="financial-label">Total</span><span class="financial-value">R$ {{ formatarDecimal(state.pedidoSelecionado.valorNota) }}</span></div>
                        </div>
                    </div>
                </div>

                <!-- Observações -->
                <div class="section" v-if="state.pedidoSelecionado.informacoesNota || state.pedidoSelecionado.observacoesPedido">
                    <h3 class="section-title">Observações</h3>
                    <div class="section-content">
                        <div v-if="state.pedidoSelecionado.informacoesNota" class="notes-block">
                            <span class="field-label"><b>NF:</b></span>
                            <p class="notes-text">{{ state.pedidoSelecionado.informacoesNota }}</p>
                        </div>
                        <div v-if="state.pedidoSelecionado.observacoesPedido" class="notes-block">
                            <span class="field-label"><b>Pedido:</b></span>
                            <p class="notes-text">{{ state.pedidoSelecionado.observacoesPedido }}</p>
                        </div>
                    </div>
                </div>

            </div>
        </RmPrintPage>
    </RmPrintView>
</template>

<style scoped>
/* ========== UNIFIED STYLES (screen and print) ========== */
.compact-print {
    font-family: Arial, sans-serif;
    font-size: 12px;
    line-height: 1.4;
    color: #333;
    padding: 0;
    margin: 0;
}

/* Header */
.header {
    display: flex;
    justify-content: flex-start;
    align-items: center;
    background: linear-gradient(135deg, #e53935 0%, #c62828 100%);
    padding: 12px 16px;
    color: white;
}
.order-number { font-size: 19px; font-weight: 700; }

/* Info Bar */
.info-bar {
    background: #f5f5f5;
    border-bottom: 1px solid #ddd;
    padding: 8px 16px;
    display: flex;
    gap: 20px;
    align-items: center;
    margin-bottom: 10px;
}
.info-item { display: flex; align-items: center; gap: 6px; }
.info-label { font-size: 10px; color: #666; text-transform: uppercase; }
.info-value { font-size: 12px; font-weight: 600; }
.status-badge {
    padding: 3px 10px;
    border-radius: 10px;
    font-size: 10px;
    font-weight: 600;
    color: white;
}

/* Sections */
.section {
    background: #f8f8f8;
    border-radius: 4px;
    margin-bottom: 10px;
    overflow: hidden;
}
.section-row { display: flex; gap: 10px; margin-bottom: 10px; }
.section.half { flex: 1; margin-bottom: 0; }
.section-title {
    font-size: 12px;
    font-weight: 700;
    text-transform: uppercase;
    color: #333;
    padding: 6px 10px;
    margin: 0;
    border-bottom: 2px solid #e53935;
    background: #fff;
}
.section-content { padding: 8px 10px; }

/* Rows and Fields */
.row { display: flex; gap: 12px; margin-bottom: 4px; }
.row:last-child { margin-bottom: 0; }
.field { display: flex; gap: 4px; align-items: baseline; }
.field-label { font-size: 11px; color: #333; white-space: nowrap; }
.field-value { font-size: 11px; color: #333; }
.w-12 { width: 3rem; }
.w-16 { width: 4rem; }
.w-20 { width: 5rem; }
.w-24 { width: 6rem; }
.w-28 { width: 7rem; }
.w-32 { width: 8rem; }
.w-40 { width: 10rem; }
.flex-1 { flex: 1; }
.ml-4 { margin-left: 1rem; }

/* Products Table */
.products-table { width: 100%; border-collapse: collapse; font-size: 12px; }
.products-table th {
    background: #e53935;
    color: white;
    padding: 6px 6px;
    font-weight: 600;
    font-size: 10px;
    text-transform: uppercase;
    text-align: center;
}
.products-table td {
    padding: 4px 6px;
    border-bottom: 1px solid #eee;
    font-size: 12px;
    text-align: center;
}
.products-table tbody tr:nth-child(even) { background: #fafafa; }

/* Totals Grid */
.totals-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 6px; }
.total-item {
    display: flex;
    justify-content: space-between;
    padding: 4px 6px;
    background: #fff;
    border-radius: 3px;
}

/* Financial Section */
.financial-section {
    flex: 2;
    background: #fff;
    border: 2px solid #e53935;
    border-radius: 4px;
    margin-bottom: 0;
    overflow: hidden;
}
.financial-title { border-bottom-color: #e53935 !important; }
.financial-grid { display: flex; gap: 8px; padding: 8px 10px; align-items: stretch; }
.financial-item {
    text-align: center;
    padding: 6px 10px;
    background: #f8f8f8;
    border-radius: 3px;
    display: flex;
    flex-direction: column;
    justify-content: center;
}
.financial-item.large { flex: 1.2; }
.financial-label { font-size: 10px; color: #666; text-transform: uppercase; display: block; }
.financial-value { font-size: 14px; font-weight: 700; color: #333; white-space: nowrap; }
.financial-item.total { background: #f8f8f8; }
.financial-item.total .financial-label { color: #666; }
.financial-item.total .financial-value { color: #333; font-size: 14px; font-weight: 700; }

/* Taxes subsection */
.financial-taxes {
    flex: 1;
    background: #f0f0f0;
    border-radius: 3px;
    padding: 4px 8px;
    text-align: center;
}
.taxes-label { font-size: 10px; color: #666; text-transform: uppercase; display: block; margin-bottom: 2px; }
.taxes-grid { display: flex; gap: 6px; justify-content: center; }
.tax-item { display: flex; gap: 2px; align-items: baseline; white-space: nowrap; }
.tax-name { font-size: 10px; color: #666; }
.tax-value { font-size: 14px; font-weight: 700; color: #333; }

/* Notes */
.notes-block { margin-bottom: 6px; }
.notes-block:last-child { margin-bottom: 0; }
.notes-text {
    margin: 3px 0 0;
    padding: 6px 8px;
    background: #fff;
    border-left: 2px solid #e53935;
    font-size: 12px;
    line-height: 1.4;
}

/* Utilities */
.text-left { text-align: left !important; }
.text-right { text-align: right !important; }

/* ========== PRINT STYLES (page-break rules only) ========== */
@page { margin: 5mm; }
@media print {
    .section { page-break-inside: avoid; }
    .products-table { page-break-inside: auto; }
    .products-table tr { page-break-inside: avoid; }
    .products-table thead { display: table-header-group; }
}
</style>
