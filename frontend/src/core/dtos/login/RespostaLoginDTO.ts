import UsuarioDTO from "./UsuarioDTO";

export default interface RespostaLoginDTO {
    tokenAcesso: string;
    expiraEm: string;
    dadosUsuario: UsuarioDTO;
}
