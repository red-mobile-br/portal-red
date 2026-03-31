import { Icons } from "@/icons";

interface ConteudoStatusTitulo {
    titulo: string;
    color: string;
    icon: Icons
}

const statusTituloEnumParser = new Map<string, ConteudoStatusTitulo>([
    ['0', { titulo: 'Em aberto', color: '#98D8D5', icon: 'PauseCircleIcon' }],
    ['1', { titulo: 'Vencido', color: '#F66A70', icon: 'InfoCircleIcon' }],
    ['2', { titulo: 'Quitado', color: '#7AC185', icon: 'CheckCircleIcon' }],
]);

// ['0', { titulo: 'Em aberto', color: '#98D8D5', icon: 'PauseCircleIcon' }],
// ['1', { titulo: 'Vencido', color: '#F66A70', icon: 'InfoCircleIcon' }],
// ['2', { titulo: 'Quitado', color: '#7AC185', icon: 'CheckCircleIcon' }],
// ['3', { titulo: 'Baixado parcialmente', color: '#F66A70', icon: 'CheckCircleIcon' }],

export default statusTituloEnumParser;
