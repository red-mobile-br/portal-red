import { Icons } from "../../icons";

interface ConteudoStatusPedido {
    titulo: string;
    color: string;
    icon: Icons
}

const statusPedidoEnumParser = new Map<string, ConteudoStatusPedido>([
    ['A', { titulo: 'Todos em aberto', color: '#98D8D5', icon: "PauseCircleIcon" }],
    ['0', { titulo: 'Novo pedido', color: '#98D8D5', icon: "PauseCircleIcon" }],
    ['1', { titulo: 'Em análise comercial', color: '#D689CE', icon: "PauseCircleIcon" }],
    ['2', { titulo: 'Em análise financeira', color: '#BED051', icon: "PauseCircleIcon" }],
    ['3', { titulo: 'Em separação', color: '#4D93BA', icon: "ClockCircleIcon" }],
    ['4', { titulo: 'Aguardando faturamento', color: '#4DBA78', icon: "ClockCircleIcon" }],
    ['F', { titulo: 'Faturado', color: '#7AC185', icon: "CheckCircleIcon"  }],
    ['P', { titulo: 'Faturado parcialmente', color: '#BED051', icon: "CheckCircleIcon" }],
    ['X', { titulo: 'Cancelado', color: '#F66A70', icon: "TimesCircleIcon" }],
    ['N', { titulo: 'Negado financeiro', color: '#F66A70', icon: "InfoCircleIcon" }],
    ['M', { titulo: 'Bloqueado por margem', color: '#BED051', icon: 'ClockCircleIcon' }],
    ['R', { titulo: 'Rejeitado por margem', color: '#F66A70', icon: 'TimesCircleIcon' }],
    ['V', { titulo: 'Revisão financeiro', color: '#4DBA78', icon: 'ClockCircleIcon' }]
]);
// '-': { titulo: 'Pedido de venda encerrado', color: '' },
// '--': { titulo: 'Pedido de venda liberado', color: '' },
// '---': { titulo: 'Pedido de venda com bloqueio de regra', color: '' },
// '----': { titulo: 'Bloqueio de margem', color: '' },

export default statusPedidoEnumParser;
