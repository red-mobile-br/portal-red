import EnderecoDTO from "../endereco/EnderecoDTO";

export default interface ClienteDTO {
    cnpj: string;
    razaoSocial: string;
    email: string;
    id: string;
    nomeFantasia: string;
    telefone: string;
    endereco: EnderecoDTO;
    idRepresentante: string;
    nomeRepresentante: string;
}
