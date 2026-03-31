import LoginDTO from "../core/dtos/login/LoginDTO";
import RespostaLoginDTO from "../core/dtos/login/RespostaLoginDTO";
import { obter, publicar, atualizar } from "./base-service";
import VerificarUsuarioDTO from "../core/dtos/login/VerificarUsuarioDTO";
import RedefinirSenhaDTO from "../core/dtos/login/RedefinirSenhaDTO";

export default function useLoginService() {
    return {
        login: (dados: LoginDTO) => {
            return publicar<LoginDTO, RespostaLoginDTO>('/login', dados);
        },
        verificar: (cnpj: string) => {
            return obter<VerificarUsuarioDTO>(`/login/check/${cnpj}`);
        },
        redefinirSenha: (dados: RedefinirSenhaDTO) => {
            return atualizar('/login/resetpassword', dados);
        }
    };
}
