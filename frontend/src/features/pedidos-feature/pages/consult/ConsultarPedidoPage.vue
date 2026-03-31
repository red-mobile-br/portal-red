<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';

import { RmCard, RmText, RmForm, RmInput, RmDivider, RmTextField, RmTimelineItem, RmIconButton, RmLoading, RmModal, RmButton, RmLoadingModal, RmSearchDialog } from '../../../../components';
import orderIllustration from '@/assets/illustrations/order-illustration.svg';

import { useAlert } from '@/hooks/alerta';
import { useToast, useDismissToast } from '@/hooks/toast';
import { useDetalheProduto } from '@/hooks/detalhe-produto';
import { useAutorizacao } from '@/hooks/autorizacao';
import { useDownloader } from '@/hooks/downloader';

import { formatarData, mascaraCnpj, mascaraTelefone } from '@/utils/string-functions';
import { formatarDecimal } from '@/utils/number-functions';
import { emailValido, obrigatorio } from '@/utils/validadores';
import Constantes from '@/utils/constantes';

import pedidoService from '@/services/pedido-service';
import orcamentoService from "@/services/orcamento-service";
import statusPedidoEnumParser from '@/core/enums/status-pedido-enum-parser';
import ErroDTO from '@/core/dtos/ErroDTO';
import { DownloadIcon, EmailIcon } from '@/icons';
import PedidoListaItemDTO from '@/core/dtos/pedido/PedidoListaItemDTO';
import { Canceler } from 'axios';

const alert = useAlert();
const route = useRoute();
const router = useRouter();
const toast = useToast();
const dismissToast = useDismissToast();
const { eAdmin, eGerente } = useAutorizacao();
const { baixarBase64 } = useDownloader();

const ehPaginaOrcamento = computed(() => route.name!.toString().indexOf('orcamento') >= 0);
const titulo = ehPaginaOrcamento.value ? "Orçamento" : "Pedido";

const estadoLocal = reactive({
    email: '',
    validarAoDigitar: false,
    enviandoEmail: false,
    modalEmailAberto: false,
    atualizandoPedido: false
});

const {
    state,
    obterDetalhePedido,
    interpretarModoFrete,
    totalVolumes,
    totalProdutos,
    totalPesoBruto,
    totalPesoLiquido,
    nomeUsuario,
    planoPagamento,
    tipoPedido,
    dialogoBuscarPedido
} = useDetalheProduto();


/** Status do pedido */
const status = computed(() => {
    return state.pedidoSelecionado ? statusPedidoEnumParser.get(state.pedidoSelecionado.status)?.titulo || '-' : "";
});

const limpar = () => {
    estadoLocal.email = "";
    estadoLocal.validarAoDigitar = false;
    estadoLocal.enviandoEmail = false;
    estadoLocal.modalEmailAberto = false;
    estadoLocal.atualizandoPedido = false;
    state.planosPagamento = [];
    state.carregando = false;
    state.pedidoSelecionado = null;
    state.numeroPedido = '';
};


const imprimir = () => {
    const rota = router.resolve({ name: 'impressaoPedido', params: { id: state.pedidoSelecionado!.id } });
    window.open(rota.href, '_blank');
};

/** Baixar nota fiscal */
const baixarNotaFiscal = async (idNota: string) => {
    toast({ mensagem: 'Aguarde enquanto processamos a nota fiscal' });
    try {

        // Baixar a nota
        const [request] = pedidoService.obterNotaFiscal(state.pedidoSelecionado!.id, idNota);
        const resp = await request;

        await baixarBase64({ base64: resp.data, nomeArquivo:`nf-${idNota}.pdf`, tipoMime: 'application/pdf' });

    } catch (error) {
        toast({ mensagem: "Não foi possível baixar a NF, por favor, tente novamente" });
    }
};

/** Baixar boleto */
const baixarBoleto = async (idNota: string) => {
    toast({ mensagem: 'Aguarde enquanto processamos o seu boleto' });
    try {

        // Baixar o boleto
        const [request] = pedidoService.obterBoleto(state.pedidoSelecionado!.id, idNota);
        const resp = await request;
        if(!resp.data) {
            toast({ mensagem: "Este boleto não está disponível" });
            return;
        }

        await baixarBase64({ base64: resp.data, nomeArquivo:`boleto-${idNota}.pdf`, tipoMime: 'application/pdf' });
    } catch (error) {
        toast({ mensagem: "Não foi possível baixar o boleto, por favor, tente novamente" });
    }
};

/** Abrir modal de enviar relatório por email */
const abrirModalEmail = () => {
    estadoLocal.validarAoDigitar = false;
    estadoLocal.email = '';
    estadoLocal.modalEmailAberto = true;
};

/** Enviar anexo por e-mail */
const enviarEmail = async (e: {isValid: boolean}) => {
    estadoLocal.validarAoDigitar = true;
    if(e.isValid && state.pedidoSelecionado) {
        // Fechar modal
        estadoLocal.modalEmailAberto = false;

        // Mostrar toast de loading
        const idToastCarregando = toast({ mensagem: 'Enviando e-mail...', carregando: true });

        try {
            // Fazer a requisição
            const [request] = pedidoService.enviarEmail(state.pedidoSelecionado.id, estadoLocal.email);
            await request;

            // Dismiss loading toast e mostrar sucesso
            dismissToast(idToastCarregando);
            toast({
                icone: "checkCircle",
                mensagem: "E-mail enviado com sucesso",
            });
        }
        catch(error) {
            // Dismiss loading toast e mostrar erro
            dismissToast(idToastCarregando);
            const mensagemErro = (error as ErroDTO)?.mensagem
                || 'Não foi possível enviar o e-mail. Tente novamente mais tarde.';
            toast({ mensagem: mensagemErro, icone: "AlertIcon" });
        }
    }
};

/** Aprovar/rejeitar pedido */
const aprovarOuRejeitar = async (aprovado: boolean) => {
    const idPedido = state.pedidoSelecionado!.id;
    estadoLocal.atualizandoPedido = true;

    try {
        const promise = aprovado
            ? (ehPaginaOrcamento.value ? orcamentoService.aprovar(idPedido)[0] : pedidoService.aprovar(idPedido)[0])
            : (ehPaginaOrcamento.value ? orcamentoService.recusar(idPedido)[0] : pedidoService.recusar(idPedido)[0]);

        const resp = await promise;

        if(!resp.sucesso) {
            return alert({
                mensagem: resp.detalhes
            });
        }

        alert({
            titulo: `${titulo} ${idPedido}`,
            mensagem: `${titulo} ${idPedido} ${(aprovado ? 'aprovado com sucesso' : 'rejeitado')}`,
            icone: aprovado ? 'checkCircle' : 'alert',
            acoes: [
                {
                    titulo: 'Ok',
                    acao: () => obterDetalhePedido({ isValid: true })
                }
            ]
        });

    } catch (e) {
        const error = e as ErroDTO | string;
        const mensagemErro = typeof(error) == 'string'
            ? error
            : (error.mensagem) || Constantes.DEFAULT_RESPONSE;

        alert({
            titulo: 'Opss...',
            mensagem: mensagemErro
        });
    }
    finally {
        estadoLocal.atualizandoPedido = false;
    }
};

let canceladorRequisicao: Canceler | null = null;
const buscarPedido = async (consulta: string): Promise<PedidoListaItemDTO[]> => {
    canceladorRequisicao && canceladorRequisicao();
    const [ request, canceler ] = ehPaginaOrcamento.value
        ? orcamentoService.obterLista({ busca: consulta, ordenarPor:'issueDate', direcao: 'desc' })
        : pedidoService.obterLista({ busca: consulta, ordenarPor:'issueDate', direcao: 'desc' });
    canceladorRequisicao = canceler;
    return (await request).dados;

};

onMounted(() => {
    if(route.params.id) {
        state.numeroPedido = route.params.id.toString();
        obterDetalhePedido({ isValid: true });
    }
});

</script>

<template>
    <div class="flex-1 flex flex-col 2xl:flex-row items-stretch gap-4">

        <RmCard class="self-stretch flex flex-col 2xl:rounded-b-none w-full 2xl:w-[calc(100%-20rem)] flex-1">
            <RmText type="headline-small" class="mb-4">
                Informe o número do {{ titulo.toLowerCase() }} ou dados do cliente
            </RmText>

            <!-- Formulário de buscar pedido -->
            <RmForm class="max-w-sm mb-5" @on:submit="obterDetalhePedido">

                <!-- Buscar cliente -->
                <RmInput v-model="state.numeroPedido"
                         name="empresa"
                         placeholder="Ex. 000123 ou Nome ou CNPJ"
                         submit-icon="SearchIcon"
                         :disabled="state.carregando"
                         :show-submit="true"
                         :show-clear-button="state.numeroPedido.length > 0 || state.pedidoSelecionado != null"
                         @on:clear="limpar" />
            </RmForm>

            <RmDivider class="mb-5" />

            <!-- ilustração -->
            <div v-if="!state.pedidoSelecionado" class="flex-1 flex flex-col items-center justify-center opacity-60">
                <img :src="orderIllustration" class="w-64 mb-4" alt="Ilustração do pedido">
                <div class="h-10 flex items-center justify-center">
                    <RmLoading v-if="state.carregando" />
                    <p v-else class="text-sm text-center font-semibold text-gray-400 w-52">
                        Selecione um {{ titulo.toLowerCase() }} para obter mais informações
                    </p>
                </div>

            </div>

            <!-- CONTEÚDO -->
            <div v-else>

                <!-- Status -->
                <RmText type="headline-small">
                    Status
                </RmText>
                <RmText type="display-large">
                    {{ status }}
                </RmText>

                <RmDivider class="my-5" />

                <!-- Informações do pedido -->
                <RmText type="headline-small" class="mb-4">
                    Informações do {{ titulo.toLowerCase() }}
                </RmText>



                <!-- Representante -->
                <div class="flex flex-col items-stretch gap-3">
                    <!-- Tipo de pedido -->
                    <RmTextField label="Tipo de pedido" class="max-w-sm">
                        {{ tipoPedido }}
                    </RmTextField>

                    <RmTextField v-if="eAdmin" label="Gerente">
                        {{ state.pedidoSelecionado.idGerente }} - {{ state.pedidoSelecionado.nomeGerente }}
                    </RmTextField>

                    <RmTextField v-if="eGerente" label="Representante">
                        {{ state.pedidoSelecionado.idRepresentante }} - {{ state.pedidoSelecionado.nomeRepresentante }}
                    </RmTextField>
                </div>

                <RmDivider class="my-5" />


                <!-- Acompanhamento -->
                <RmText type="headline-small" class="mb-4">
                    Acompanhamento do {{ titulo.toLowerCase() }}
                </RmText>
                <div class="bg-neutral-300 dark:bg-gray-600  rounded-md mb-4 p-4">
                    <RmTimelineItem v-for="(note, index) in state.pedidoSelecionado.notas" :key="index"
                                    :date="note.dataHora"
                                    :title="note.atividade"
                                    :subtitle="note.detalhe">
                        <p class="text-sm">
                            {{ note.autor }}
                        </p>
                    </RmTimelineItem>

                    <p v-if="!state.pedidoSelecionado.notas || state.pedidoSelecionado.notas.length == 0" class="text-center text-sm">
                        Não houve atualizações para este {{ titulo.toLowerCase() }}
                    </p>
                </div>

                <RmDivider class="mb-5" />

                <!-- Conteúdo -->
                <RmText type="headline-small" class="mb-4">
                    Dados do cliente
                </RmText>

                <!-- Dados do cliente -->
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                    <RmTextField label="Identificador">
                        {{ state.pedidoSelecionado.cliente.id }}
                    </RmTextField>
                    <RmTextField label="Razão social" class="sm:col-span-2">
                        {{ state.pedidoSelecionado.cliente.razaoSocial }}
                    </RmTextField>
                    <RmTextField label="CNPJ">
                        {{ mascaraCnpj(state.pedidoSelecionado.cliente.cnpj) }}
                    </RmTextField>
                    <RmTextField label="Nome fantasia" class="sm:col-span-2">
                        {{ state.pedidoSelecionado.cliente.nomeFantasia }}
                    </RmTextField>
                    <RmTextField label="Cep">
                        {{ state.pedidoSelecionado.cliente.endereco.cep }}
                    </RmTextField>
                    <RmTextField label="Logradouro" class="sm:col-span-2">
                        {{ state.pedidoSelecionado.cliente.endereco.logradouro }}
                    </RmTextField>
                    <RmTextField label="Número">
                        {{ state.pedidoSelecionado.cliente.endereco.numero }}
                    </RmTextField>
                    <RmTextField label="Complemento" class="sm:col-span-2">
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
                    <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                        <RmTextField label="Cep">
                            {{ state.pedidoSelecionado.enderecoEntrega.cep }}
                        </RmTextField>
                        <RmTextField label="Logradouro" class="sm:col-span-2">
                            {{ state.pedidoSelecionado.enderecoEntrega.logradouro }}
                        </RmTextField>
                        <RmTextField label="Número">
                            {{ state.pedidoSelecionado.enderecoEntrega.numero }}
                        </RmTextField>
                        <RmTextField label="Complemento" class="sm:col-span-2">
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
                    <div class="max-w-xs mb-5">
                        <RmTextField label="E-mail do destinatário" class="mb-4">
                            {{ state.pedidoSelecionado.dadosAgendamento.email }}
                        </RmTextField>
                        <RmTextField label="Telefone para contato">
                            {{ mascaraTelefone(state.pedidoSelecionado.dadosAgendamento.telefone) }}
                        </RmTextField>
                    </div>

                    <RmDivider class="my-5" />
                </template>

                <!-- Produtos -->
                <RmText type="headline-small" class="mb-4">
                    Produtos
                </RmText>

                <div class="overflow-x-auto">
                    <table class="red-mobile-table --small" style="min-width: 1000px">
                        <tr>
                            <th class="w-10">
                                Item
                            </th>
                            <th class="w-14">
                                Cód.
                            </th>
                            <th class="text-left">
                                Produto
                            </th>
                            <th class="w-12">
                                Qtd
                            </th>
                            <th class="w-18">
                                Desc. (%)
                            </th>
                            <th class="w-24">
                                Valor Unit.
                            </th>
                            <th class="w-24">
                                Preço tabela
                            </th>
                            <th class="w-18">
                                Comissão
                            </th>
                            <th class="w-16">
                                IPI
                            </th>
                            <th class="w-16">
                                ICMS
                            </th>
                            <th class="w-16">
                                ICMS ST
                            </th>
                            <th v-if="eGerente" class="w-16">
                                Margem
                            </th>
                            <th class="w-24">
                                Total
                            </th>
                            <th class="w-24">
                                A faturar
                            </th>
                        </tr>
                        <tr v-for="(product, index) in state.pedidoSelecionado.produtos" :key="index">
                            <td>{{ index + 1 }}</td>
                            <td>{{ product.id }}</td>
                            <td class="!text-left">
                                {{ product.descricao }}
                            </td>
                            <td>{{ product.quantidade }}</td>
                            <td>{{ formatarDecimal(product.percentualDesconto) }}%</td>
                            <td>R$ {{ formatarDecimal(product.valorUnitario) }}</td>
                            <td>R$ {{ formatarDecimal(product.precoBase) }}</td>
                            <td>{{ formatarDecimal(product.comissao) }}%</td>
                            <td>{{ formatarDecimal(product.percentualIPI ?? 0) }}%</td>
                            <td>{{ formatarDecimal(product.percentualICMS ?? 0) }}%</td>
                            <td>{{ formatarDecimal(product.percentualICMSST ?? 0) }}%</td>
                            <td v-if="eGerente">
                                {{ formatarDecimal(product.margem) }}%
                            </td>
                            <td>R$ {{ formatarDecimal(product.valorTotal) }}</td>
                            <td>{{ product.saldoPendente }}</td>
                        </tr>
                    </table>
                </div>

                <RmDivider class="mb-5" />

                <div v-if="!ehPaginaOrcamento">
                    <!-- ============== FATURAMENTO =========-->
                    <RmText type="headline-small" class="mb-4">
                        Fiscal e faturamento
                    </RmText>

                    <div class="max-w-xs">

                        <!-- Notas fiscais -->
                        <RmTextField label="NFs" class="mb-4">
                            <div v-for="invoice in state.pedidoSelecionado.notasFiscais"
                                 :key="invoice.codigo"
                                 class="flex items-center w-full">

                                <div class="flex-1 flex items-center hover:underline cursor-pointer"
                                     @click="baixarNotaFiscal(invoice.codigo)">
                                    <DownloadIcon class="fill-primary mr-1 w-4" />
                                    <div class="flex-1">
                                        {{ invoice.codigo }}
                                    </div>
                                </div>
                                <a v-if="invoice.urlRastreio" :href="invoice.urlRastreio" target="_blank">
                                    <RmIconButton icon="TruckIcon" class="w-6" icon-class="w-4 fill-primary" />
                                </a>
                            </div>
                            <div v-if="!state.pedidoSelecionado.notasFiscais.length">
                                -
                            </div>
                        </RmTextField>

                        <!-- Boletos -->
                        <RmTextField label="Boletos" class="mb-4">
                            <div v-if="state.pedidoSelecionado.boletos.length">
                                <div v-for="boleto in state.pedidoSelecionado.boletos"
                                     :key="boleto" class="flex items-center hover:underline mb-1 last:mb-0 cursor-pointer"
                                     @click="baixarBoleto(boleto)">
                                    <DownloadIcon class="fill-primary mr-1 w-4" />
                                    <div>{{ boleto }}</div>
                                </div>
                            </div>
                            <div v-else>
                                -
                            </div>
                        </RmTextField>
                    </div>

                    <!-- Número da OC -->
                    <RmTextField label="Número da OC" class="mb-4">
                        {{ state.pedidoSelecionado.numeroPedidoCompra || '-' }}
                    </RmTextField>

                    <!-- Condição de pagamento -->
                    <RmTextField label="Condição de pagamento" class="mb-4">
                        {{ planoPagamento }}
                    </RmTextField>

                    <!-- Data do faturamento -->
                    <RmTextField label="Data do faturamento" class="mb-4">
                        {{ formatarData(state.pedidoSelecionado.dataLancamento) }}
                    </RmTextField>
                </div>

                <RmTextField label="Informações para a NF" class="mb-4">
                    {{ state.pedidoSelecionado.informacoesNota || '-' }}
                </RmTextField>
                <RmTextField :label="`Observações do ${titulo.toLowerCase()}`">
                    {{ state.pedidoSelecionado.observacoesPedido || '-' }}
                </RmTextField>
            </div>

        </RmCard>

        <div class="w-full 2xl:w-80 transition-opacity duration-300 mb-4 2xl:block" :class="{'opacity-0 hidden': !state.pedidoSelecionado}">
            <transition name="fade">
                <RmCard v-if="state.pedidoSelecionado" class="!p-0 sticky top-4 flex flex-col items-stretch 2xl:h-novo-pedido-card">
                    <div class="p-4">
                        <div class="flex items-center mb-2">
                            <RmText type="headline-small" class="flex-1">
                                Código do {{ titulo.toLowerCase() }}
                            </RmText>
                            <RmIconButton v-if="state.pedidoSelecionado.status == '0' || state.pedidoSelecionado.status == 'M' || state.pedidoSelecionado.status == 'R'"
                                          icon="EditIcon"
                                          icon-class="fill-primary w-[1.3rem]"
                                          class="mr-2 w-[1.3rem]"
                                          @click="$router.replace({ name: ehPaginaOrcamento ? 'editarOrcamento' : 'editarPedido', params: { id: state.pedidoSelecionado.id }})" />
                            <RmIconButton v-if="!ehPaginaOrcamento"
                                          icon="EmailIcon"
                                          icon-class="fill-primary w-[1.3rem]"
                                          class="mr-2 w-[1.3rem]"
                                          @click="abrirModalEmail" />
                            <RmIconButton icon="PrintIcon"
                                          icon-class="fill-primary w-[1.3rem]"
                                          class="mr-2 w-[1.3rem]"
                                          @click="imprimir" />
                        </div>
                        <RmText type="display-large">
                            {{ state.pedidoSelecionado.id }}
                        </RmText>
                    </div>
                    <RmDivider />
                    <div class="flex-1 overflow-auto light-scroll p-4">
                        <div class="grid grid-cols-1 sm:grid-cols-4 lg:grid-cols-5 2xl:grid-cols-1 gap-4">
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
                            <RmTextField v-if="eGerente" :label="`Margem do ${titulo.toLowerCase()}`">
                                {{ formatarDecimal(state.pedidoSelecionado.margemPedido) }}%
                            </RmTextField>
                        </div>
                    </div>
                    <RmDivider />
                    <div class="p-4 sm:max-w-xs">
                        <RmText type="headline-small">
                            Total da nota fiscal:
                        </RmText>
                        <RmText type="display-large">
                            R$ {{ formatarDecimal(state.pedidoSelecionado.valorNota) }}
                        </RmText>
                    </div>

                    <template v-if="eAdmin && state.pedidoSelecionado.status == 'M'">
                        <RmDivider />
                        <div class="flex items-center space-x-3 p-4">
                            <RmButton type="hollow" class="flex-1" @click="aprovarOuRejeitar(false)">
                                Rejeitar
                            </RmButton>
                            <RmButton class="flex-1" @click="aprovarOuRejeitar(true)">
                                Aprovar
                            </RmButton>
                        </div>
                    </template>
                </RmCard>
            </transition>
        </div>

        <RmModal :is-opened="estadoLocal.modalEmailAberto">
            <template #default>
                <EmailIcon class="fill-primary w-15 mb-5" />
                <h2 class="font-bold mb-2">
                    Enviar {{ titulo.toLowerCase() }} por e-mail
                </h2>
                <p class="text-sm opacity-80 whitespace-pre-line mb-4">
                    Para qual e-mail gostaria de enviar este {{ titulo.toLowerCase() }}?
                </p>

                <RmForm id="emailForm"
                        class="w-full" :validate-on-input="estadoLocal.validarAoDigitar" @on:submit="enviarEmail">
                    <RmInput v-model="estadoLocal.email" name="email" placeholder="Ex.: contato@empresa.com.br" :rules="[obrigatorio, emailValido]" />
                </RmForm>
            </template>

            <!-- Error Footer -->
            <template #footer>
                <button class="modal-secondary" @click="estadoLocal.modalEmailAberto = false">
                    Cancelar
                </button>
                <button class="modal-primary" form="emailForm">
                    Enviar
                </button>
            </template>
        </RmModal>

        <!-- Modal de carregando -->
        <RmLoadingModal :is-loading="estadoLocal.atualizandoPedido" />

        <RmSearchDialog ref="dialogoBuscarPedido" :title="`Buscar ${titulo.toLowerCase()}`" placeholder="Informe o nome ou CNPJ do cliente" :request-callback="buscarPedido">
            <template #header>
                <tr>
                    <th class="w-20 !px-4">
                        {{ titulo.toLowerCase() }}
                    </th>
                    <th class="w-20 !px-4">
                        Data
                    </th>
                    <th class="text-left !px-3">
                        Cliente
                    </th>
                    <th class="w-36">
                        CNPJ
                    </th>
                    <th class="w-28">
                        Valor
                    </th>
                </tr>
            </template>
            <template #content="{ data }">
                <td>{{ data.id }}</td>
                <td>{{ formatarData(data.dataLancamento) }}</td>
                <td class="!text-left !px-3">
                    {{ data.nome }}
                </td>
                <td>{{ mascaraCnpj(data.cnpj) }}</td>
                <td class="!text-right">
                    R$ {{ formatarDecimal(data.valorTotal) }}
                </td>
            </template>

            <template #empty>
                <td colspan="3">
                    Nenhum {{ titulo.toLowerCase() }} encontrado
                </td>
            </template>
        </RmSearchDialog>
    </div>
</template>
