<script lang="ts" setup>
import { reactive, ref, watch } from 'vue';

// Components
import {
    RmCard,
    RmText,
    RmDivider,
    RmInput,
    RmSelect,
    RmForm,
    RmButton,
    RmIconButton,
    RmBuscarGerenteModal,
    BuscarGerenteModalInstancia } from '@/components';
import RmFilePicker from '@/components/RmFilePicker.vue';
import ValidacoesCliente from './components/ValidacoesCliente.vue';

// Services e DTOS
import { ClienteDetalhadoDTO, SocioInfoDTO, ReferenciaComercialDTO, DadosBancariosDTO } from '@/core/dtos/cliente/ClienteDetalhadoDTO';
import type EnderecoDTO from '@/core/dtos/endereco/EnderecoDTO';
import CepService from '@/services/cep-service';

type ClienteFormulario = Required<Omit<ClienteDetalhadoDTO, 'comprovantes' | 'contratoSocial' | 'documentoSintegra' | 'notasComerciais' | 'idGerente' | 'nomeGerente' | 'endereco' | 'enderecoEntrega' | 'socios' | 'referenciasComerciais' | 'dadosBancarios'>> & {
    endereco: Required<Omit<EnderecoDTO, 'numero'>> & { numero: number | '' };
    enderecoEntrega: Required<Omit<EnderecoDTO, 'numero'>> & { numero: number | '' };
    socios: (Required<Omit<SocioInfoDTO, 'percentual'>> & { percentual: number | '' })[];
    referenciasComerciais: Required<ReferenciaComercialDTO>[];
    dadosBancarios: Required<DadosBancariosDTO>;
    idGerente: string | null;
    nomeGerente: string | null;
};

import RmBuscarRepresentanteModal from '@/components/RmBuscarRepresentanteModal.vue';
import { BuscarRepresentanteModalInstancia } from '@/components/RmBuscarRepresentanteModal.vue';
import BotaoRepresentante from './components/BotaoRepresentante.vue';

// Utils
import { cepValido, obrigatorio, cnpjValido, telefoneValido, emailValido, cpfValido } from '@/utils/validadores';
import { estadosBrasileiros } from '@/utils/estados-brasileiros';
import { clienteService } from '@/services/cliente-service';
import { useAlert } from '@/hooks/alerta';
import RmLoadingModal from '@/components/RmLoadingModal.vue';
import { AxiosError } from 'axios';
import { mapAxiosError } from '@/utils/string-functions';
import useAppStore from '@/store/app-store';
import { useAutorizacao } from '@/hooks/autorizacao';

const { nomeUsuario, idRepresentanteUsuario } = useAppStore();
const { eGerente, eAdmin } = useAutorizacao();

const comprovantes = ref<File[]>([]);
const contratoSocial = ref<File[]>([]);
const notasComerciais = ref<File[]>([]);
const documentoSintegra = ref<File[]>([]);

const validarAoDigitar = ref(false);
const carregando = ref(false);

const refModalBuscarRepresentante = ref<BuscarRepresentanteModalInstancia>();
const refModalBuscarGerente = ref<BuscarGerenteModalInstancia | null>(null);

const alert = useAlert();

const form = reactive<ClienteFormulario>({
    tipoCliente: '',
    ramoAtividade: '',
    endereco: {
        cep: '',
        cidade: '',
        complemento: '',
        bairro: '',
        numero: '',
        uf: '',
        logradouro: ''
    },
    telefoneCobranca: '',
    cnae: '',
    cnpj: '',
    referenciasComerciais: [
        { nomeEmpresa: '', nomeContato: '', celular: '', telefone: '' }
    ],
    razaoSocial: '',
    nomeContato: '',
    email: '',
    fax: '',
    dataFundacao: '',
    celular: '',
    socios: [],
    telefone: '',
    capitalSocial: '',
    enderecoEntrega: {
        cep: '',
        cidade: '',
        complemento: '',
        bairro: '',
        numero: '',
        uf: '',
        logradouro: ''
    },
    nomeContatoEntrega: '',
    emailEntrega: '',
    telefoneEntrega: '',
    inscricaoEstadual: '',
    suframa: '',
    nomeFantasia: '',
    nomeRepresentante: nomeUsuario.value,
    idRepresentante: idRepresentanteUsuario.value,
    nomeGerente: eGerente ? nomeUsuario.value : null,
    idGerente: eGerente ? idRepresentanteUsuario.value : null,
    website: '',
    id: '',
    dadosBancarios: {
        numeroConta: '',
        nomeBanco: '',
        numeroBanco: '',
        nomeAgencia: '',
        numeroAgencia: ''
    },
});

function adicionarSocio() {
    form.socios.push({
        cpf: '',
        nome: '',
        percentual: '',
    });
}

function adicionarReferencia() {
    form.referenciasComerciais.push({
        nomeEmpresa: '',
        nomeContato: '',
        celular: '',
        telefone: '',
    });
}

function limparFormulario() {
    form.tipoCliente = '';
    form.ramoAtividade = '';
    form.endereco = {
        cep: '',
        cidade: '',
        complemento: '',
        bairro: '',
        numero: '',
        uf: '',
        logradouro: ''
    };
    form.telefoneCobranca = '';
    form.cnae = '';
    form.cnpj = '';
    form.referenciasComerciais = [];
    form.razaoSocial = '';
    form.nomeContato = '';
    form.email = '';
    form.fax = '';
    form.dataFundacao = '';
    form.celular = '';
    form.socios = [];
    form.telefone = '';
    form.capitalSocial = '';
    form.enderecoEntrega = {
        cep: '',
        cidade: '',
        complemento: '',
        bairro: '',
        numero: '',
        uf: '',
        logradouro: ''
    };
    form.nomeContatoEntrega = '';
    form.emailEntrega = '';
    form.telefoneEntrega = '';
    form.inscricaoEstadual = '';
    form.suframa = '';
    form.nomeFantasia = '';
    form.nomeRepresentante = nomeUsuario.value;
    form.idRepresentante = idRepresentanteUsuario.value;
    form.nomeGerente = eGerente ? nomeUsuario.value : null;
    form.idGerente = eGerente ? idRepresentanteUsuario.value : null;
    form.website = '';
    form.id = '';
    form.dadosBancarios = {
        numeroConta: '',
        nomeBanco: '',
        numeroBanco: '',
        nomeAgencia: '',
        numeroAgencia: ''
    };
}

/** Selecionar um representante */
const buscarRepresentante = async () => {
    const representante = await refModalBuscarRepresentante.value?.search();
    if(representante) {
        form.idRepresentante = representante.id ?? '';
        form.nomeRepresentante = representante.nome ?? '';
    }
};

/** Abrir modal de busca de gerentes */
const buscarGerente = async () => {
    const gerente = await refModalBuscarGerente.value?.search();
    if(gerente) {
        form.idGerente = gerente.id ?? null;
        form.nomeGerente = gerente.nome ?? null;
    }
};

async function enviar(e: {isValid: boolean }) {
    validarAoDigitar.value = true;

    if(!e.isValid) {
        alert({ mensagem: 'Alguns campos possuem erro de validação' });
        return;
    }
    try {
        carregando.value = true;
        const dadosFormulario = new FormData();

        for (const [chave, valor] of Object.entries(form)) {
            if(valor == null) continue;
            if(typeof valor == 'object') {
                dadosFormulario.append(chave, JSON.stringify(valor));
            }
            else {
                dadosFormulario.append(chave, valor.toString());
            }
        }

        comprovantes.value.forEach(el => dadosFormulario.append('comprovantes', el));
        contratoSocial.value.forEach(el => dadosFormulario.append('contratoSocial', el));
        notasComerciais.value.forEach(el => dadosFormulario.append('notasComerciais', el));
        documentoSintegra.value.forEach(el => dadosFormulario.append('documentoSintegra', el));


        const resposta = await clienteService.criarCliente(dadosFormulario);

        alert({
            icone: "checkCircle",
            titulo: `Cliente: ${resposta.id}`,
            mensagem: "Cliente cadastrado com sucesso!"
        });

        limparFormulario();
    } catch (error) {
        alert({
            mensagem: mapAxiosError(error as AxiosError)
        });
    }
    finally {
        carregando.value = false;
    }
}

/** Buscar endereço ao editar CEP */
watch(() => form.enderecoEntrega.cep, (newValue, oldValue) => {
    if(newValue != oldValue && newValue.length == 9) {
        CepService.buscarCep(newValue)
            .then(resp => {
                form.enderecoEntrega.logradouro = resp.logradouro;
                form.enderecoEntrega.bairro = resp.bairro;
                form.enderecoEntrega.cidade = resp.localidade;
                form.enderecoEntrega.uf = resp.uf;
            });
    }
});

watch(() => form.endereco.cep, (newValue, oldValue) => {
    if(newValue != oldValue && newValue.length == 9) {
        CepService.buscarCep(newValue)
            .then(resp => {
                form.endereco.logradouro = resp.logradouro;
                form.endereco.bairro = resp.bairro;
                form.endereco.cidade = resp.localidade;
                form.endereco.uf = resp.uf;
            });
    }
});

</script>
<template>
    <RmForm class="grid grid-cols-1 2xl:grid-cols-[minmax(0,_1fr)_20rem] gap-4 pb-4"
            :validate-on-input="validarAoDigitar"
            @on:submit="enviar">
        <div class="flex flex-col items-stretch gap-4 ">
            <RmCard>
                <BotaoRepresentante v-if="eGerente"
                              class="mb-4"
                              label="Gerente responsável"
                              :disabled="!eAdmin"
                              @on:click="buscarGerente">
                    {{ form.idGerente }} - {{ form.nomeGerente }}
                </BotaoRepresentante>

                <!-- Representante -->
                <BotaoRepresentante label="Representante responsável"
                              :disabled="!eGerente"
                              @on:click="buscarRepresentante">
                    {{ form.idRepresentante }} - {{ form.nomeRepresentante }}
                </BotaoRepresentante>
            </RmCard>

            <RmCard>
                <RmText type="headline-small" class="mb-4">
                    Dados do cliente
                </RmText>

                <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3">
                    <RmInput v-model="form.razaoSocial"
                             placeholder="Ex.: Empresa S.A."
                             label="Razão social" name="razão social"
                             class="sm:col-span-2 lg:col-span-4"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.nomeFantasia"
                             placeholder="Insira o nome fantasia"
                             label="Nome fantasia"
                             name="nome fantasia"
                             class="sm:col-span-2 lg:col-span-4"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.cnpj" 
                             placeholder="CNPJ"
                             label="CNPJ"
                             name="cnpj do cliente"
                             mask="**.***.***/****-##"
                             :rules="[obrigatorio, cnpjValido]" />

                    <RmInput v-model="form.ramoAtividade"
                             placeholder="Insira o ramo de atividade"
                             label="Ramo de atividade"
                             name="ramo de atividade" />

                    <RmInput v-model="form.capitalSocial"
                             placeholder="Capital social"
                             label="Capital social"
                             name="capital social"
                             mask="0.99"
                             mask-tokens="0:\d:multiple|9:\d:optional" />

                    <RmInput v-model="form.suframa"
                             placeholder="Suframa"
                             label="Suframa"
                             name="suframa" />

                    <RmInput v-model="form.dataFundacao"
                             placeholder="Data de fundação"
                             label="Data de fundação"
                             name="Data de fundação"
                             mask="##/##/####" />

                    <RmInput v-model="form.cnae" 
                             placeholder="CNAE" 
                             label="CNAE" 
                             name="CNAE" />

                    <RmInput v-model="form.inscricaoEstadual"
                             placeholder="Inscrição estadual"
                             label="Inscrição estadual"
                             name="Inscrição estadual"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.telefoneCobranca"
                             placeholder="Telefone faturamento"
                             label="Telefone faturamento"
                             name="telefone faturamento"
                             mask="['(##) #####-####', '(##) ####-####']"
                             :rules="[obrigatorio, telefoneValido]" />
                </div>
            </RmCard>

            <RmCard>
                <RmText type="headline-small" class="mb-4">
                    Contatos
                </RmText>

                <div class="grid grid-cols-1 sm:grid-cols-4 gap-3">
                    <RmInput v-model="form.nomeContato"
                             placeholder="Nome do contato"
                             label="Nome do contato"
                             name="nome do contato"
                             class="sm:col-span-2"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.celular"
                             placeholder="Celular"
                             label="Celular"
                             name="celular"
                             mask="['(##) #####-####', '(##) ####-####']"
                             class="sm:col-span-2"
                             :rules="[obrigatorio, telefoneValido]" />

                    <RmInput v-model="form.email"
                             placeholder="Email"
                             label="Email"
                             name="email"
                             :rules="[obrigatorio, emailValido]" />

                    <RmInput v-model="form.telefone"
                             placeholder="Telefone"
                             label="Telefone"
                             mask="['(##) #####-####', '(##) ####-####']"
                             name="phone"
                             :rules="[obrigatorio, telefoneValido]" />

                    <RmInput v-model="form.fax"
                             placeholder="Fax"
                             label="Fax"
                             mask="['(##) #####-####', '(##) ####-####']"
                             name="fax" 
                             :rules="[telefoneValido]" />

                    <RmInput v-model="form.website"
                             placeholder="Site" 
                             label="Site" 
                             name="website" />
                </div>
            </RmCard>

            <RmCard>
                <RmText type="headline-small" class="mb-4">
                    Endereço de faturamento
                </RmText>
                <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                    <RmInput v-model="form.endereco.cep"
                             name="cep faturamento"
                             mask="#####-###"
                             placeholder="Ex. 00000-00"
                             label="Cep"
                             :rules="[obrigatorio, cepValido]" />

                    <RmInput v-model="form.endereco.logradouro"
                             class="sm:col-span-2"
                             name="logradouro faturamento"
                             placeholder="Ex.: Rua das Acácias"
                             label="Logradouro"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.endereco.numero"
                             name="numero faturamento"
                             placeholder="Ex.: 18"
                             label="Número"
                             mask="#####"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.endereco.complemento"
                             name="complemento faturamento"
                             class="sm:col-span-2"
                             placeholder="Ex. Bloco A"
                             label="Complemento" />

                    <RmInput v-model="form.endereco.bairro"
                             name="bairro faturamento"
                             placeholder="Ex.: Centro"
                             label="Bairro"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.endereco.cidade"
                             name="cidade faturamento"
                             placeholder="Ex. São Paulo"
                             label="Cidade"
                             :rules="[obrigatorio]" />

                    <RmSelect v-model="form.endereco.uf"
                              name="estado faturamento"
                              placeholder="Selecione um estado"
                              label="Estado"
                              :rules="[obrigatorio]">
                        <option v-for="estadoBrasileiro in estadosBrasileiros" :key="estadoBrasileiro.initials" :value="estadoBrasileiro.initials">
                            {{ estadoBrasileiro.name }}
                        </option>
                    </RmSelect>
                </div>

            </RmCard>

            <RmCard class="flex flex-col items-stretch gap-4">
                <RmText type="headline-small">
                    Sócios
                </RmText>
                
                <div v-for="(socio, indice) in form.socios" :key="indice" class="flex flex-col sm:flex-row items-stretch sm:items-start gap-4">

                    <RmInput v-model="socio.nome" class="sm:flex-[2]"
                             placeholder="Nome do sócio"
                             label="Nome do sócio"
                             :name="`nome do sócio ${indice + 1}`"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="socio.percentual"
                             class="flex-1"
                             placeholder="Porcentagem"
                             label="Porcentagem"
                             mask="###"
                             :name="`porcentagem do sócio ${indice + 1}`"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="socio.cpf"
                             class="flex-1"
                             placeholder="CPF"
                             label="CPF"
                             mask="###.###.###-##"
                             :name="`CPF do sócio ${indice + 1}`"
                             :rules="[obrigatorio, cpfValido]" />

                    <RmIconButton icon="TimesIcon"
                                  class="w-6 sm:mt-9 fill-red-500 max-sm:self-center"
                                  @click.stop.prevent="() => form.socios.splice(indice, 1)" />
                </div>
                <RmButton type="hollow" @click.stop.prevent="adicionarSocio">
                    Adicionar sócio
                </RmButton>
            </RmCard>

            <RmCard>

                <RmText type="headline-small" class="mb-4">
                    Dados da entrega
                </RmText>
               

                <div class="grid grid-cols-1 sm:grid-cols-3 gap-3">
                    <RmInput v-model="form.nomeContatoEntrega"
                             placeholder="Nome do contato"
                             label="Nome do contato"
                             name="contato nfe"
                             class="sm:col-span-3"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.telefoneEntrega"
                             placeholder="Telefone"
                             label="Telefone"
                             name="telefone nfe"
                             mask="['(##) #####-####', '(##) ####-####']"
                             :rules="[obrigatorio, telefoneValido]" />

                    <RmInput v-model="form.emailEntrega"
                             placeholder="E-mail NFe"
                             label="E-mail NFe"
                             name="email nfe"
                             class="sm:col-span-2"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.enderecoEntrega.cep"
                             name="cep"
                             mask="#####-###"
                             placeholder="Ex. 00000-00"
                             label="Cep"
                             :rules="[obrigatorio, cepValido]" />

                    <RmInput v-model="form.enderecoEntrega.logradouro"
                             class="sm:col-span-2"
                             name="Logradouro"
                             placeholder="Ex.: Rua das Acácias"
                             label="Logradouro"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.enderecoEntrega.numero"
                             name="numero"
                             placeholder="Ex.: 18"
                             label="Número"
                             mask="#####"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.enderecoEntrega.complemento"
                             name="complemento"
                             class="sm:col-span-2"
                             placeholder="Ex. Bloco A"
                             label="Complemento" />

                    <RmInput v-model="form.enderecoEntrega.bairro"
                             name="bairro"
                             placeholder="Ex.: Centro"
                             label="Bairro"
                             :rules="[obrigatorio]" />

                    <RmInput v-model="form.enderecoEntrega.cidade"
                             name="cidade"
                             placeholder="Ex. São Paulo"
                             label="Cidade"
                             :rules="[obrigatorio]" />

                    <RmSelect v-model="form.enderecoEntrega.uf"
                              name="estado"
                              placeholder="Selecione um estado"
                              label="Estado"
                              :rules="[obrigatorio]">
                        <option v-for="estadoBrasileiro in estadosBrasileiros" :key="estadoBrasileiro.initials" :value="estadoBrasileiro.initials">
                            {{ estadoBrasileiro.name }}
                        </option>
                    </RmSelect>
                </div>
            </RmCard>

            <RmCard class="flex flex-col items-stretch gap-4">
                <RmText type="headline-small">
                    Referências comerciais
                </RmText>

                <div>
                    <div v-for="(referencia, indice) in form.referenciasComerciais" :key="indice" class="flex items-start pb-4 gap-2">
                        <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4 flex-1">
                            <RmInput v-model="referencia.nomeContato"
                                     placeholder="Nome"
                                     label="Nome"
                                     :name="`contato ref ${indice + 1}`"
                                     :rules="[obrigatorio]" />

                            <RmInput v-model="referencia.nomeEmpresa"
                                     placeholder="Empresa"
                                     label="Empresa"
                                     :name="`empresa ref ${indice + 1}`"
                                     :rules="[obrigatorio]" />

                            <RmInput v-model="referencia.celular"
                                     placeholder="Celular"
                                     label="Celular"
                                     mask="['(##) #####-####', '(##) ####-####']"
                                     :name="`celular ref ${indice + 1}`"
                                     :rules="[obrigatorio, telefoneValido]" />

                            <RmInput v-model="referencia.telefone"
                                     placeholder="Telefone"
                                     label="Telefone"
                                     mask="['(##) #####-####', '(##) ####-####']"
                                     :name="`telefone ref ${indice + 1}`"
                                     :rules="[obrigatorio, telefoneValido]" />
                        </div>

                        <RmIconButton icon="TimesIcon" class="w-6 mt-9 fill-red-500" @click.stop.prevent="() => form.referenciasComerciais.splice(indice, 1)" />
                    </div>
                </div>

                <RmButton type="hollow" @click.stop.prevent="adicionarReferencia">
                    Adicionar referência
                </RmButton>
            </RmCard>

            <RmCard class="flex flex-col items-stretch gap-4">
                <RmText type="headline-small">
                    Dados bancários
                </RmText>

                <div class="grid grid-cols-4 gap-4">
                    <RmInput v-model="form.dadosBancarios.nomeBanco"
                             placeholder="Nome do banco"
                             label="Nome do banco"
                             name="banco"
                             class="col-span-4" />

                    <RmInput v-model="form.dadosBancarios.numeroBanco"
                             placeholder="Nº do banco"
                             label="Nº do banco"
                             name="numero do banco" />

                    <RmInput v-model="form.dadosBancarios.nomeAgencia"
                             placeholder="Nome da agência"
                             label="Nome da agência"
                             name="nome da agencia" />

                    <RmInput v-model="form.dadosBancarios.numeroAgencia"
                             placeholder="Nº da agência"
                             label="Nº da agência"
                             name="agencia" />

                    <RmInput v-model="form.dadosBancarios.numeroConta"
                             placeholder="Nº da conta corrente"
                             label="Nº da conta corrente"
                             name="conta corrente" />
                </div>
            </RmCard>

            <RmCard class="flex flex-col items-stretch gap-4">
                <RmText type="headline-small">
                    Documentos em anexo
                </RmText>
                <RmFilePicker v-model="contratoSocial"
                              label="Cópia de contrato social"
                              :allow-multiple="false" />

                <RmFilePicker v-model="documentoSintegra"
                              label="Cópia da tela do sintegra"
                              :allow-multiple="false" />

                <RmFilePicker v-model="comprovantes"
                              label="Cópia de NF's" />

                <RmFilePicker v-model="notasComerciais"
                              label="Cópia de duplicatas pagas" />
            </RmCard>
        </div>
 
        <RmCard class="!p-0 sticky top-4 flex flex-col items-stretch 2xl:h-novo-pedido-card">
            <RmText type="headline-small" class="p-4">
                Validações
            </RmText>
            <RmDivider />
            <div class="flex-1">
                <ValidacoesCliente :form="(form as unknown as ClienteDetalhadoDTO)" :trade-notes="notasComerciais" :sintegra-document="documentoSintegra" :receipts="comprovantes" :social-contract-document="contratoSocial" />
            </div>
            <RmDivider />
                        
            <div class="p-4">
                <RmButton class="max-w-sm mx-auto">
                    Finalizar cadastro
                </RmButton>

            </div>
        </RmCard>

        <RmLoadingModal :is-loading="carregando" />

        <!-- Modal de buscar representante -->
        <RmBuscarRepresentanteModal ref="refModalBuscarRepresentante" />


        <!-- Modal de buscar gerente -->
        <RmBuscarGerenteModal ref="refModalBuscarGerente" />
    </RmForm>
    
</template>