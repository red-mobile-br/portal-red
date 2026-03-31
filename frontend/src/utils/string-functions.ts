import format from "date-fns/format";
import formatISO from "date-fns/formatISO";
import parse from "date-fns/parse";
import { obterMensagemErro } from "./utilitarios-erro";

export const formatarFrase = (value: string): string => {
    const lowerCase = value.slice(1, value.length)?.toLowerCase();
    const upperCase = value[0]?.toUpperCase();
    if(lowerCase && upperCase) {
        return upperCase + lowerCase;
    }
    return value;
};

export const formatarTitulo = (value: string): string  => {
    return value.split(" ").map(word => {
        return formatarFrase(word);
    }).join(" ");
};


// Deixar bold texto da busca
export const negritarTexto = (text: string, textToCompare: string): string => {
    const hasWord = text.search(new RegExp(textToCompare, 'gi'));
    if(hasWord >= 0 && textToCompare.length > 0){
        return text.substr(0, hasWord) + '<b>' + text.substr(hasWord, textToCompare.length) + '</b>' + text.substring(hasWord + textToCompare.length, text.length);
    }
    else {
        return text;
    }
};

// Remover mascara de CNPJ
export const removerMascaraCnpj = (text: string) => {
    return text.replace(new RegExp('[./-]', 'gi'), '');
};

export const mascaraCnpj = (value: string) => {
    const part1 = value.substr(0, 2);
    const part2 = value.substr(2,3);
    const part3 = value.substr(5,3);
    const part4 = value.substr(8,4);
    const part5 = value.substr(12,2);
    return `${part1}.${part2}.${part3}/${part4}-${part5}`;
};

export const mascaraCpf = (value: string): string => {
    return `${value.substr(0,3)}.${value.substr(3,3)}.${value.substr(6,3)}-${value.substr(9,3)}`;
};

export const mascaraTelefone = (value?: string): string => {
    if(!value) return '';
    return value.length == 10
        ? `(${value.substr(0,2)}) ${value.substr(2,4)}-${value.substr(6,4)}`
        : `(${value.substr(0,2)}) ${value.substr(2, 5)}-${value.substr(7,4)}`;
};

export const mascaraCep = (value?: string): string => {
    if (!value) return '';
    const cleaned = value.replace(/\D/g, '');
    if (cleaned.length < 8) return cleaned;
    return `${cleaned.substr(0, 5)}-${cleaned.substr(5, 3)}`;
};

export const mascaraCodigoCliente = (value?: string | number): string => {
    if (!value) return '';
    const cleaned = value.toString().replace(/\D/g, '');
    if (cleaned.length < 8) return cleaned;
    return `${cleaned.substr(0, 6)}-${cleaned.substr(6, 2)}`;
};

export const removerMascaraCpf = (value: string): string => {
    return value.replace(new RegExp('[.-]', 'gi'), '');
};

export const dataParaIso = (value: string): string => {
    const date = parse(value, 'dd/MM/yyyy', new Date());
    return formatISO(date);
};

export const formatarData = (value: string): string => {
    return format(new Date(value), 'dd/MM/yyyy');
};

export const formatarDataHora = (value: string): string => {
    return format(new Date(value), 'dd/MM/yyyy HH:mm:ss');
};

export const removerCamposVazios = (value: {[key: string]: any }) => {
    const filteredObject = Object.entries(value).filter(([_, value]) => !!value);
    return Object.fromEntries(filteredObject);
};

/**
 * @deprecated Use obterMensagemErro from utilitarios-erro.ts instead
 */
export const mapAxiosError = (error: unknown): string => {
    return obterMensagemErro(error);
};
