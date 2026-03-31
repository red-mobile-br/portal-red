export default interface FiltroPadraoQuery {
    busca?: string;
    pagina?: number;
    tamanho?: number;
    de?: Date;
    ate?: Date;
    ordenarPor?: string;
    direcao?: 'asc' | 'desc';
    status?: string;
    idRepresentante?: string;
    idGerente?: string;
    idCliente?: string;
}