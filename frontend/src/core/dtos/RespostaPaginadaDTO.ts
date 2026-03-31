export default interface RespostaPaginadaDTO<M> {
    numeroPagina: number;
    totalPaginas: number;
    dados: M[]
}
