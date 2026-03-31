<script lang="ts" setup>
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { format } from 'date-fns';
import useAppStore from '@/store/app-store';

// Componentes
import {
    RmCard,
    RmText,
    RmDivider,
    RmButton,
    RmInput,
    RmForm,
    RmTextField,
    RmSelect,
    RmTextArea,
    RmLoading,
    RmIconButton,
    RmModal,
    RmDateInput,
    RmLoadingModal,
    RmCheckbox,
    RmBuscarGerenteModal,
    BuscarGerenteModalInstancia,
    RmMoneyInput,
} from '@/components';
import { FormInstance } from '@/components/RmForm.vue';
import RmBuscarRepresentanteModal from '@/components/RmBuscarRepresentanteModal.vue';
import { BuscarRepresentanteModalInstancia } from '@/components/RmBuscarRepresentanteModal.vue';

// Funções
import { obrigatorio, cnpjValido, dataValida, telefoneValido, cepValido, emailValido } from '@/utils/validadores';
import { formatarDecimal, arredondarPreco } from '@/utils/number-functions';
import { estadosBrasileiros } from '@/utils/estados-brasileiros';
import { mascaraCnpj, removerMascaraCnpj, dataParaIso, mascaraTelefone } from '@/utils/string-functions';
import Constantes from '@/utils/constantes';

// Services e DTOS
import pedidoService from '@/services/pedido-service';
import orcamentoService from "@/services/orcamento-service";
import planoPagamentoService from '@/services/plano-pagamento-service';
import { clienteService } from '@/services/cliente-service';
import CepService from '@/services/cep-service';
import ProdutoDTO from '@/core/dtos/produto/ProdutoDTO';
import CriarPedidoDTO from '@/core/dtos/pedido/CriarPedidoDTO';
import ConsultaImpostoLoteDTO from '@/core/dtos/produto/ConsultaImpostoLoteDTO';
import axios, { type Canceler } from 'axios';

// Hooks
import { useAlert } from '@/hooks/alerta';
import { useToast, useDismissToast } from '@/hooks/toast';
import { useAutorizacao } from '@/hooks/autorizacao';

// Includes
import DialogoBuscarCliente, { SearchCustomerDialogInstance } from './components/DialogoBuscarCliente.vue';
import DialogoBuscarProduto, { SearchProductDialogInstance } from './components/DialogoBuscarProduto.vue';
import PlanoPagamentoDTO from '@/core/dtos/planosPagamento/PlanoPagamentoDTO';
import ErroDTO from '@/core/dtos/ErroDTO';
import { onBeforeRouteLeave, useRoute, useRouter } from 'vue-router';
import statusPedidoEnumParser from '@/core/enums/status-pedido-enum-parser';

import selectIllustration from '@/assets/illustrations/select-illustration.svg';
import { AlertIcon } from '@/icons';
import { ClienteDetalhadoDTO } from '@/core/dtos/cliente/ClienteDetalhadoDTO';
import BotaoRepresentante from './components/BotaoRepresentante.vue';

interface NovoPedidoState {
    carregandoDetalheCliente: boolean;
    carregandoDetalheProduto: boolean;
    buscaCliente: string;
    clienteSelecionado: ClienteDetalhadoDTO | null;
    produtos: ProdutoDTO[];
    buscaProduto: string;
    validarAoDigitar: boolean;
    enviandoPedido: boolean;
    planosPagamento: PlanoPagamentoDTO[];
    codigoExclusao: string;
    modalExclusaoAberto: boolean;
    statusPedido: string;
    nomeRepresentante: string | null;
    idRepresentante: string | null;
    nomeGerente: string | null;
    idGerente: string | null;
    form: {
        agendado: boolean,
        planoPagamento: string,
        tipoPedido: 0 | 1,
        enderecoEntrega: {
            logradouro: string,
            numero: string,
            complemento: string,
            bairro: string,
            cidade: string,
            estado: string,
            cep: string
        },
        numeroPedidoCompra: string,
        dataFaturamento: string,
        modoFrete: 0 | 1,
        dadosAgendamento: {
            email: string,
            telefone: string
        },
        entregarNoEnderecoCliente: boolean,
        informacoesNota: string,
        observacoesPedido: string
    },
    resumoPedido: {
        totalProdutos: number,
        totalICMS: number,
        totalIPI: number,
        totalICMSST: number,
        totalNF: number,
        pesoLiquido: number,
        pesoBruto: number,
        margemPedido: number,
        totalVolumes: number,
        totalItens: number,
    },
}

// Hooks
const alert = useAlert();
const toast = useToast();
const dismissToast = useDismissToast();
const { params, name } = useRoute();
const { replace } = useRouter();
const { eGerente, eAdmin } = useAutorizacao();
const { nomeUsuario, idRepresentanteUsuario } = useAppStore();
const tamanhoColspan = eGerente ? "14" : "13";

// Refs
const modalCliente = ref<SearchCustomerDialogInstance | null>(null);
const modalProduto = ref<SearchProductDialogInstance | null>(null);
const freteForm = ref<FormInstance | null>(null);
const agendamentoForm = ref<FormInstance | null>(null);
const entregaForm = ref<FormInstance | null>(null);
const faturamentoForm = ref<FormInstance | null>(null);
const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia>();
const refModalBuscarGerente = ref<BuscarGerenteModalInstancia | null>(null);

const ehPaginaOrcamento = computed(() => name!.toString().indexOf('orcamento') >= 0);
const titulo = ehPaginaOrcamento.value ? "Orçamento" : "Pedido";

// Variáveis
const exibirAlertaAoSair = ref(false);

// Estado
const state = reactive<NovoPedidoState>({
    carregandoDetalheCliente: false,
    carregandoDetalheProduto: false,
    clienteSelecionado: null,
    buscaCliente: '',
    buscaProduto: '',
    produtos: [],
    planosPagamento: [],
    validarAoDigitar: false,
    enviandoPedido: false,
    codigoExclusao: '',
    modalExclusaoAberto: false,
    statusPedido: "Em aberto",
    nomeRepresentante: nomeUsuario.value,
    idRepresentante: idRepresentanteUsuario.value,
    nomeGerente: eGerente ? nomeUsuario.value : null,
    idGerente: eGerente ? idRepresentanteUsuario.value : null,
    form: {
        tipoPedido: 0,
        agendado: false,
        planoPagamento: "",
        enderecoEntrega: {
            logradouro: '',
            numero: '',
            complemento: '',
            bairro: '',
            cidade: '',
            estado: '',
            cep: ''
        },
        numeroPedidoCompra: '',
        dataFaturamento: format(new Date(), 'dd/MM/yyyy'),
        modoFrete: 0,
        dadosAgendamento: {
            email: '',
            telefone: ''
        },
        entregarNoEnderecoCliente: true,
        informacoesNota: '',
        observacoesPedido: ''
    },
    resumoPedido: {
        totalProdutos: 0,
        totalICMS: 0,
        totalIPI: 0,
        totalICMSST: 0,
        totalNF: 0,
        pesoLiquido: 0,
        pesoBruto: 0,
        margemPedido: 0,
        totalVolumes: 0,
        totalItens: 0,
    },
});

// Observadores
/** Adicionar máscara caso digite um CNPJ válido */
watch(() => state.buscaCliente, (novoValor, valorAntigo) => {
    // Verificar se o valor novo é um cnpj válido para ser tratado
    const cnpjEValido = (/[0-9]{14}/i).test(novoValor) && novoValor.length == 14;
    if(cnpjEValido) {
        state.buscaCliente = mascaraCnpj(novoValor);
    }
    // Caso o valor antigo seja um cnpj com mascara, retirar a mascara
    const cnpjComMascara = cnpjValido(valorAntigo).length == 0;
    if(cnpjComMascara) {
        state.buscaCliente = removerMascaraCnpj(novoValor);
    }
});

/** Buscar endereço ao editar CEP */
watch(() => state.form.enderecoEntrega.cep, (novoValor, valorAntigo) => {
    if(novoValor != valorAntigo && novoValor.length == 9) {
        CepService.buscarCep(novoValor)
            .then(resp => {
                state.form.enderecoEntrega.logradouro = resp.logradouro;
                state.form.enderecoEntrega.bairro = resp.bairro;
                state.form.enderecoEntrega.cidade = resp.localidade;
                state.form.enderecoEntrega.estado = resp.uf;
            });
    }
});

// Propriedades computadas
const dataEmissao = computed(() => format(new Date(), 'dd/MM/yyyy'));

/** Aplicar impostos do response no produto */
const aplicarImpostosProduto = (produto: ProdutoDTO, taxes: import('@/core/dtos/produto/ImpostoProdutoDTO').default) => {
    produto.comissaoMaxima = taxes.comissaoMaxima ?? 5;
    produto.comissao = taxes.comissao;
    produto.icms = taxes.icms;
    produto.icmsst = taxes.icmsst;
    produto.ipi = taxes.ipi;
    produto.percentualICMSST = taxes.percentualICMSST ?? 0;
    produto.percentualICMS = taxes.percentualICMS ?? 0;
    produto.percentualIPI = taxes.percentualIPI ?? 0;
    produto.quantidade = produto.quantidade ?? 1;
    produto.percentualDesconto = produto.percentualDesconto ?? 0;
    produto.valorUnitario = produto.valorUnitario ?? produto.precoBase;
    produto.margem = taxes.margem;
    produto.valorTotal = taxes.valorTotal;
};

/** Cancelador de request anterior */
let canceladorImpostoLote: Canceler | null = null;

/** Recalcular tributação de todos os produtos via bulk */
const recalcularImpostos = async (produtoPendente?: ProdutoDTO): Promise<boolean> => {
    if ((state.produtos.length === 0 && !produtoPendente) || !state.clienteSelecionado) return true;

    if (canceladorImpostoLote) {
        canceladorImpostoLote();
    }

    const idToast = toast({ mensagem: 'Recalculando margem dos produtos...', carregando: true });

    try {
        const todosItens = produtoPendente
            ? [...state.produtos, produtoPendente]
            : state.produtos;

        const payload: ConsultaImpostoLoteDTO = {
            idCliente: state.clienteSelecionado.id,
            idRepresentante: state.idRepresentante,
            idGerente: state.idGerente,
            tipoPedido: state.form.tipoPedido,
            planoPagamento: state.form.planoPagamento,
            modoFrete: state.form.modoFrete,
            idPedido: params.id?.toString(),
            itens: todosItens.map(produto => ({
                idProduto: produto.id,
                precoBase: produto.precoBase,
                precoVenda: produto.valorUnitario ?? produto.precoBase,
                quantidade: produto.quantidade ?? 1,
                comissao: produto.comissao,
            })),
        };

        const [request, canceler] = clienteService.obterImpostosLote(payload);
        canceladorImpostoLote = canceler;
        const resposta = await request;

        if (resposta.itens.length !== todosItens.length) {
            throw new Error(`Tax response length mismatch: expected ${todosItens.length}, got ${resposta.itens.length}`);
        }

        // Aplicar impostos nos itens já na lista
        resposta.itens.forEach((impostos, index) => {
            if (state.produtos[index]) {
                aplicarImpostosProduto(state.produtos[index], impostos);
            }
        });

        // Aplicar impostos no produto pendente (último item da resposta)
        if (produtoPendente) {
            const impostoPendente = resposta.itens[resposta.itens.length - 1];
            if (impostoPendente) {
                aplicarImpostosProduto(produtoPendente, impostoPendente);
            }
        }

        state.resumoPedido = {
            totalProdutos: resposta.totalProdutos,
            totalICMS: resposta.totalICMS,
            totalIPI: resposta.totalIPI,
            totalICMSST: resposta.totalICMSST,
            totalNF: resposta.totalNF,
            pesoLiquido: resposta.pesoLiquido,
            pesoBruto: resposta.pesoBruto,
            margemPedido: resposta.margemPedido,
            totalVolumes: resposta.totalItens,
            totalItens: resposta.totalItens,
        };

        dismissToast(idToast);
        toast({ icone: 'checkCircle', mensagem: 'Margem dos produtos atualizada!' });
        return true;
    } catch (e) {
        dismissToast(idToast);
        if (!axios.isCancel(e)) {
            toast({ mensagem: 'Não foi possível calcular as taxas dos produtos. Por favor, tente novamente.' });
        }
        return false;
    }
};

/** Mudar o plano de pagamento */
const mudarPlanoPagamento = async (planoPagamento: string) => {
    state.form.planoPagamento = planoPagamento;
    await recalcularImpostos();
};

/** Mudar o tipo de frete */
const mudarModoFrete = async (modoFrete: 0 | 1) => {
    state.form.modoFrete = modoFrete;
    await recalcularImpostos();
};

/** Buscar detalhe de cliente */
const obterDetalheCliente = async() => {
    try {

        state.carregandoDetalheCliente = true;

        let cnpj = '';
        // Caso seja um CNPJ, buscar detalhe do cliente
        const cnpjEValido = cnpjValido(state.buscaCliente).length == 0;
        if(cnpjEValido) {
            cnpj = state.buscaCliente;
        }
        else {
            const cliente = await modalCliente.value!.search(state.buscaCliente);
            if(!cliente) {
                return;
            }
            cnpj = cliente.cnpj;
        }

        const [ requisicaoCliente ] = clienteService.obterDetalheCliente(cnpj);
        const [ requisicaoPlanos ] = planoPagamentoService.obterPlanosPagamento();

        const [respostaCliente, respostaPlanos] = await Promise.all([requisicaoCliente, requisicaoPlanos]);

        limparFormulario();

        exibirAlertaAoSair.value = true;

        state.clienteSelecionado = respostaCliente;
        state.planosPagamento = respostaPlanos;

        // Adicionar campos do cliente no agendamento
        state.form.dadosAgendamento.email = respostaCliente.email;
        state.form.dadosAgendamento.telefone = mascaraTelefone(respostaCliente.telefone);

        state.buscaCliente = '';

        state.idRepresentante = respostaCliente.idRepresentante;
        state.nomeRepresentante = respostaCliente.nomeRepresentante;

        state.idGerente = respostaCliente.idGerente;
        state.nomeGerente = respostaCliente.nomeGerente;
    } catch (e) {
        const error = e as ErroDTO;
        const mensagem = error.codigo == 404 ? 'Não foi encontrando nenhum cliente com o CNPJ informado' : error.mensagem;
        alert({ mensagem: mensagem });
    }
    finally {
        state.carregandoDetalheCliente = false;
    }
};

/** Buscar produto por código */
const obterProduto = async () => {

    try {
        state.carregandoDetalheProduto = true;

        if (!state.form.planoPagamento) {
            return  alert({ mensagem: 'Antes de adicionar um produto, é necessário escolher uma forma de pagamento.' });
        }

        let idProduto = '';
        // Caso seja um código, buscar detalhe do produto
        const ehNumeroValido = !isNaN(parseInt(state.buscaProduto));
        if(ehNumeroValido) {
            idProduto = state.buscaProduto;
        }
        else {
            const produto = await modalProduto.value!.search(state.buscaProduto);
            if(!produto) {
                return;
            }
            idProduto = produto.id;
        }

        const produtoExistente = state.produtos.find(el => el.id == idProduto.padStart(6, '0'));
        if(produtoExistente) {
            const quantidadeOriginal = produtoExistente.quantidade;
            produtoExistente.quantidade = quantidadeOriginal + 1;
            const sucesso = await recalcularImpostos();
            if (!sucesso) {
                produtoExistente.quantidade = quantidadeOriginal;
            } else {
                state.buscaProduto = "";
                toast({ mensagem: `${produtoExistente.descricao} adicionado`, icone: 'checkCircle' });
            }
        }
        else {
            const [ requisicaoProduto ] = clienteService.obterDetalheProduto(state.clienteSelecionado!.id, idProduto);
            const resp = await requisicaoProduto;
            if(!resp) {
                return alert({ mensagem: "Produto não encontrado" });
            }

            const produto = resp;
            produto.quantidade = 1;
            produto.valorUnitario = produto.precoBase;
            produto.percentualIPI = produto.percentualIPI ?? 0;
            produto.percentualICMS = produto.percentualICMS ?? 0;
            produto.percentualICMSST = produto.percentualICMSST ?? 0;
            produto.margem = produto.margem ?? 0;
            produto.percentualDesconto = produto.percentualDesconto ?? 0;

            const sucesso = await recalcularImpostos(produto);
            if (sucesso) {
                state.produtos.push(produto);
                state.buscaProduto = "";
                toast({ mensagem: `${produto.descricao} adicionado`, icone: 'checkCircle' });
            }
        }
    } catch (error) {
        const { codigo, mensagem } = error as ErroDTO;

        alert({ mensagem: codigo == 404 ? 'Produto não encontrado' : mensagem });
    }
    finally {
        state.carregandoDetalheProduto = false;
    }
};

/** Calcular porcentagem desconto */
const calcularPercentualDesconto = (valorUnitario: number, precoBase: number) => {
    return valorUnitario < precoBase
        ? arredondarPreco(100 - ((valorUnitario / precoBase) * 100))
        : 0;
};

/** Remover produto */
const removerProduto = (index: number) => {
    state.produtos.splice(index, 1);
    recalcularImpostos();
};

/** Limpar formulário */
const limparFormulario = () => {
    exibirAlertaAoSair.value = false;
    state.carregandoDetalheCliente= false;
    state.carregandoDetalheProduto= false;
    state.clienteSelecionado= null;
    state.buscaCliente= '';
    state.buscaProduto= '';
    state.produtos= [];
    state.validarAoDigitar= false;
    state.enviandoPedido = false;

    state.form.enderecoEntrega.logradouro= '';
    state.form.enderecoEntrega.numero = '';
    state.form.enderecoEntrega.complemento= '';
    state.form.enderecoEntrega.bairro= '';
    state.form.enderecoEntrega.cidade= '';
    state.form.enderecoEntrega.estado= '';
    state.form.enderecoEntrega.cep= '';
    state.form.agendado= false;
    state.form.numeroPedidoCompra = '';
    state.form.dataFaturamento = format(new Date(), 'dd/MM/yyyy');
    state.form.modoFrete = 0;
    state.form.dadosAgendamento.email = '';
    state.form.dadosAgendamento.telefone = '';
    state.form.entregarNoEnderecoCliente= false;
    state.form.informacoesNota= '';
    state.form.observacoesPedido = '';

    state.resumoPedido = {
        totalProdutos: 0,
        totalICMS: 0,
        totalIPI: 0,
        totalICMSST: 0,
        totalNF: 0,
        pesoLiquido: 0,
        pesoBruto: 0,
        margemPedido: 0,
        totalVolumes: 0,
        totalItens: 0,
    };

    state.nomeRepresentante = nomeUsuario.value;
    state.idRepresentante = idRepresentanteUsuario.value;
    state.nomeGerente = eGerente ? nomeUsuario.value : null;
    state.idGerente = eGerente ? idRepresentanteUsuario.value : null;
};

/** Cancelar pedido */
const cancelarPedido = () => {
    if(name == 'novopedido' || name == 'novoOrcamento') {
        alert({
            titulo: "Atenção",
            mensagem: `Tem certeza que deseja cancelar a inserção do ${ titulo.toLowerCase() }?`,
            acoes: [
                {
                    titulo: "Cancelar",
                    acao: () => limparFormulario()
                },
                {
                    titulo: "Voltar",
                    primario: true
                }
            ]
        });
    }
    else {
        state.modalExclusaoAberto = true;
    }
};


/** Enviar novo pedido */
const enviarNovoPedido = async (pedido: CriarPedidoDTO) => {

    const [request] = ehPaginaOrcamento.value
        ? orcamentoService.criar(pedido)
        : pedidoService.criar(pedido);

    const resp = await request;
    if(!resp.sucesso) {
        throw resp.detalhes;
    }

    alert({
        icone: "checkCircle",
        titulo: `${ titulo }: ${resp.id}`,
        mensagem: `Seu ${ titulo.toLowerCase() } foi criado com sucesso!`
    });

    limparFormulario();
};

const salvarEdicaoPedido = async (pedido: CriarPedidoDTO) => {
    const [ request ] = ehPaginaOrcamento.value
        ? orcamentoService.editar(params.id.toString(), pedido)
        : pedidoService.editar(params.id.toString(), pedido);

    await request;

    state.enviandoPedido = false;
    exibirAlertaAoSair.value = false;

    alert({
        titulo: `${ titulo } editado`,
        mensagem: `O ${ titulo.toLowerCase() } ${params.id} foi editado com sucesso`,
        acoes: [
            {
                titulo: 'Ver pedido',
                primario: true,
                acao: () => replace({ name: ehPaginaOrcamento.value ? 'consultarOrcamento' : 'consultarPedido', params: { id: params.id } })
            }
        ]
    });

};

/** Submeter pedido */
const submeter = async () => {
    try {
        state.validarAoDigitar = true;

        // Validar formulários
        const erroresFrete = freteForm.value?.submit().errors || [];
        const erroresAgendamento = agendamentoForm.value?.submit().errors || [];
        const erroresEntrega = entregaForm.value?.submit().errors || [];
        const erroresFaturamento = faturamentoForm.value?.submit().errors || [];

        const erros = [...erroresFrete, ...erroresAgendamento, ...erroresEntrega, ...erroresFaturamento];
        if(erros.length > 0) {
            return  alert({ titulo: 'Falha na validação', mensagem: erros.join(".\n") });
        }

        // Validar produtos
        if(state.produtos.length == 0) {
            return  alert({ titulo: 'Falha na validação', mensagem: "É necessário adicionar ao mínimo um produto." });
        }

        const produtoSemQuantidade = state.produtos.find(el => el.quantidade == 0);
        if(produtoSemQuantidade) {
            return  alert({
                titulo: 'Falha na validação',
                mensagem:  `O produto ${produtoSemQuantidade.descricao} precisa ter uma quantidade maior que zero.`
            });
        }

        state.enviandoPedido = true;

        const form: CriarPedidoDTO = {
            numeroPedidoCompra: state.form.numeroPedidoCompra,
            idCliente: state.clienteSelecionado!.id,
            dataEmissao: dataParaIso(state.form.dataFaturamento),
            modoFrete: state.form.modoFrete,
            tipoPedido: state.form.tipoPedido,
            dadosAgendamento: state.form.agendado ? { email: state.form.dadosAgendamento.email, telefone: state.form.dadosAgendamento.telefone } : undefined,
            enderecoEntrega: state.form.entregarNoEnderecoCliente
                ? state.clienteSelecionado!.endereco
                : {
                    logradouro: state.form.enderecoEntrega.logradouro,
                    numero: state.form.enderecoEntrega.numero,
                    complemento: state.form.enderecoEntrega.complemento,
                    bairro: state.form.enderecoEntrega.bairro,
                    cidade: state.form.enderecoEntrega.cidade,
                    estado: state.form.enderecoEntrega.estado,
                    cep: state.form.enderecoEntrega.cep,
                },
            planoPagamento: state.form.planoPagamento,
            idRepresentante: state.idRepresentante,
            idGerente: state.idGerente,
            produtos: state.produtos.map(produto => {
                return {
                    id: produto.id,
                    quantidade: produto.quantidade,
                    valorUnitario: produto.valorUnitario,
                    comissao: produto.comissao
                };
            }),
            informacoesNota: state.form.informacoesNota,
            observacoesPedido: state.form.observacoesPedido,
        };

        name == 'novopedido' || name == 'novoOrcamento' ? await enviarNovoPedido(form) : await salvarEdicaoPedido(form);

    } catch (e) {
        const error = e as ErroDTO | string;
        const mensagemErro = typeof(error) == 'string'
            ? error
            : (error.mensagem) || Constantes.DEFAULT_RESPONSE;
        state.enviandoPedido = false;
        alert({ titulo: 'Falha na validação', mensagem: mensagemErro });
    }

};

/** Excluir pedido */
const excluirPedido = async () => {
    state.modalExclusaoAberto = false;
    state.enviandoPedido = true;
    state.codigoExclusao = '';

    try {
        const [ request ] = ehPaginaOrcamento.value
            ? orcamentoService.excluir(params.id.toString())
            : pedidoService.excluir(params.id.toString());

        await request;
        exibirAlertaAoSair.value = false;
        state.enviandoPedido = false;
        alert({
            icone: 'checkCircle',
            titulo: `${ titulo } excluído`,
            mensagem: `Seu ${ titulo.toLowerCase() } foi excluído com sucesso!`,
            acoes: [
                {
                    titulo: "Voltar",
                    acao: () => replace({ name: ehPaginaOrcamento.value ? 'orcamentos' : 'pedidos' })
                }
            ]
        });
    }
    catch(e) {
        const error = e as ErroDTO;
        alert({
            mensagem: error.mensagem
        });
    }
    finally {
        state.enviandoPedido = false;
    }
};

/** Carregar dados do produto para edição */
const carregarParaEdicao = async () => {
    try {
        exibirAlertaAoSair.value = true;
        const [ requisicaoPlanos ] = planoPagamentoService.obterPlanosPagamento();
        const [ request ] = ehPaginaOrcamento.value
            ? orcamentoService.obterPorId(params.id.toString())
            : pedidoService.obterPorId(params.id.toString());

        const [planosPagamento, pedido] = await Promise.all([requisicaoPlanos, request]);
        state.planosPagamento = [...planosPagamento];

        // Mapear produtos
        state.produtos = [...pedido.produtos];

        // Mapear cliente
        state.clienteSelecionado = { ...pedido.cliente } as ClienteDetalhadoDTO;

        // Mapear propriedades
        state.idGerente = pedido.idGerente ?? '-';
        state.nomeGerente = pedido.nomeGerente ?? '-';
        state.nomeRepresentante = pedido.nomeRepresentante ?? '-';
        state.idRepresentante = pedido.idRepresentante ?? '-';
        state.statusPedido = statusPedidoEnumParser.get(pedido.status)!.titulo;
        state.form.tipoPedido = pedido.tipoPedido;
        state.form.agendado = (pedido.dadosAgendamento?.email?.length ?? 0) > 0 || (pedido.dadosAgendamento?.telefone?.length ?? 0) > 0;


        state.form.enderecoEntrega.logradouro = pedido.enderecoEntrega.logradouro;
        state.form.enderecoEntrega.numero = pedido.enderecoEntrega.numero;
        state.form.enderecoEntrega.complemento = pedido.enderecoEntrega.complemento;
        state.form.enderecoEntrega.bairro = pedido.enderecoEntrega.bairro;
        state.form.enderecoEntrega.cidade = pedido.enderecoEntrega.cidade;
        state.form.enderecoEntrega.estado = pedido.enderecoEntrega.estado;
        state.form.enderecoEntrega.cep = pedido.enderecoEntrega.cep;

        state.form.numeroPedidoCompra = pedido.numeroPedidoCompra;
        state.form.dataFaturamento = format(new Date(pedido.dataEmissao), 'dd/MM/yyyy');
        state.form.modoFrete = pedido.modoFrete;
        state.form.dadosAgendamento.email = pedido.dadosAgendamento?.email ?? '';
        state.form.dadosAgendamento.telefone = pedido.dadosAgendamento?.telefone ?? '';
        state.form.informacoesNota = pedido.informacoesNota;
        state.form.observacoesPedido = pedido.observacoesPedido;

        await mudarPlanoPagamento(pedido.planoPagamento);

    }
    catch(e) {
        const error = e as ErroDTO;
        alert({ mensagem: error.mensagem || "Não foi possível carregar os dados do pedido." });
    }
};

/** Selecionar um representante */
const buscarRepresentante = async () => {
    const representante = await refModalBuscarRepresentante.value?.search();
    if(representante) {
        state.idRepresentante = representante.id;
        state.nomeRepresentante = representante.nome;
        await recalcularImpostos();
    }
};

/** Limpar gerente selecionado */
const limparGerente = async () => {
    state.idGerente = null;
    state.nomeGerente = null;
    await recalcularImpostos();
};

/** Abrir modal de busca de gerentes */
const buscarGerente = async () => {
    const gerente = await refModalBuscarGerente.value?.search();
    if(gerente) {
        state.idGerente = gerente.id;
        state.nomeGerente = gerente.nome;
        await recalcularImpostos();
    }
};

// Debounce para recálculo de impostos
let temporizadorCalculo: NodeJS.Timeout | null = null;

const calcularImpostosDebounced = (_produto?: ProdutoDTO, delay = 500) => {
    if (temporizadorCalculo) {
        clearTimeout(temporizadorCalculo);
    }

    temporizadorCalculo = setTimeout(() => {
        recalcularImpostos();
        temporizadorCalculo = null;
    }, delay);
};

const definirComissaoProduto = async (comissao: number, produto: ProdutoDTO) => {
    produto.comissao = Math.max(0, Math.min(produto.comissaoMaxima, comissao));
    calcularImpostosDebounced(produto);
};
const definirQuantidadeProduto = async (quantidade: number, produto: ProdutoDTO) => {
    produto.quantidade = quantidade;
    calcularImpostosDebounced(produto);
};
const definirValorUnitarioProduto = async (valorUnitario: number, produto: ProdutoDTO) => {
    produto.valorUnitario = valorUnitario;
    calcularImpostosDebounced(produto);
};

onMounted(() => {
    if(name == 'editarPedido' || name == 'editarOrcamento') {
        carregarParaEdicao();
    }
});

onBeforeRouteLeave((_, __, next) => {
    if(exibirAlertaAoSair.value) {
        alert({
            titulo: "Atenção",
            mensagem: `Seu ${ titulo.toLowerCase() } não foi concluído. Caso saia, os dados serão perdidos. Tem certeza que deseja sair?`,
            acoes: [
                {
                    titulo: "Sair",
                    acao: () => {
                        exibirAlertaAoSair.value = false;
                        next();
                    }
                },
                {
                    titulo: "Permanecer",
                    primario: true
                }
            ]
        });
    }
    else {
        next();
    }
});
</script>

<template>
    <div class="flex-1 flex flex-col 2xl:flex-row items-stretch gap-4">

        <RmCard class="self-stretch flex-1 flex flex-col 2xl:rounded-b-none">

            <template v-if="$route.name == 'novopedido' || $route.name == 'novoOrcamento'">
                <RmText type="headline-small" class="mb-4">
                    Cliente
                </RmText>

                <!-- Nome do cliente -->
                <RmForm class="max-w-sm mb-5" @on:submit="obterDetalheCliente">

                    <!-- Buscar cliente -->
                    <RmInput v-model="state.buscaCliente"
                             name="empresa"
                             placeholder="Informe o código, nome ou CNPJ da empresa"
                             :disabled="state.carregandoDetalheCliente"
                             :show-submit="true"
                             submit-icon="SearchIcon" />
                </RmForm>

                <RmDivider class="mb-5" />
            </template>

            <div v-if="state.carregandoDetalheCliente" class="flex-1 flex flex-col items-center justify-center opacity-60">
                <RmLoading class="mb-3" />
                <p class="text-sm text-center font-semibold text-gray-400 w-40">
                    Carregando...
                </p>

            </div>

            <div v-else-if="!state.clienteSelecionado" class="flex-1 flex flex-col items-center justify-center opacity-60">
                <!-- Illustração -->
                <img :src="selectIllustration" class="mb-3 w-44" alt="Ilustração selecionar producto">
                <p class="text-sm text-center font-semibold text-gray-400 w-40">
                    {{ $route.name == 'novopedido' || $route.name == 'novoOrcamento' ? `Selecione um cliente para iniciar o ${ titulo.toLowerCase() }` : 'Carregando...' }}
                </p>
            </div>

            <div v-else>
                <RmText type="headline-small" class="mb-4">
                    Informações do {{ titulo.toLowerCase() }}
                </RmText>

                <!-- Número do pedido (em edição) -->
                <div v-if="$route.name == 'editarPedido' || $route.name == 'editarOrcamento'" class="grid grid-cols-1 sm:grid-cols-3 mb-4">
                    <RmText type="headline-small" class="flex-1 sm:col-span-3">
                        Código do {{ titulo.toLowerCase() }}
                    </RmText>
                    <RmText type="display-large">
                        {{ $route.params.id }}
                    </RmText>
                </div>

                <!-- Tipo do pedido -->
                <RmForm class="mb-4">
                    <RmSelect v-model.number="state.form.tipoPedido"
                              :placeholder="`Selecione o tipo do ${ titulo.toLowerCase() }`"
                              label="Tipo do pedido"
                              class="max-w-sm"
                              :rules="[obrigatorio]"
                              :name="`tipo do ${ titulo.toLowerCase() }`">
                        <option :value="0">
                            Venda
                        </option>
                        <option :value="1">
                            Bonificação
                        </option>
                    </RmSelect>
                </RmForm>

                <BotaoRepresentante v-if="eGerente"
                              class="mb-4"
                              label="Gerente responsável"
                              :disabled="!eAdmin"
                              :allow-empty="true"
                              :has-value="!!(state.idGerente && state.nomeGerente)"
                              @on:click="buscarGerente"
                              @on:clear="limparGerente">
                    {{ state.idGerente && state.nomeGerente ? `${state.idGerente} - ${state.nomeGerente}` : '-' }}
                </BotaoRepresentante>


                <!-- Representante -->
                <BotaoRepresentante label="Representante responsável"
                              :disabled="!eGerente"
                              @on:click="buscarRepresentante">
                    {{ state.idRepresentante }} - {{ state.nomeRepresentante }}
                </BotaoRepresentante>

                <RmDivider class="my-4" />

                <!-- Dados do cliente -->
                <RmText type="headline-small" class="mb-4">
                    Dados do cliente
                </RmText>

                <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                    <RmTextField label="Identificador">
                        {{ state.clienteSelecionado.id }}
                    </RmTextField>
                    <RmTextField label="Razão social" class="sm:col-span-2">
                        {{ state.clienteSelecionado.razaoSocial }}
                    </RmTextField>
                    <RmTextField label="CNPJ">
                        {{ mascaraCnpj(state.clienteSelecionado.cnpj) }}
                    </RmTextField>
                    <RmTextField label="Nome fantasia" class="sm:col-span-2">
                        {{ state.clienteSelecionado.nomeFantasia }}
                    </RmTextField>
                    <RmTextField label="Cep">
                        {{ state.clienteSelecionado.endereco.cep }}
                    </RmTextField>
                    <RmTextField label="Logradouro" class="sm:col-span-2">
                        {{ state.clienteSelecionado.endereco.logradouro }}
                    </RmTextField>
                    <RmTextField label="Número">
                        {{ state.clienteSelecionado.endereco.numero }}
                    </RmTextField>
                    <RmTextField label="Complemento" class="sm:col-span-2">
                        {{ state.clienteSelecionado.endereco.complemento || '-' }}
                    </RmTextField>
                    <RmTextField label="Bairro">
                        {{ state.clienteSelecionado.endereco.bairro }}
                    </RmTextField>
                    <RmTextField label="Município">
                        {{ state.clienteSelecionado.endereco.cidade }}
                    </RmTextField>
                    <RmTextField label="Estado">
                        {{ state.clienteSelecionado.endereco.estado }}
                    </RmTextField>
                </div>

                <RmDivider class="my-5" />

                <!-- Faturamento -->

                <RmText type="headline-small" class="mb-4">
                    Faturamento
                </RmText>

                <RmForm class="grid grid-cols-1 md:grid-cols-3 gap-3">
                    <!-- Condição de pagamento -->
                    <RmSelect :model-value="state.form.planoPagamento"
                              placeholder="Selecione a condição de pagamento"
                              label="Condição de pagamento"
                              name="condicao de pagamento"
                              :rules="[obrigatorio]"
                              @update:model-value="mudarPlanoPagamento">
                        <option v-for="plan in state.planosPagamento" :key="plan.codigo" :value="plan.codigo">
                            {{ plan.descricao }}
                        </option>
                    </RmSelect>
                    <!-- Data do faturamento -->
                    <RmDateInput v-model="state.form.dataFaturamento"
                                 submit-label="Selecionar"
                                 type="date"
                                 name="data do faturamento"
                                 :rules="[obrigatorio, dataValida]"
                                 placeholder="Ex.: 01/01/2022"
                                 label="Data do faturamento" />
                    <!-- Número da OC -->
                    <RmInput v-model="state.form.numeroPedidoCompra"
                             name="número pedido"
                             placeholder="Ex.: 123456"
                             label="Número da OC do cliente" />
                </RmForm>
                <RmDivider class="my-5" />

                <!-- Produtos -->
                <div class="flex items-center mb-4">
                    <RmText type="headline-small">
                        Produtos
                    </RmText>
                    <RmLoading v-if="state.carregandoDetalheProduto" class="ml-5" />
                </div>

                <!-- Buscar produto -->
                <RmForm class="max-w-sm mb-4" @on:submit="obterProduto">
                    <RmInput v-model="state.buscaProduto"
                             name="codigoProduto"
                             placeholder="Informe o código do produto"
                             submit-icon="SearchIcon"
                             :show-submit="true"
                             :disabled="state.carregandoDetalheProduto" />
                </RmForm>

                <div class="overflow-x-auto">
                    <table class="red-mobile-table --small" style="min-width: 900px">
                        <tr>
                            <th class="w-10">
                                Item
                            </th>
                            <th class="w-12">
                                Cód.
                            </th>
                            <th class="text-left">
                                Produto
                            </th>
                            <th class="w-16">
                                Qtd
                            </th>
                            <th class="w-24">
                                Valor Unit.
                            </th>
                            <th class="w-18">
                                Desc. (%)
                            </th>
                            <th class="w-24">
                                Preço tabela
                            </th>
                            <th class="w-20">
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
                                Margem (%)
                            </th>
                            <th class="w-24">
                                Total
                            </th>
                            <th class="w-8" />
                        </tr>
                        <tr v-for="(produto, index) in state.produtos" :key="index">
                            <td>{{ index + 1 }}</td>
                            <td>{{ produto.id }}</td>
                            <td class="!text-left">
                                {{ produto.descricao }}
                            </td>
                            <td>
                                <input
                                    :value="produto.quantidade"
                                    :min="1"
                                    type="number"
                                    step="1"
                                    class="w-16 text-center py-1 border rounded dark:bg-gray-600"
                                    @input="definirQuantidadeProduto(+($event.target as HTMLInputElement).value, produto)">
                            </td>
                            <td>
                                <RmMoneyInput :model-value="produto.valorUnitario"
                                              prefix="R$ "
                                              :precision="2"
                                              :min="0"
                                              class="w-20 text-center py-1 border rounded dark:bg-gray-600"
                                              @update:model-value="(e) => definirValorUnitarioProduto(e, produto)" />
                            </td>
                            <td>
                                {{ formatarDecimal(calcularPercentualDesconto(produto.valorUnitario, produto.precoBase)) }}%
                            </td>
                            <td>R$ {{ formatarDecimal(produto.precoBase) }}</td>
                            <td>
                                <RmMoneyInput :model-value="produto.comissao"
                                              prefix=""
                                              suffix="%"
                                              :precision="2"
                                              :min="0"
                                              :max="produto.comissaoMaxima"
                                              class="w-20 text-center py-1 border rounded dark:bg-gray-600"
                                              @update:model-value="(e) => definirComissaoProduto(e, produto)" />

                            </td>

                            <td>{{ formatarDecimal(produto.percentualIPI) }}%</td>
                            <td>{{ formatarDecimal(produto.percentualICMS) }}%</td>
                            <td>{{ formatarDecimal(produto.percentualICMSST) }}%</td>
                            <td v-if="eGerente">
                                {{ formatarDecimal(produto.margem) }}%
                            </td>
                            <td>R$ {{ formatarDecimal(arredondarPreco(state.produtos[index].valorTotal)) }}</td>
                            <td>
                                <RmIconButton icon="TrashIcon"
                                              icon-class="w-6 fill-primary w-[1.15rem]"
                                              class="w-6 opacity-60"
                                              @click="removerProduto(index)" />
                            </td>
                        </tr>
                        <tr v-if="state.produtos.length == 0">
                            <td :colspan="tamanhoColspan">
                                Nenhum produto adicionado
                            </td>
                        </tr>
                    </table>
                </div>

                <RmDivider class="mb-5" />

                <!-- ========== ENTREGA E FATURAMENTO =========== -->
                <RmText type="headline-small" class="mb-4">
                    Entrega
                </RmText>

                <RmForm ref="freteForm" class="mb-4" :validate-on-input="state.validarAoDigitar">
                    <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-4">
                        <!-- Tipo de frete -->
                        <RmSelect v-model.number="state.form.modoFrete"
                                  label="Tipo de frete"
                                  placeholder="Selecione o tipo de frete"
                                  :rules="[obrigatorio]"
                                  name="tipo de frete"
                                  @update:model-value="mudarModoFrete">
                            <option value="0">
                                CIF
                            </option>
                            <option value="1">
                                FOB
                            </option>
                        </RmSelect>
                    </div>

                    <!-- Entrega agendada -->
                    <RmCheckbox v-model="state.form.agendado" label="Entrega agendada?" class="mb-5" />

                    <!-- Entregar no mesmo endereço do cliente -->
                    <RmCheckbox v-model="state.form.entregarNoEnderecoCliente" label="Entrega no mesmo endereço do cliente?" />
                </RmForm>

                <!-- ============== DADOS DO AGENDAMENTO =========-->
                <RmForm v-if="state.form.agendado" ref="agendamentoForm" :validate-on-input="state.validarAoDigitar">
                    <RmDivider class="my-5" />

                    <RmText type="headline-small" class="mb-4">
                        Dados do agendamento
                    </RmText>
                    <div class="max-w-sm mb-5">
                        <RmInput v-model="state.form.dadosAgendamento.email"
                                 name="email"
                                 placeholder="Ex.: red@mobile.com"
                                 label="E-mail do destinatário"
                                 class="mb-5"
                                 :rules="[emailValido]" />

                        <RmInput v-model="state.form.dadosAgendamento.telefone"
                                 name="telefone"
                                 mask="['(##) #####-####', '(##) ####-####']"
                                 placeholder="Ex.: (99) 99999-9999"
                                 label="Telefone para contato"
                                 class="mb-5"
                                 :rules="[obrigatorio, telefoneValido]" />

                    </div>
                </RmForm>

                <RmDivider class="mb-5" />

                <!-- ============== ENTREGA =========-->
                <RmForm v-if="!state.form.entregarNoEnderecoCliente" ref="entregaForm" :validate-on-input="state.validarAoDigitar">

                    <!-- Endereço de entrega -->
                    <RmText type="headline-small" class="mb-4">
                        Endereço de entrega
                    </RmText>
                    <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                        <RmInput v-model="state.form.enderecoEntrega.cep"
                                 name="cep"
                                 mask="#####-###"
                                 placeholder="Ex. 00000-00"
                                 label="Cep"
                                 :rules="[obrigatorio, cepValido]" />

                        <RmInput v-model="state.form.enderecoEntrega.logradouro"
                                 class="sm:col-span-2"
                                 name="Logradouro"
                                 placeholder="Ex.: Rua das Acácias"
                                 label="Logradouro"
                                 :rules="[obrigatorio]" />

                        <RmInput v-model="state.form.enderecoEntrega.numero"
                                 name="numero"
                                 placeholder="Ex.: 18"
                                 label="Número"
                                 mask="#####"
                                 :rules="[obrigatorio]" />

                        <RmInput v-model="state.form.enderecoEntrega.complemento"
                                 name="complemento"
                                 class="sm:col-span-2"
                                 placeholder="Ex. Bloco A"
                                 label="Complemento" />

                        <RmInput v-model="state.form.enderecoEntrega.bairro"
                                 name="bairro"
                                 placeholder="Ex.: Centro"
                                 label="Bairro"
                                 :rules="[obrigatorio]" />

                        <RmInput v-model="state.form.enderecoEntrega.cidade"
                                 name="cidade"
                                 placeholder="Ex. São Paulo"
                                 label="Cidade"
                                 :rules="[obrigatorio]" />

                        <RmSelect v-model="state.form.enderecoEntrega.estado"
                                  name="estado"
                                  placeholder="Selecione um estado"
                                  label="Estado"
                                  :rules="[obrigatorio]">
                            <option v-for="brasilianState in estadosBrasileiros" :key="brasilianState.initials" :value="brasilianState.initials">
                                {{ brasilianState.name }}
                            </option>
                        </RmSelect>
                    </div>
                    <RmDivider class="my-5" />
                </RmForm>

                <!-- ============== FATURAMENTO =========-->
                <RmForm ref="faturamentoForm" :validate-on-input="state.validarAoDigitar">
                    <RmText type="headline-small" class="mb-4">
                        Fiscal
                    </RmText>

                    <RmTextArea v-model="state.form.informacoesNota"
                                name="informações"
                                placeholder="Insira as informações para a NF"
                                label="Informações para a NF"
                                class="mb-4" />

                    <RmTextArea v-model="state.form.observacoesPedido"
                                name="observacoes"
                                :placeholder="`Insira as observações do ${titulo.toLowerCase()}`"
                                :label="`Observações do ${titulo.toLowerCase()}`"
                                class="mb-4" />
                </RmForm>
            </div>
        </RmCard>

        <div class="w-full 2xl:w-80 transition-opacity duration-300 mb-4 2xl:block" :class="{'opacity-0 hidden': !state.clienteSelecionado}">
            <RmCard class="!p-0 sticky top-4 flex flex-col items-stretch 2xl:h-novo-pedido-card">

                <RmText type="headline-small" class="p-4">
                    Resumo do {{ titulo.toLowerCase() }}
                </RmText>
                <RmDivider />

                <div class="flex-1 overflow-auto light-scroll p-4">
                    <div class="grid grid-cols-1 sm:grid-cols-4 lg:grid-cols-5 2xl:grid-cols-1 gap-4">
                        <RmTextField label="Status">
                            {{ state.statusPedido }}
                        </RmTextField>
                        <RmTextField label="Data de emissão">
                            {{ dataEmissao }}
                        </RmTextField>
                        <RmTextField label="Total de items">
                            {{ state.resumoPedido.totalItens }}
                        </RmTextField>
                        <RmTextField label="Total de peças">
                            {{ state.resumoPedido.totalVolumes }}
                        </RmTextField>
                        <RmTextField label="Total produtos">
                            R$ {{ formatarDecimal(state.resumoPedido.totalProdutos) }}
                        </RmTextField>
                        <RmTextField label="Total ICMS">
                            R$ {{ formatarDecimal(state.resumoPedido.totalICMS) }}
                        </RmTextField>
                        <RmTextField label="Total IPI">
                            R$ {{ formatarDecimal(state.resumoPedido.totalIPI) }}
                        </RmTextField>
                        <RmTextField label="Total ICMS ST">
                            R$ {{ formatarDecimal(state.resumoPedido.totalICMSST) }}
                        </RmTextField>
                        <RmTextField label="Peso líquido">
                            {{ formatarDecimal(state.resumoPedido.pesoLiquido) }} kg
                        </RmTextField>
                        <RmTextField label="Peso bruto">
                            {{ formatarDecimal(state.resumoPedido.pesoBruto) }} kg
                        </RmTextField>
                        <RmTextField v-if="eGerente" :label="`Margem do ${ titulo.toLowerCase() }`">
                            {{ formatarDecimal(state.resumoPedido.margemPedido) }}%
                        </RmTextField>
                    </div>
                </div>
                <RmDivider class="mb-3" />
                <div class="px-4 pb-4 max-w-lg">
                    <RmText type="headline-small">
                        Total da nota fiscal:
                    </RmText>
                    <RmText type="display-large" class="mb-2">
                        R$ {{ formatarDecimal(state.resumoPedido.totalNF) }}
                    </RmText>
                    <div class="flex items-center space-x-3">
                        <RmButton type="hollow" class="flex-1" @click="cancelarPedido">
                            {{ $route.name == 'novopedido' || $route.name == 'novoOrcamento' ? `Cancelar ${ titulo.toLowerCase() }` : `Excluir ${ titulo.toLowerCase() }` }}
                        </RmButton>
                        <RmButton class="flex-1" @click="submeter">
                            {{ $route.name == 'novopedido' || $route.name == 'novoOrcamento' ? `Finalizar ${ titulo.toLowerCase() }` : 'Finalizar edição' }}
                        </RmButton>
                    </div>
                </div>
            </RmCard>
        </div>

        <!-- Modal de carregando -->
        <RmLoadingModal :is-loading="state.enviandoPedido" />

        <!-- Modal de busca de cliente -->
        <DialogoBuscarCliente ref="modalCliente" />

        <!-- Modal de busca de produto -->
        <DialogoBuscarProduto ref="modalProduto" :selected-client-id="state.clienteSelecionado?.id || ''" />

        <RmModal :is-opened="state.modalExclusaoAberto">
            <template #default>
                <AlertIcon class="fill-primary w-15 mb-5" />
                <h2 class="font-bold mb-2">
                    Atenção
                </h2>
                <p class="text-sm opacity-80 whitespace-pre-line mb-8">
                    Para excluir o {{ titulo.toLowerCase() }}, escreva o código <b>{{ $route.params.id }}</b> abaixo:
                </p>
                <RmForm class="w-full">
                    <RmInput v-model="state.codigoExclusao" placeholder="Código" name="codigoExclusao" />
                </RmForm>
            </template>

            <!-- Error Footer -->
            <template #footer>
                <button class="modal-secondary" @click="state.modalExclusaoAberto = false">
                    Cancelar
                </button>
                <button class="modal-primary" :disabled="state.codigoExclusao != $route.params.id" @click="excluirPedido">
                    Excluir
                </button>
            </template>
        </RmModal>

        <!-- Modal de buscar representante -->
        <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />


        <!-- Modal de buscar gerente -->
        <RmBuscarGerenteModal ref="refModalBuscarGerente" />
    </div>
</template>
