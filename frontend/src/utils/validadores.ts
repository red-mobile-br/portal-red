export const emailValido = (value: string): string => {
    const isValid = (/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/).test(value);
    return !isValid ? "Este campo precisa ser um e-mail válido" : "";
};

export const obrigatorio = (value: string): string => {
    const isValid = (value && value.toString() as string).trim().length > 0;
    return !isValid ? "O campo $NAME é obrigatório" : "";
};
(obrigatorio as any).__isRequired = true;

export const cpfValido = (value: string): string => {
    const isValid = (/[0-9]{3}.[0-9]{3}.?[0-9]{3}-[0-9]{2}/i).test(value);
    return !isValid ? "O campo $NAME precisa ser um CPF válido" : "";
};

export const cnpjValido = (value: string): string => {
    const isValid = (/\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}/i).test(value);
    return !isValid ? "O campo $NAME precisa ser um cnpj válido" : "";
};

export const cepValido = (value: string): string => {
    const isValid = (/\d{5}\-\d{3}/i).test(value);
    return !isValid ? "O campo $NAME precisa ser um cep válido" : "";
};

export const telefoneValido = (value: string): string => {
    const isValid = value == '' || (/\(\d{2,}\) \d{4,}-\d{4}/).test(value);
    return !isValid ? "O campo $NAME precisa ser um telefone válido" : "";
};

export const eIgual = (value: string, valueToCompare: string): string => {
    const isValid = value === valueToCompare;
    return !isValid ? "Os campos não conferem" : "";
};

export const dataValida = (value: string): string => {
    const isValid = (/(^(((0[1-9]|1[0-9]|2[0-8])[/](0[1-9]|1[012]))|((29|30|31)[/](0[13578]|1[02]))|((29|30)[/](0[4,6,9]|11)))[/](19|[2-9][0-9])\d\d$)|(^29[/]02[/](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)/).test(value);
    return !isValid ? "O campo $NAME precisa ser uma data válida": "";
};

export const mesValido = (value: string): string => {
    const isValid = (/((0[1-9])|(1[0-2]))\/(\d{4})/).test(value);
    return !isValid ? "Insira um mês válido" : "";
};

export const senhaValida = (value: string): string => {
    const hasNoSpecialCharacters = (/^[\w&.-]*$/).test(value);
    const isValid = value.length >= 6 && hasNoSpecialCharacters;
    return !isValid ? "A senha precisa possuir no mínimo 6 caracteres e não pode conter caracteres especiais" : '';
};

export const semCaracteresEspeciais = (value: string): string => {
    const noSpace = value.replaceAll(' ', '');
    const isValid = (/^[\w&.-]*$/).test(noSpace);

    return !isValid ? "Este campo não pode possuir caracteres especiais" : "";
};

export const tamanhoMaximo = (value: string, tamanhoMaximo: number): string => {
    return value.length > tamanhoMaximo ? `Este campo pode ter no máximo ${tamanhoMaximo} caracteres` : '';
};

export const horaValida = (value: string): string => {
    const validFormat = (/[0-9]{2}:[0-9]{2}/).test(value);
    let horaValida = false;

    if(validFormat) {
        const splitted = value.split(':');
        const hours = parseInt(splitted[0]);
        const minutes = parseInt(splitted[1]);

        horaValida = hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59;
    }

    return !validFormat || !horaValida ? "O campo $NAME precisa ser uma hora válida": "";
};

export const emailOuCpfValido = (value: string) => {
    const emailError = emailValido(value);
    const emailCpf = cpfValido(value);

    return !emailError || !emailCpf ? '' : "O campo precisa ser um e-mail ou cpf válido";
};
