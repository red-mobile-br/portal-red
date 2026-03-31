import useAppStore from "../store/app-store";
import { computed } from "vue";

export function useAutorizacao() {
    const { papelUsuario } = useAppStore();

    const eGerente = computed(() => papelUsuario.value! >= 1);
    const eAdmin = computed(() => papelUsuario.value! >= 2);

    return { eGerente, eAdmin };
}
