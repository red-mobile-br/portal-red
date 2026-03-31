<script lang="ts" setup>
import { computed, ref } from 'vue';
import * as icons from '../icons';
import RmModal from './RmModal.vue';

interface Opcao {
    titulo: string;
    acao?: (args?: any) => void;
    primario?: boolean;
}

const acoes = ref<Opcao[]>([]);
const icone = ref('');
const titulo = ref('Opss...');
const mensagem = ref('');
const aberto = ref(false);
const detalhe = ref('');

const exibirAlerta = (opcoes: { titulo?: string; mensagem: string; acoes?: Opcao[]; icone?: string; detalhe?: string}) => {
    acoes.value = opcoes.acoes || [];
    icone.value = opcoes.icone || 'AlertIcon';
    titulo.value = opcoes.titulo || "Opss...";
    mensagem.value = opcoes.mensagem;
    detalhe.value = opcoes.detalhe || '';
    aberto.value = true;
};

const executarAcao = (acao?: () => void) => {
    aberto.value = false;
    setTimeout(() => {
        if(acao != null) {
            acao();
        }
    }, 300);
};

const componente = computed(() => (icons as any)[icone.value]);

defineExpose({ exibirAlerta });
</script>

<template>
    <RmModal :is-opened="aberto">
        <template #default>
            <!-- Ícone -->
            <component :is="componente"
                       class="mb-5 w-15 fill-primary" />

            <!-- Título -->
            <h2 class="font-bold mb-2">
                {{ titulo }}
            </h2>

            <!-- Subtítulo -->
            <p class="text-sm opacity-80 whitespace-pre-line"
               v-html="mensagem" />

            <!-- Detalhe-->
            <p v-if="detalhe.length"
               class="alert__detail">
                <small>{{ detalhe }}</small>
            </p>
        </template>

        <!-- Rodapé -->
        <template #footer>
            <button v-if="acoes.length == 0"
                    class="modal-primary"
                    @click="aberto = false">
                Ok
            </button>
            <template v-else>
                <button v-for="(opcao, indice) in acoes"
                        :key="indice"
                        :class="[opcao.primario ? 'modal-primary' : 'modal-secondary']"
                        @click="executarAcao(opcao.acao)">
                    {{ opcao.titulo }}
                </button>
            </template>
        </template>
    </RmModal>
</template>
