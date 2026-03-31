import axios, { Canceler, AxiosResponse, AxiosError } from 'axios';
import $http from './http_client/http';
import { normalizarErroApi } from '@/utils/utilitarios-erro';
import type { ApiErrorResponse, ApiError } from '@/core/interfaces/ApiError';

export type ObjetoServico<R> = [Promise<R>, Canceler | null];

const TEMPO_LIMITE_PADRAO = 45000;

/**
 * Trata erros da API normalizando-os para um formato consistente.
 */
function tratarErroApi(error: AxiosError<ApiErrorResponse>): ApiError {
    return normalizarErroApi(error);
}

/**
 * Função genérica para requisições POST e PUT.
 */
export const enviar = <M, R>(dados: {
    url: string;
    corpo: M;
    metodo: 'post' | 'put';
    opcoes?: { onProgress?: (progresso: number) => void; timeout?: number }
}): ObjetoServico<R> => {
    const { url, corpo, metodo, opcoes } = dados;
    let cancelador: Canceler | undefined;

    const requisicao = new Promise<R>((resolve, reject) => {
        $http[metodo]<M, AxiosResponse<R>>(url, corpo, {
            cancelToken: new axios.CancelToken((c) => cancelador = c),
            onUploadProgress: (e) => {
                const percentual = Math.round((e.loaded / e.bytes) * 100);
                opcoes?.onProgress?.(percentual);
            },
            timeout: opcoes?.timeout || TEMPO_LIMITE_PADRAO
        })
            .then((resp) => resolve(resp.data))
            .catch((error: AxiosError<ApiErrorResponse>) => reject(tratarErroApi(error)));
    });

    return [requisicao, cancelador] as ObjetoServico<R>;
};

export const publicar = <M, R>(
    url: string,
    corpo: M,
    opcoes?: { onProgress?: (progresso: number) => void; timeout?: number }
): ObjetoServico<R> => {
    return enviar<M, R>({ corpo, metodo: 'post', url, opcoes });
};

export const atualizar = <M, R>(
    url: string,
    corpo?: M,
    opcoes?: { onProgress?: (progresso: number) => void; timeout?: number }
): ObjetoServico<R> => {
    return enviar<M, R>({ corpo: corpo as M, metodo: 'put', url, opcoes });
};

export const excluir = <R>(
    url: string,
    opcoes?: { onProgress?: (progresso: number) => void; timeout?: number }
): ObjetoServico<R> => {
    let cancelador: Canceler | undefined;

    const requisicao = new Promise<R>((resolve, reject) => {
        $http.delete<void, AxiosResponse<R>>(url, {
            cancelToken: new axios.CancelToken((c) => cancelador = c),
            onUploadProgress: (e) => {
                const percentual = Math.round((e.loaded / e.bytes) * 100);
                opcoes?.onProgress?.(percentual);
            },
            timeout: opcoes?.timeout || TEMPO_LIMITE_PADRAO
        })
            .then((resp) => resolve(resp.data))
            .catch((error: AxiosError<ApiErrorResponse>) => reject(tratarErroApi(error)));
    });

    return [requisicao, cancelador] as ObjetoServico<R>;
};

export const obter = <R>(
    url: string = '',
    query?: Record<string, unknown> | unknown
): ObjetoServico<R> => {
    let cancelador: Canceler | null = null;

    const requisicao = new Promise<R>((resolve, reject) => {
        $http.get(url, {
            params: query,
            cancelToken: new axios.CancelToken((c) => cancelador = c)
        })
            .then((resp: AxiosResponse<R>) => resolve(resp.data))
            .catch((error: AxiosError<ApiErrorResponse>) => reject(tratarErroApi(error)));
    });

    return [requisicao, cancelador];
};
