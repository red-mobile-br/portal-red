import type { AxiosError } from 'axios';
import type { ApiError, ApiErrorResponse } from '@/core/interfaces/ApiError';

/**
 * Mensagem de erro padrão exibida quando o servidor retorna erro 500
 * ou quando a mensagem real não é amigável ao usuário.
 */
const MENSAGEM_ERRO_PADRAO = 'Não conseguimos processar sua solicitação. Tente novamente em breve.';

/**
 * Normaliza respostas de erro da API para um formato consistente.
 * Suporta tanto o formato legado (errorCode/errorMessage) quanto
 * o formato padronizado (code/message/detailedMessage).
 */
export function normalizarErroApi(error: AxiosError<ApiErrorResponse>): ApiError {
    const data = error.response?.data;
    const codigoStatus = error.response?.status ?? 500;

    // Extrair código, preferindo o formato padronizado
    const codigo = data?.code ?? data?.errorCode ?? codigoStatus;

    // Extrair mensagem, preferindo o formato padronizado
    let mensagem = data?.message ?? data?.errorMessage ?? MENSAGEM_ERRO_PADRAO;

    // Para erros de servidor (5xx), exibir mensagem amigável ao usuário
    if (codigo >= 500 || mensagem === 'Internal Server Error') {
        mensagem = MENSAGEM_ERRO_PADRAO;
    }

    return {
        codigo,
        mensagem,
        mensagemDetalhada: data?.detailedMessage
    };
}

/**
 * Verifica se um erro é um erro de API do Axios com dados de resposta.
 */
export function eErroApi(error: unknown): error is AxiosError<ApiErrorResponse> {
    return (
        typeof error === 'object' &&
        error !== null &&
        'isAxiosError' in error &&
        error.isAxiosError === true
    );
}

/**
 * Obtém uma mensagem de erro amigável de qualquer tipo de erro.
 * Use esta função para exibir erros ao usuário.
 */
export function obterMensagemErro(error: unknown): string {
    if (eErroApi(error)) {
        return normalizarErroApi(error).mensagem;
    }

    if (error instanceof Error) {
        return error.message;
    }

    if (typeof error === 'string') {
        return error;
    }

    return MENSAGEM_ERRO_PADRAO;
}

/**
 * Obtém a mensagem de erro detalhada se disponível, caso contrário a mensagem regular.
 * Use para logging de erros ou exibição detalhada.
 */
export function obterMensagemErroDetalhada(error: unknown): string {
    if (eErroApi(error)) {
        const erroApi = normalizarErroApi(error);
        return erroApi.mensagemDetalhada || erroApi.mensagem;
    }

    return obterMensagemErro(error);
}
