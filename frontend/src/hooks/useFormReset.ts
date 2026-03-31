import { reactive, toRefs } from 'vue';

/**
 * Composable for managing form state with easy reset functionality
 * Eliminates manual field-by-field clearing
 *
 * @example
 * const { state, reiniciarFormulario } = useFormReset({ name: '', email: '' });
 * state.name = 'John';
 * reiniciarFormulario(); // Resets to initial values
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function useFormReset<T extends Record<string, any>>(initialState: T) {
    // Deep clone to prevent reference issues
    const obterEstadoInicial = (): T => structuredClone(initialState);

    const state = reactive(obterEstadoInicial());

    const reiniciarFormulario = () => {
        Object.assign(state, obterEstadoInicial());
    };

    const reiniciarCampo = <K extends keyof T>(field: K) => {
        (state as T)[field] = obterEstadoInicial()[field];
    };

    return {
        state,
        reiniciarFormulario,
        reiniciarCampo,
        ...toRefs(state)
    };
}
