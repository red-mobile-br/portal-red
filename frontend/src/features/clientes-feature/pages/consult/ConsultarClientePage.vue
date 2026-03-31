<script lang="ts" setup>
import { nextTick, onMounted, reactive, ref, watch } from 'vue';
import { RmCard, RmText, RmForm, RmTextField, RmInput, RmLoading } from '@/components';
import DialogoBuscarCliente, { SearchCustomerDialogInstance } from '@/features/pedidos-feature/pages/create/components/DialogoBuscarCliente.vue';
import { cnpjValido } from '@/utils/validadores';
import { clienteService } from '@/services/cliente-service';
import { ClienteDetalhadoDTO } from '@/core/dtos/cliente/ClienteDetalhadoDTO';
import ErroDTO from '@/core/dtos/ErroDTO';
import { mascaraCnpj, removerMascaraCnpj, mascaraTelefone } from '@/utils/string-functions';
import orderIllustration from '@/assets/illustrations/order-illustration.svg';
import UltimosPedidosCliente from './components/UltimosPedidosCliente.vue';
import { useRoute } from 'vue-router';
import { useAlert } from '@/hooks/alerta';

interface ConsultarClientePageState {
    consulta: string;
    carregando: boolean;
    clienteSelecionado: ClienteDetalhadoDTO | null;

}
const route = useRoute();
const alert = useAlert();

const dialogoCliente = ref<SearchCustomerDialogInstance | null>(null);
const state = reactive<ConsultarClientePageState>({
    consulta: '',
    carregando: false,
    clienteSelecionado: null,
});

async function obterDetalheCliente() {
    try {

        state.carregando = true;

        let cnpj = '';
        // Caso seja um CNPJ, buscar detalhe do cliente
        const cnpjEValido = cnpjValido(state.consulta).length == 0;
        if(cnpjEValido) {
            cnpj = state.consulta;
        }
        else {
            const cliente = await dialogoCliente.value!.search(state.consulta);
            if(!cliente) {
                return;
            }
            cnpj = cliente.cnpj;
        }

        const [ requisicaoCliente ] = clienteService.obterDetalheCliente(cnpj);

        state.clienteSelecionado = await requisicaoCliente;
    } catch (e) {
        const error = e as ErroDTO;
        const mensagem = error.codigo == 404 ? 'Não foi encontrando nenhum cliente com o CNPJ informado' : error.mensagem;
        alert({ mensagem: mensagem });
    }
    finally {
        state.carregando = false;
    }
}

function limpar() {
    state.consulta = '';
}

// Observadores
/** Adicionar máscara caso digite um CNPJ válido */
watch(() => state.consulta, (novoValor, valorAntigo) => {
    // Verificar se o valor novo é um cnpj válido para ser tratado
    const cnpjEValido = (/[0-9]{14}/i).test(novoValor) && novoValor.length == 14;
    if(cnpjEValido) {
        state.consulta = mascaraCnpj(novoValor);
    }
    // Caso o valor antigo seja um cnpj com mascara, retirar a mascara
    const cnpjComMascara = cnpjValido(valorAntigo).length == 0;
    if(cnpjComMascara) {
        state.consulta = removerMascaraCnpj(novoValor);
    }
});

onMounted(() => {
    if(route.params.cnpj?.length) {
        state.consulta = route.params.cnpj.toString();
        nextTick(() => {
            obterDetalheCliente();

        });
    }
});

</script>

<template>
    <div class="flex flex-col items-stretch gap-4 pb-4 flex-1">
        <RmCard class="flex flex-col items-stretch gap-4">
            <RmText type="headline-small">
                Informe o nome ou CNPJ do cliente
            </RmText>

            <!-- Formulário de buscar pedido -->
            <RmForm class="max-w-sm" @on:submit="obterDetalheCliente">

                <!-- Buscar cliente -->
                <RmInput v-model="state.consulta"
                         name="empresa"
                         placeholder="Ex.: 123.456.789-10"
                         submit-icon="SearchIcon"
                         :disabled="state.carregando"
                         :show-clear-button="state.consulta.length > 0 || state.clienteSelecionado != null"
                         :show-submit="true"
                         @on:clear="limpar" />
            </RmForm>

        </RmCard>

        <Transition name="fade" mode="out-in">
            <RmCard v-if="state.clienteSelecionado == null" class="flex-1 flex flex-col items-center justify-center">
                <img :src="orderIllustration" class="w-64 mb-4 opacity-60">
                <div class="h-10 flex items-center justify-center opacity-60">
                    <RmLoading v-if="state.carregando" />
                    <p v-else class="text-sm text-center font-semibold text-gray-400 w-52">
                        Selecione um pedido para obter mais informações
                    </p>
                </div>
            </RmCard>

            <div v-else class="flex flex-col items-stretch gap-4">
                <RmCard>
                    <RmText type="headline-small" class="mb-4">
                        Representante
                    </RmText>

                    <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                        <RmTextField label="Cód. Representante">
                            {{ state.clienteSelecionado.idRepresentante }}
                        </RmTextField>
                        <RmTextField label="Nome representante" class="sm:col-span-2">
                            {{ state.clienteSelecionado.nomeRepresentante }}
                        </RmTextField>
                    </div>
                </RmCard>

                <RmCard>
                    <RmText type="headline-small" class="mb-4">
                        Dados do cliente
                    </RmText>

                    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3">
                        <RmTextField label="Razão social" class="sm:col-span-2 lg:col-span-4">
                            {{ state.clienteSelecionado.razaoSocial }}
                        </RmTextField>
                        <RmTextField label="Nome fantasia" class="sm:col-span-2 lg:col-span-4">
                            {{ state.clienteSelecionado.nomeFantasia }}
                        </RmTextField>
                        <RmTextField label="CNPJ">
                            {{ mascaraCnpj(state.clienteSelecionado.cnpj) }}
                        </RmTextField>
                        <RmTextField label="Ramo de atividade">
                            {{ state.clienteSelecionado.ramoAtividade }}
                        </RmTextField>
                        <RmTextField label="Capital social">
                            {{ state.clienteSelecionado.capitalSocial }}
                        </RmTextField>
                        <RmTextField label="Suframa">
                            {{ state.clienteSelecionado.suframa }}
                        </RmTextField>
                        <RmTextField label="Data de fundação">
                            {{ state.clienteSelecionado.dataFundacao }}
                        </RmTextField>
                        <RmTextField label="CNAE">
                            {{ state.clienteSelecionado.cnae }}
                        </RmTextField>
                        <RmTextField label="Inscrição estadual">
                            {{ state.clienteSelecionado.inscricaoEstadual }}
                        </RmTextField>
                        <RmTextField label="Telefone faturamento">
                            {{ state.clienteSelecionado.telefoneCobranca }}
                        </RmTextField>
                    </div>
                </RmCard>

                <RmCard>

                    <RmText type="headline-small" class="mb-4">
                        Contatos
                    </RmText>

                    <div class="grid grid-cols-1 sm:grid-cols-4 gap-3">
                        <RmTextField label="Nome do contato" class="sm:col-span-2">
                            {{ state.clienteSelecionado.nomeContato }}
                        </RmTextField>
                        <RmTextField label="Email" class="sm:col-span-2">
                            {{ state.clienteSelecionado.email }}
                        </RmTextField>
                        <RmTextField label="Celular">
                            {{ mascaraTelefone(state.clienteSelecionado.celular) }}
                        </RmTextField>
                        <RmTextField label="Telefone">
                            {{ mascaraTelefone(state.clienteSelecionado.telefone) }}
                        </RmTextField>
                        <RmTextField label="Fax">
                            {{ mascaraTelefone(state.clienteSelecionado.fax) }}
                        </RmTextField>
                        <RmTextField label="Site">
                            {{ state.clienteSelecionado.website }}
                        </RmTextField>
                    </div>
                </RmCard>

                <RmCard>
                    <RmText type="headline-small" class="mb-4">
                        Endereço de faturamento
                    </RmText>
                    <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                        <RmTextField label="Cep">
                            {{ state.clienteSelecionado.endereco.cep }}
                        </RmTextField>

                        <RmTextField label="Logradouro" class="sm:col-span-2">
                            {{ state.clienteSelecionado.endereco.logradouro }}
                        </RmTextField>

                        <RmTextField label="Número">
                            {{ state.clienteSelecionado.endereco.numero }}
                        </RmTextField>

                        <RmTextField label="Complemento" class="sm:col-span-2">
                            {{ state.clienteSelecionado.endereco.complemento }}
                        </RmTextField>

                        <RmTextField label="Bairro">
                            {{ state.clienteSelecionado.endereco.bairro }}
                        </RmTextField>

                        <RmTextField label="Cidade">
                            {{ state.clienteSelecionado.endereco.cidade }}
                        </RmTextField>

                        <RmTextField label="Estado">
                            {{ state.clienteSelecionado.endereco.estado }}
                        </RmTextField>
                    
                    </div>

                </RmCard>

                <RmCard class="flex flex-col items-stretch gap-4">
                    <RmText type="headline-small">
                        Sócios
                    </RmText>
                    <div v-for="(partner, index) in state.clienteSelecionado.socios ?? []" :key="index" class="flex items-center gap-4">
                        <RmTextField label="Nome do sócio" class="flex-[2]">
                            {{ partner.nome }}
                        </RmTextField>
                        <RmTextField label="Porcentagem" class="flex-1">
                            {{ partner.percentual }}
                        </RmTextField>
                        <RmTextField label="CPF" class="flex-1">
                            {{ partner.cpf }}
                        </RmTextField>
                    </div>
                    <p v-if="!state.clienteSelecionado.socios?.length" type="" class="text-center text-sm">
                        Não há sócios informados
                    </p>
                </RmCard>

                <RmCard>

                    <RmText type="headline-small" class="mb-4">
                        Dados da entrega
                    </RmText>
               

                    <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                        <RmTextField label="Nome do contato" class="col-span-3">
                            {{ state.clienteSelecionado.nomeContatoEntrega }}
                        </RmTextField>
                        <RmTextField label="Telefone">
                            {{ state.clienteSelecionado.telefoneEntrega }}
                        </RmTextField>
                        <RmTextField label="E-mail NFe" class="col-span-2">
                            {{ state.clienteSelecionado.emailEntrega }}
                        </RmTextField>
                        <RmTextField label="CEP">
                            {{ state.clienteSelecionado.enderecoEntrega?.cep }}
                        </RmTextField>
                        <RmTextField label="Logradouro" class="sm:col-span-2">
                            {{ state.clienteSelecionado.enderecoEntrega?.logradouro }}
                        </RmTextField>
                        <RmTextField label="Número">
                            {{ state.clienteSelecionado.enderecoEntrega?.numero }}
                        </RmTextField>
                        <RmTextField label="Complemento" class="sm:col-span-2">
                            {{ state.clienteSelecionado.enderecoEntrega?.complemento }}
                        </RmTextField>
                        <RmTextField label="Bairro">
                            {{ state.clienteSelecionado.enderecoEntrega?.bairro }}
                        </RmTextField>
                        <RmTextField label="Cidade">
                            {{ state.clienteSelecionado.enderecoEntrega?.cidade }}
                        </RmTextField>
                        <RmTextField label="Estado">
                            {{ state.clienteSelecionado.enderecoEntrega?.estado }}
                        </RmTextField>
                    </div>
                </RmCard>

                <RmCard>
                    <RmText type="headline-small" class="mb-4">
                        Dados da cobrança
                    </RmText>

                    <div>
                        <RmTextField label="Telefone">
                            {{ state.clienteSelecionado.telefoneCobranca }}
                        </RmTextField>
                    </div>
                </RmCard>

                <RmCard class="flex flex-col items-stretch gap-4">
                    <RmText type="headline-small">
                        Referências comerciais
                    </RmText>

                    <div v-for="(reference, index) in state.clienteSelecionado.referenciasComerciais"
                         :key="index"
                         class="grid grid-cols-4 gap-4 flex-1 border-b last:border-b-0 border-gray-200">
                        <RmTextField label="Nome">
                            {{ reference.nomeContato }}
                        </RmTextField>
                        <RmTextField label="Empresa">
                            {{ reference.razaoSocial }}
                        </RmTextField>
                        <RmTextField label="Celular">
                            {{ reference.telefone }}
                        </RmTextField>
                        <RmTextField label="Telefone">
                            {{ reference.celular }}
                        </RmTextField>
                    </div>
                    <p v-if="!state.clienteSelecionado.referenciasComerciais?.length" type="" class="text-center text-sm">
                        Não há referências comerciais informadas
                    </p>
                </RmCard>

                <RmCard class="flex flex-col items-stretch gap-4">
                    <RmText type="headline-small">
                        Documentos em anexo
                    </RmText>
                    
                    <RmTextField label="Cópia do contrato social">
                        {{ state.clienteSelecionado.contratoSocial?.nome }}
                    </RmTextField>
                    <RmTextField label="Cópia da tela do Sintegra">
                        {{ state.clienteSelecionado.documentoSintegra?.nome }}
                    </RmTextField>
                    <RmTextField label="Cópia de NF's">
                        {{ state.clienteSelecionado.comprovantes?.map(a => a.nome).join(', ') }}
                    </RmTextField>
                    <RmTextField label="Trade notes">
                        {{ state.clienteSelecionado.notasComerciais?.map(a => a.nome).join(', ') }}
                    </RmTextField>

          
                </RmCard>

                <RmCard class="flex flex-col items-stretch gap-4">
                    <RmText type="headline-small">
                        Dados bancários
                    </RmText>

                    <div v-for="(bankData, index) in state.clienteSelecionado.dadosBancarios ?? []"
                         :key="index"
                         class="grid grid-cols-4 gap-4 flex-1 border-b last:border-b-0 border-gray-300">
                        <RmTextField label="Nome do banco" class="col-span-4">
                            {{ bankData.nomeBanco }}
                        </RmTextField>
                        <RmTextField label="Nº do banco">
                            {{ bankData.numeroBanco }}
                        </RmTextField>
                        <RmTextField label="Nome da agência">
                            {{ bankData.nomeAgencia }}
                        </RmTextField>
                        <RmTextField label="Nº da agência">
                            {{ bankData.numeroAgencia }}
                        </RmTextField>
                        <RmTextField label="Nº da conta">
                            {{ bankData.numeroConta }}
                        </RmTextField>
                    </div>
                    <p v-if="!state.clienteSelecionado.dadosBancarios?.length" class="text-center text-sm">
                        Não há dados bancários informados
                    </p>
                </RmCard>

                <UltimosPedidosCliente :cnpj="state.clienteSelecionado.cnpj" />
            </div>
        </Transition>
        <!-- Modal de busca de cliente -->
        <DialogoBuscarCliente ref="dialogoCliente" />
    </div>

</template>