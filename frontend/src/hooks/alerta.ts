import { Plugin, createApp } from 'vue';
import RmAlert from '../components/RmAlert.vue';

interface Opcao {
    titulo: string;
    acao?: (args?: any) => void;
    primario?: boolean;
}

let useAlert: () => ((opcoes: { titulo?: string; mensagem: string; acoes?: Opcao[]; icone?: string; detalhe?: string}) => void);

const alerta: Plugin = {
    install: () => {
        const alertRoot = createApp(RmAlert).mount(document.body.appendChild(document.createElement('div')));
        useAlert = () => (alertRoot as any).exibirAlerta;
    }
};

export { useAlert };

export default alerta;
