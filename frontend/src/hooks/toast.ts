import { Plugin, createApp } from 'vue';
import RmToast from '../components/RmToast.vue';

interface OpcoesToast {
    mensagem: string;
    icone?: string;
    persistente?: boolean;
    carregando?: boolean;
}

interface InstanciaToast {
    exibir: (opcoes: OpcoesToast) => number;
    dispensar: (id: number) => void;
}

let instanciaToast: InstanciaToast;

let useToast: () => (opcoes: OpcoesToast) => number;
let useDismissToast: () => (id: number) => void;

const alert: Plugin = {
    install: () => {
        const toastRoot = createApp(RmToast).mount(document.body.appendChild(document.createElement('div'))) as unknown as InstanciaToast;
        instanciaToast = toastRoot;
        useToast = () => toastRoot.exibir;
        useDismissToast = () => toastRoot.dispensar;
    }
};

export { useToast, useDismissToast };

export default alert;
