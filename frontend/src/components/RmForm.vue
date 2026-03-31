<script lang="ts" setup>import { nextTick, provide, ref } from 'vue';

export interface FormField {
    name: string;
    error: string;
    validation: ((value: string) => string)[];
}

export interface FormInstance {
    submit: () => { isValid: boolean, errors: string[] };
}

const props = defineProps({
    validateOnInput: {
        type: Boolean,
        default: false
    }
});

const emit = defineEmits<{
    (e: 'on:submit', result: { isValid: boolean, errors: string[] }): void
}>();

const form = ref<HTMLFormElement | null>(null);
const isValid = ref(true);

// Array que armazenará a refencias para os inputs
const fields = ref<FormField[]>([]);
       
// Método que os inputs utilização para adicionar a sua referência ao form
const setField = (data: FormField) => {
    fields.value.push(data);
};

const removeField = (name: string) => {
    fields.value = fields.value.filter(el => el.name != name);
};

// Método de Validação
const validate = () => {
    if(form.value) {
        // Iremos criar um FormData para capturar todos os valores que estão dentro do Form
        const formData = new FormData(form.value!);
        // Iremos pegar os dados através do método entries()
        for(const [key, value] of (formData as any).entries()) {
            // Verificar se o campo é válido
            const field = fields.value.find(el => el.name == key);
        
            if(field) {
        
                // Realizar as validações e retornar o erro
                field.error = field.validation.reduce((acc, rule) => {
                    // Caso tenha erro, iremos adiciona-lo trancando o placeholder $NAME pelo nome do campo
                    return acc.length > 0 ? acc : rule(value.toString()).replace("$NAME", key);
                }, "");
            }
        }
        
        // Verificar se existe algum campo com erro
        isValid.value = fields.value.find(el => el.error.length > 0) == null;
    }
};

const submit = () => {
    validate();
    const valid = isValid.value;
    const result = { isValid: valid, errors: fields.value.filter(el => el.error.length > 0).map(el => el.error) };
    nextTick(() => {
        emit("on:submit", result);
        if (!valid) {
            const errorNames = new Set(
                fields.value.filter(f => f.error.length > 0).map(f => f.name)
            );
            const firstEl = Array.from(
                document.querySelectorAll<HTMLElement>('[data-field-name]')
            ).find(el => errorNames.has(el.dataset.fieldName ?? ''));
            firstEl?.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    });
    return result;
};

// Realizar a validação ao digitar apenas após o primeiro submit        
const onInput = () => {
    if(props.validateOnInput) {
        validate();
    }
};

// Dados providos aos elementos dentro do Slot
provide("fields", fields);
provide("addField", setField);
provide("onInput", onInput);
provide("removeField", removeField);

defineExpose({ submit });
</script>

<template>
    <form ref="form"
          novalidate
          @submit.prevent="submit">
        <slot :is-valid="isValid" />
    </form>
</template>