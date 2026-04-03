import PedidoDTO from "../dtos/pedido/PedidoDTO";
import statusPedidoEnumParser from '../enums/status-pedido-enum-parser';
import { formatarDecimal } from "../../utils/number-functions";
import { formatarData, mascaraCnpj } from "../../utils/string-functions";

interface ReportTemplateOptions {
    representante: string;
    planoPagamento: string;
    order: PedidoDTO;
    totalVolumes: string;
    totalProdutos: string;
    totalPesoLiquido: string;
    totalPesoBruto: string
}

export default function templateRelatorioPedido(data: ReportTemplateOptions) {
    const { representante, planoPagamento, order, totalVolumes, totalProdutos, totalPesoLiquido, totalPesoBruto } = data;

    return {
        'element': 'report',
        'logo': 'https://www.redmobile.com.br/image/catalog/Logo/Logo-Red-Mobile-bottom.png',
        'title': 'Relatório de pedido',
        'content': [
            {
                'element': 'flex',
                'children': [
                    {
                        'element': 'flex',
                        'direction': 'column',
                        'children': [
                            {
                                'element': 'text',
                                'type': 'headline4',
                                'marginBottom': 0.25,
                                'content': 'Código do pedido'
                            },
                            {
                                'element': 'text',
                                'type': 'headline-primary',
                                'marginBottom': 0,
                                'content': order.id ?? ''
                            }
                        ]
                    },
                    {
                        'element': 'divider',
                        'vertical': true
                    },
                    {
                        'element': 'flex',
                        'direction': 'column',
                        'children': [
                            {
                                'element': 'text',
                                'type': 'headline4',
                                'marginBottom': 0.25,
                                'content': 'Tipo'
                            },
                            {
                                'element': 'text',
                                'type': 'headline-primary',
                                'marginBottom': 0,
                                'content': order.tipoPedido == 0 ? 'Venda' : 'Bonificação'
                            }
                        ]
                    },
                    {
                        'element': 'divider',
                        'vertical': true
                    },
                    {
                        'element': 'flex',
                        'direction': 'column',
                        'children': [
                            {
                                'element': 'text',
                                'type': 'headline4',
                                'marginBottom': 0.25,
                                'content': 'Status'
                            },
                            {
                                'element': 'text',
                                'type': 'headline-primary',
                                'marginBottom': 0,
                                'content': statusPedidoEnumParser.get(order.status ?? '0')?.titulo
                            }
                        ]
                    }
                ]
            },
            {
                'element': 'divider'
            },
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Representante'
            },
            {
                'element': 'textField',
                'content': representante
            },
            {
                'element': 'divider'
            },
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Dados do cliente'
            },
            {
                'element': 'grid',
                'cols': 3,
                'gap': 0.75,
                'children': [
                    {
                        'element': 'textField',
                        'label': 'Identificador',
                        'content': order.cliente?.id ?? ''
                    },
                    {
                        'element': 'colSpan',
                        'colSpan': 2,
                        'child': {
                            'element': 'textField',
                            'label': 'Razão social',
                            content: order.cliente?.razaoSocial ?? ''
                        }
                    },
                    {
                        'element': 'textField',
                        'label': 'CNPJ',
                        'content': mascaraCnpj(order.cliente?.cnpj ?? '')
                    },
                    {
                        'element': 'colSpan',
                        'colSpan': 2,
                        'child': {
                            'element': 'textField',
                            'label': 'Nome fantasia',
                            'content': order.cliente?.nomeFantasia ?? ''
                        }
                    },
                    {
                        'element': 'textField',
                        'label': 'CEP',
                        'content': order.cliente?.endereco?.cep ?? ''
                    },
                    {
                        'element': 'colSpan',
                        'colSpan': 2,
                        'child': {
                            'element': 'textField',
                            'label': 'Logradouro',
                            'content': order.cliente?.endereco?.logradouro ?? ''
                        }
                    },
                    {
                        'element': 'textField',
                        'label': 'Número',
                        'content': order.cliente?.endereco?.numero ?? ''
                    },
                    {
                        'element': 'colSpan',
                        'colSpan': 2,
                        'child': {
                            'element': 'textField',
                            'label': 'Complemento',
                            'content': order.cliente?.endereco?.complemento || '-'
                        }
                    },
                    {
                        'element': 'textField',
                        'label': 'Bairro',
                        'content': order.cliente?.endereco?.bairro ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Município',
                        'content': order.cliente?.endereco?.cidade ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Estado',
                        'content': order.cliente?.endereco?.estado ?? ''
                    }
                ]
            },
            {
                'element': 'divider'
            },
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Entrega'
            },
            {
                'element': 'grid',
                'cols': 3,
                'gap': 0.75,
                'children': [
                    {
                        'element': 'colSpan',
                        'colSpan': 2,
                        'child': {
                            'element': 'textField',
                            'label': 'Logradouro',
                            'content': order.enderecoEntrega?.logradouro ?? ''
                        }
                    },
                    {
                        'element': 'textField',
                        'label': 'CEP',
                        'content': order.enderecoEntrega?.cep ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Número',
                        'content': order.enderecoEntrega?.numero ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Complemento',
                        'content': order.enderecoEntrega?.complemento || '-'
                    },
                    {
                        'element': 'textField',
                        'label': 'Bairro',
                        'content': order.enderecoEntrega?.bairro ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Município',
                        'content': order.enderecoEntrega?.cidade ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Estado',
                        'content': order.enderecoEntrega?.estado ?? ''
                    },
                    {
                        'element': 'textField',
                        'label': 'Tipo de frete',
                        'content': order.modoFrete == 0 ? 'CIF' : 'FOB'
                    }
                ]
            },
            {
                'element': 'pageBreak'
            },
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Dados do agendamento'
            },
            {
                element: 'grid',
                cols: 2,
                gap: 0.75,
                children: [
                    {
                        'element': 'textField',
                        'label': 'E-mail do destinatário',
                        'content': order.dadosAgendamento?.email,
                    },
                    {
                        'element': 'textField',
                        'label': 'Telefone para contato',
                        'content': order.dadosAgendamento?.telefone
                    }
                ]
            },
            {
                'element': 'divider'
            },
            // Produtos
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Produtos'
            },
            {
                'element': 'dataTable',
                'columns': [
                    { title: 'Item' },
                    { title: 'Cod' },
                    { title: 'Produto', "textAlign": "left" },
                    { title: 'Qtd' },
                    { title: 'Valor Unit.' },
                    { title: 'Preço tabela' },
                    { title: 'IPI' },
                    { title: 'ICMS' },
                    { title: 'ICMS ST' },
                    { title: 'Total' }
                ],
                rows: (order.produtos ?? []).map((product, index) => {
                    return [
                        index + 1,
                        product.id ?? '',
                        product.descricao ?? '',
                        product.quantidade ?? 0,
                        formatarDecimal(product.valorUnitario ?? 0),
                        formatarDecimal(product.precoBase ?? 0),
                        formatarDecimal(product.ipi ?? 0),
                        formatarDecimal(product.icms ?? 0),
                        formatarDecimal(product.icmsst ?? 0),
                        formatarDecimal(product.valorTotal ?? 0),
                    ];
                })
            },
            {
                'element': 'divider'
            },
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Fiscal e faturamento'
            },
            {
                'element': 'grid',
                cols: 3,
                gap: 0.75,
                children: [
                    {
                        'element': 'textField',
                        'label': 'NFs',
                        'content': (order.notasFiscais?.length ?? 0) > 0 ? order.notasFiscais!.join(', ') : '-',
                        'marginBottom': 1
                    },
                    {
                        'element': 'textField',
                        'label': 'Boletos',
                        'content': (order.boletos?.length ?? 0) > 0 ? order.boletos!.join(', ') : '-',
                        'marginBottom': 1
                    },
                    {
                        'element': 'textField',
                        'label': 'Número da OC',
                        'content': order.numeroPedidoCompra || '-',
                        'marginBottom': 1
                    },
                    {
                        'element': 'textField',
                        'label': 'Condição de pagamento',
                        'content': planoPagamento,
                        'marginBottom': 1
                    },
                    {
                        element: 'colSpan',
                        colSpan: 2,
                        child: {
                            'element': 'textField',
                            'label': 'Data do faturamento',
                            'content': formatarData(order.dataLancamento ?? ''),
                            'marginBottom': 1
                        }
                    }
                ]
            },
            {
                'element': 'textField',
                'label': 'Informações para a NF',
                'content': order.informacoesNota || '-',
                'marginBottom': 1
            },
            {
                'element': 'textField',
                'label': 'Observações do pedido',
                'content': order.observacoesPedido || '-'
            },
            {
                'element': 'divider'
            },
            {
                'element': 'text',
                'type': 'headline4',
                'content': 'Totais'
            },
            {
                'element': 'grid',
                'cols': 5,
                'gap': 0.75,
                children: [
                    {
                        'element': 'textField',
                        'label': 'Data de emissão',
                        'content': formatarData(order.dataEmissao ?? '')
                    },
                    {
                        'element': 'textField',
                        'label': 'Total de items',
                        'content': (order.produtos?.length ?? 0).toString()
                    },
                    {
                        'element': 'textField',
                        'label': 'Total de peças',
                        'content': totalVolumes
                    },
                    {
                        'element': 'textField',
                        'label': 'Total produtos',
                        'content': `R$ ${totalProdutos}`
                    },
                    {
                        'element': 'textField',
                        'label': 'Total ICMS',
                        'content': `R$ ${order.valorICMS ?? 0}`
                    },
                    {
                        'element': 'textField',
                        'label': 'Total IPI',
                        'content': `R$ ${order.valorIPI ?? 0}`
                    },
                    {
                        'element': 'textField',
                        'label': 'Total ICMS ST',
                        'content': `R$ ${order.valorICMSST ?? 0}`
                    },
                    {
                        'element': 'textField',
                        'label': 'Peso bruto',
                        'content': totalPesoBruto + 'kg'
                    },
                    {
                        'element': 'textField',
                        'label': 'Peso liquido',
                        'content': totalPesoLiquido + 'kg'
                    },
                    {
                        'element': 'textField',
                        'label': 'Total da nota fiscal',
                        'content': 'R$ ' + formatarDecimal(order.valorNota ?? 0)
                    }
                ]
            }
        ]
    };
}
