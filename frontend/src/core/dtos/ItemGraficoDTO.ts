export default interface ItemGraficoDTO {
    rotulo: string;
    series: {
        nome: string;
        valor: number
    }[]
}
