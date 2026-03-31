import ErroDTO from "../core/dtos/ErroDTO";
import PedidoDTO from "../core/dtos/pedido/PedidoDTO";
import PlanoPagamentoDTO from "../core/dtos/planosPagamento/PlanoPagamentoDTO";
import pedidoService from "../services/pedido-service";
import planoPagamentoService from "../services/plano-pagamento-service";
import { formatarDecimal, arredondarPreco } from "../utils/number-functions";
import statusPedidoEnumParser from '../core/enums/status-pedido-enum-parser';
import { computed, reactive, ref } from "vue";
import { useAlert } from "./alerta";
import useAppStore from '../store/app-store';
import { RmSearchDialogInstance } from "@/components/RmSearchDialog.vue";
import PedidoListaItemDTO from "@/core/dtos/pedido/PedidoListaItemDTO";

interface EstadoConsultarPedido {
    planosPagamento: PlanoPagamentoDTO[],
    carregando: boolean;
    numeroPedido: string;
    pedidoSelecionado: PedidoDTO | null
}


export function useDetalheProduto() {
    const alert = useAlert();

    const { nomeUsuario: nome } = useAppStore();
    const state = reactive<EstadoConsultarPedido>({
        planosPagamento: [],
        carregando: false,
        numeroPedido: '',
        pedidoSelecionado: null
    });

    const dialogoBuscarPedido = ref<RmSearchDialogInstance<PedidoListaItemDTO>>();

    function eIdPedidoValido(value: string) {
        if(value.length != 6) return false;

        return value.split('').every(el => !isNaN(+el));
    }

    const obterDetalhePedido = async (e: { isValid: boolean }) => {
        if(e.isValid && !state.carregando) {

            try {
                let consulta = state.numeroPedido;
                if(!eIdPedidoValido(consulta) && dialogoBuscarPedido.value) {
                    const pedido = await dialogoBuscarPedido.value.search(consulta);
                    if(!pedido) return;

                    consulta = pedido.id;
                }


                state.carregando = true;
                state.pedidoSelecionado = null;

                const [requisicao] = pedidoService.obterPorId(consulta);
                const [requisicaoPlano] = planoPagamentoService.obterPlanosPagamento();

                const [pedidoResp, planosResp] = await Promise.all([requisicao, requisicaoPlano]);

                state.pedidoSelecionado = pedidoResp;
                state.planosPagamento = planosResp;

            } catch (e) {
                const error = e as ErroDTO;
                alert({
                    titulo: 'Opss...',
                    mensagem: error.mensagem
                });
            } finally {
                state.carregando = false;
            }
        }
    };

    const interpretarModoFrete = (modoFrete: number) => {
        const modoFreteEnum: {[key:number]: string } = {
            0: 'CIF - Pago pelo Remetente',
            1: 'FOB - Pago pelo Destinatário'
        };

        return modoFreteEnum[modoFrete] ?? 'Não especificado';
    };

    // Propriedades computadas
    const statusPedido = computed(() =>
        state.pedidoSelecionado
            ? statusPedidoEnumParser.get(state.pedidoSelecionado.status)?.titulo ?? ''
            : ''
    );

    const tipoPedido = computed(() => state.pedidoSelecionado?.tipoPedido == 0 ? 'Venda' : 'Bonificação');

    const totalVolumes = computed(() => state.pedidoSelecionado?.produtos.reduce<number>((acc, p) => acc += p.quantidade, 0));

    const totalProdutos = computed(() => formatarDecimal(state.pedidoSelecionado?.valorProdutos || 0));

    const totalPesoBruto = computed(() => formatarDecimal(arredondarPreco(state.pedidoSelecionado?.pesoBruto || 0)));

    const totalPesoLiquido = computed(() => formatarDecimal(arredondarPreco(state.pedidoSelecionado?.pesoLiquido || 0)));

    const nomeUsuario = computed(() => nome.value);

    const planoPagamento = computed(() => state.pedidoSelecionado != null ? state.planosPagamento.find(el => el.codigo == state.pedidoSelecionado!.planoPagamento)?.descricao || '': '');

    return { state, obterDetalhePedido, interpretarModoFrete, totalVolumes, totalProdutos, totalPesoBruto, totalPesoLiquido, statusPedido, nomeUsuario, planoPagamento, tipoPedido, dialogoBuscarPedido };
}
