export const arredondarPreco = (value: number) => {
    return Math.ceil(value * 100) / 100;
};

export const calcularDesconto = (value: number, discount: number) => {
    return value - (value * (discount / 100));
};

export const formatarDecimal = (value: number) => {
    return value.toLocaleString('pt-BR', { minimumFractionDigits: 2, useGrouping: true });
};

export const obterInteiroDeDecimal = (value: number) => {
    return value.toLocaleString('pt-BR', { maximumFractionDigits: 0, useGrouping: true });
};

export function formatarBytes(bytes: number, decimals = 2) {
    if (!+bytes) return '0 Bytes';

    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;
    const sizes = ['bytes', 'kb', 'mb', 'gb', 'tb', 'pb', 'eb', 'zb', 'yb'];

    const i = Math.floor(Math.log(bytes) / Math.log(k));

    return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
}
