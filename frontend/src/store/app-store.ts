import { computed, reactive } from "vue";
import isAfter from "date-fns/isAfter";

import UsuarioDTO from "../core/dtos/login/UsuarioDTO";
import RespostaLoginDTO from "../core/dtos/login/RespostaLoginDTO";

import ConstantesArmazenamento from "../utils/constantes-armazenamento";

import { definirTokenBearer } from "../services/http_client/http";

interface EstadoAppStore {
    usuario: UsuarioDTO | null;
}

const state = reactive<EstadoAppStore>({
    usuario: null
});
export default function useAppStore() {


    const nomeUsuario = computed(() => {
        return state.usuario?.nome || "";
    });

    const idRepresentanteUsuario = computed(() => {
        return state.usuario?.idRepresentante || "";
    });

    const papelUsuario = computed(() => {
        return state.usuario?.tipoUsuario;
    });

    function verificarDadosUsuario(): boolean {
        const tokenAcesso = localStorage.getItem(ConstantesArmazenamento.tokenAcesso);
        const expiresInString = localStorage.getItem(ConstantesArmazenamento.expiraEm);
        const dadosUsuario = localStorage.getItem(ConstantesArmazenamento.usuario);

        // Verificar se existe um token
        if(!tokenAcesso || !expiresInString || !dadosUsuario) {
            state.usuario = null;
            definirTokenBearer("");
            return false;
        }

        // Verificar a data de validade
        const dataExpiracao = new Date(expiresInString);
        if(isAfter(new Date(), dataExpiracao)) {
            state.usuario = null;
            definirTokenBearer("");
            return false;
        }

        if(!state.usuario) {
            state.usuario = JSON.parse(dadosUsuario);
            definirTokenBearer(tokenAcesso);
        }
        return true;
    }

    function definirDadosUsuario(dados: RespostaLoginDTO) {
        localStorage.setItem(ConstantesArmazenamento.tokenAcesso, dados.tokenAcesso);
        localStorage.setItem(ConstantesArmazenamento.expiraEm, dados.expiraEm);
        localStorage.setItem(ConstantesArmazenamento.usuario, JSON.stringify(dados.dadosUsuario));
        definirTokenBearer(dados.tokenAcesso);
        state.usuario = dados.dadosUsuario;
    }

    return {
        nomeUsuario,
        papelUsuario,
        idRepresentanteUsuario,
        verificarDadosUsuario,
        definirDadosUsuario,
    };
}
