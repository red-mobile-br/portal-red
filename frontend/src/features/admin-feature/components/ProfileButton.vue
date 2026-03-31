<script lang='ts' setup>
import ArrowDownIcon from '../../../icons/ArrowDownIcon.vue';
import RmCircleAvatar from '../../../components/RmCircleAvatar.vue';

defineProps({
    nomeUsuario: {
        type: String,
        required: true
    }
});
</script>

<template>
    <button class="items-center h-12 rounded-md cursor-pointer md:hover:bg-gray-100 dark:hover:bg-gray-800 px-2 flex">
        <p class="text-sm hidden lg:block">
            Olá, <span class="font-semibold">{{ nomeUsuario }}</span>
        </p>
        <RmCircleAvatar size="2.5rem"
                        class="ml-3 mr-1.5"
                        :name="nomeUsuario" />
        <ArrowDownIcon class="hidden lg:flex fill-primary w-6" />
    </button>
</template>