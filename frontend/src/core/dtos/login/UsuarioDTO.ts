export default interface UsuarioDTO {
    nome?: string;
    urlFoto?: string | null;
    tipoUsuario?: 0 | 1 | 2;
    nomeUsuario?: string;
    idRepresentante?: string;
}
