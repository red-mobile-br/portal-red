import { ref, watch, Ref } from 'vue';

/**
 * Composable for reactive localStorage with TypeScript support
 * Automatically syncs reactive value with localStorage
 *
 * @param chave - localStorage key
 * @param valorPadrao - Default value if key doesn't exist
 * @param opcoes - Configuration options
 *
 * @example
 * const { valor, remover } = useLocalStorage('user', null);
 * valor.value = { id: 1, name: 'John' }; // Automatically saved to localStorage
 */
export function useLocalStorage<T>(
    chave: string,
    valorPadrao: T,
    opcoes: {
        /** Watch for changes and auto-save (default: true) */
        watch: boolean;
        /** Custom serializer (default: JSON.stringify) */
        serializador?: (value: T) => string;
        /** Custom deserializer (default: JSON.parse) */
        desserializador?: (value: string) => T;
    } = { watch: true }
) {
    const serializador = opcoes.serializador ?? JSON.stringify;
    const desserializador = opcoes.desserializador ?? JSON.parse;

    // Read initial value from localStorage
    const ler = (): T => {
        try {
            const item = localStorage.getItem(chave);
            return item ? desserializador(item) : valorPadrao;
        } catch (error) {
            console.warn(`Error reading localStorage key "${chave}":`, error);
            return valorPadrao;
        }
    };

    const valor: Ref<T> = ref(ler()) as Ref<T>;

    // Write to localStorage
    const escrever = (novoValor: T) => {
        try {
            localStorage.setItem(chave, serializador(novoValor));
        } catch (error) {
            console.error(`Error writing localStorage key "${chave}":`, error);
        }
    };

    // Remove from localStorage
    const remover = () => {
        try {
            localStorage.removeItem(chave);
            valor.value = valorPadrao;
        } catch (error) {
            console.error(`Error removing localStorage key "${chave}":`, error);
        }
    };

    // Auto-save when value changes
    if (opcoes.watch) {
        watch(
            valor,
            (novoValor) => escrever(novoValor),
            { deep: true }
        );
    }

    return {
        valor,
        escrever,
        remover
    };
}
