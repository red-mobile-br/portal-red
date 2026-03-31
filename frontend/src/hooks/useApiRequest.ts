import { ref, Ref } from 'vue';
import { obterMensagemErro } from '@/utils/utilitarios-erro';

export interface EstadoRequisicaoApi<T> {
    carregando: Ref<boolean>;
    erro: Ref<string | null>;
    dados: Ref<T | null>;
}

/**
 * Composable for managing API request state (loading, error, data pattern)
 * Standardizes the common pattern of loading states, error handling, and data storage
 *
 * @example
 * const { carregando, erro, dados, executar } = useApiRequest<UsuarioDTO>();
 *
 * async function fetchUser() {
 *   await executar(async () => {
 *     const [request] = userService.getUser(id);
 *     return await request;
 *   });
 * }
 */
export function useApiRequest<T>() {
    const carregando = ref(false);
    const erro = ref<string | null>(null);
    const dados = ref<T | null>(null);

    /**
     * Execute an API request with automatic loading and error state management
     * @param funcaoRequisicao - Async function that returns the API data
     */
    const executar = async (funcaoRequisicao: () => Promise<T>): Promise<T | null> => {
        try {
            carregando.value = true;
            erro.value = null;

            const resultado = await funcaoRequisicao();
            dados.value = resultado as T & null;

            return resultado;
        } catch (e) {
            erro.value = obterMensagemErro(e);
            dados.value = null;
            return null;
        } finally {
            carregando.value = false;
        }
    };

    /**
     * Reset all state to initial values
     */
    const reiniciar = () => {
        carregando.value = false;
        erro.value = null;
        dados.value = null;
    };

    return {
        carregando,
        erro,
        dados,
        executar,
        reiniciar
    };
}
