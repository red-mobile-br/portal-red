/**
 * Formato de erro padronizado usado em toda a aplicação.
 * Espelha a estrutura do ModeloErro.cs do backend.
 */
export interface ApiError {
    codigo: number;
    mensagem: string;
    mensagemDetalhada?: string;
}

/**
 * Possíveis formatos de resposta de erro da API.
 * Suporta tanto o formato legado quanto o novo formato padronizado.
 * As propriedades estão em inglês pois representam os campos JSON retornados pelo backend.
 */
export interface ApiErrorResponse {
    // Formato padronizado (preferido)
    code?: number;
    message?: string;
    detailedMessage?: string;
    // Formatos legados (compatibilidade retroativa)
    errorCode?: number;
    errorMessage?: string;
}
