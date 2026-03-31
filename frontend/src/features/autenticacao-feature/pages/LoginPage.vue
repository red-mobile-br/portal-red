<script lang="ts" setup>
import { reactive } from 'vue';
import { useRouter } from 'vue-router';

// Componentes
import { Logo } from '../../../illustrations';
import RmText from '../../../components/RmText.vue';
import RmForm from '../../../components/RmForm.vue';
import RmInput from '../../../components/RmInput.vue';
import RmLoading from '../../../components/RmLoading.vue';
import RmModal from '../../../components/RmModal.vue';
import PasswordIcon from '../../../icons/PasswordIcon.vue';

// Hooks
import { useAlert } from '../../../hooks/alerta';

// Services
import useLoginService from '../../../services/login-service';

// Utils
import { removerMascaraCnpj } from '../../../utils/string-functions';
import { obrigatorio, cnpjValido, emailValido } from '../../../utils/validadores';

// Interfaces
import ErroDTO from '../../../core/dtos/ErroDTO';
import useAppStore from '../../../store/app-store';

const router = useRouter();
const alert = useAlert();
const { login } = useLoginService();
const { definirDadosUsuario } = useAppStore();

const state = reactive({
    nomeUsuario: '',
    senha: '',
    carregando: false,
    validarAoDigitar: false,
    validarFormRedefinirAoDigitar: false,
    modalRedefinirSenhaAberto: false,
    emailRedefinirSenha: ''
});

function enviar(e: {isValid: boolean }) {
    state.validarAoDigitar = true;
    if(e.isValid && !state.carregando) {
        state.carregando = true;
        const [ requisicao ] = login({ nomeUsuario: removerMascaraCnpj(state.nomeUsuario), senha: state.senha });
        requisicao
            .then(resp => {
                definirDadosUsuario(resp);
                router.replace({ name: 'dashboard' });
            })
            .catch((error: ErroDTO) => {
                state.carregando = false;
                alert({
                    titulo: 'Opss...',
                    mensagem: error.mensagem
                });
            });
    }
}

function redefinirSenha(e: { isValid: boolean }) {
    state.validarFormRedefinirAoDigitar = true;
    if(e.isValid && !state.carregando) {
        state.modalRedefinirSenhaAberto = false;
        state.carregando = true;
        setTimeout(() => {
            state.emailRedefinirSenha = '';
            state.validarFormRedefinirAoDigitar = false;
            state.carregando = false;
            alert({
                titulo: "Redefinir senha",
                mensagem: "As instruções para redefinir sua senha foram enviadas para seu e-mail",
                icone: "email"
            });
        }, 4000);
    }
}
</script>

<template>
    <div class="w-full max-w-sm pb-10">
        <!-- Logo da empresa -->
        <Logo class="w-16" />

        <!-- Header -->
        <div class="w-full h-px bg-gray-200 my-6" />
        <RmText type="display-small">
            Bem-vindo representante
        </RmText>
        <RmText type="headline-medium"
                class="mb-10">
            Acesse o sistema com as suas credenciais
        </RmText>

        <RmForm :validate-on-input="state.validarAoDigitar"
                @on:submit="enviar">
            <!--  CNPJ -->
            <RmInput v-model="state.nomeUsuario"
                     label="CNPJ"
                     placeholder="Ex.: 00.000.000/0001-00"
                     name="cnpj"
                     type="tel"
                     :rules="[obrigatorio, cnpjValido]"
                     mask="##.###.###/####-##"
                     class="mb-3"
                     :disabled="state.carregando" />

            <!-- Senha -->
            <RmInput v-model="state.senha"
                     label="Senha"
                     type="password"
                     placeholder="Insira sua senha"
                     name="senha"
                     :show-submit="true"
                     :rules="[obrigatorio]"
                     :disabled="state.carregando" />
        </RmForm>

        <RmLoading class="my-5 opacity-0 transition-opacity duration-200"
                   :class="{'opacity-100': state.carregando}" />

        <div class="w-full h-px bg-gray-200 mb-2" />

        <!-- Recuperar senha -->
        <!-- <TextButton class="mx-auto" @click="carregando ? null : state.modalRedefinirSenhaAberto = true">Esqueci minha senha</TextButton> -->

        <RmModal :is-opened="state.modalRedefinirSenhaAberto">
            <template #default>
                <PasswordIcon class="mb-5 w-15 fill-primary" />
                <h2 class="font-bold mb-2">
                    Redefinir senha
                </h2>
                <p class="text-sm opacity-80 whitespace-pre-line mb-5">
                    Informe o seu e-mail para receber as instruções de redefinição de sua senha
                </p>
                <RmForm id="formRedefinir"
                        class="w-full"
                        :validate-on-input="state.validarFormRedefinirAoDigitar"
                        @on:submit="redefinirSenha">
                    <RmInput v-model="state.emailRedefinirSenha"
                             placeholder="Ex.: red@mobile.com"
                             name="email"
                             :rules="[obrigatorio, emailValido]" />
                </RmForm>
            </template>

            <!-- Rodapé -->
            <template #footer>
                <button class="modal-secondary"
                        @click="state.modalRedefinirSenhaAberto = false">
                    Voltar
                </button>
                <button class="modal-primary"
                        form="formRedefinir">
                    Continuar
                </button>
            </template>
        </RmModal>
    </div>
</template>
