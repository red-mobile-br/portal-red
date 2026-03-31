export default interface VerificarUsuarioDTO {
    possuiSenha: boolean;
    ativo: boolean;
    tokenRedefinicaoSenha: string | null;
}