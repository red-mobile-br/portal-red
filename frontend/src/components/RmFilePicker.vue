<script lang="ts" setup>
import { PropType, ref, watch } from 'vue';
import { FileUploadIcon, FileIcon } from '@/icons';
import { formatarBytes } from '@/utils/number-functions';
import { TimesIcon } from '@/icons';

const props = defineProps({
    modelValue: {
        type: Array as PropType<File[]>,
        required: true
    },
    label: {
        type: String,
        required: true
    },
    allowMultiple: {
        type: Boolean,
        default: true
    }
});

const emits = defineEmits(['update:modelValue', 'on:error']);

const inputRef = ref<HTMLInputElement | null>(null);

const dragOver = ref(false);

watch(() => props.modelValue, () => {
    if(inputRef.value) {
        inputRef.value.value = '';
    }
});

const dragEnter = (e: DragEvent) => {
    e.preventDefault();
    dragOver.value = true;
};

const dragLeave = (e: DragEvent) => {
    e.preventDefault();
    dragOver.value = false;
};

const drop = (e: DragEvent) => {
    e.preventDefault();
    dragOver.value = false;
    const files = Array.from(e.dataTransfer!.files).filter(el => el.type == 'application/pdf');

    emits('update:modelValue', props.allowMultiple ?  [...props.modelValue, ...files] : [files[0]]);
};

const fileChange = (e: Event) => {
    const files = (e.target as HTMLInputElement).files;
    if(files != null && files.length) {
        emits('update:modelValue', props.allowMultiple ?  [...props.modelValue, ...Array.from(files)] : [files[0]]);
    }
    inputRef.value!.value = '';
};

const removeFile = (index: number) => {
    const newArray = [...props.modelValue];
    newArray.splice(index, 1);
    emits('update:modelValue', newArray);
};

</script>

<template>
    <div>
        <!-- Label -->
        <label class="text-sm font-semibold block mb-1.5 dark:text-white">
            {{ label }}
        </label>

        <div class="flex gap-2 flex-wrap">
            <div v-for="(file, index) in modelValue" :key="index"
                 class="rm-file-picker_file">
                <FileIcon class="w-8 fill-gray-900 dark:fill-white" />
                
                <div class="flex-1">
                    <p class="text-sm font-semibold line-clamp-2">
                        {{ file.name }}
                    </p>
                    <p class="text-xs opacity-80">
                        {{ formatarBytes(file.size) }}
                    </p>

                </div>
                <TimesIcon class="w-6 fill-gray-900 dark:fill-white cursor-pointer" @click="() => removeFile(index)" />
            
            </div>

            <label :class="[
                       dragOver ?'border-solid shadow-lg scale-105':'border-dashed',
                       modelValue.length ? 'w-24' : 'flex-1'
                   ]"
                   class="rm-file-picker"
                   @dragover="(e) => e.preventDefault()"
                   @dragenter="dragEnter"
                   @dragleave="dragLeave"
                   @drop="drop">
                   
                <FileUploadIcon class="mb-2 w-8 fill-gray-900 dark:fill-white" />
                <p class="text-xs text-gray-600 w-full text-center whitespace-pre-wrap leading-4">{{ dragOver ? "Solte para adicionar" : "Clique para adicionar" }}</p>
                <input ref="inputRef" type="file" accept="application/pdf" :multiple="allowMultiple" class="hidden" @change="fileChange">
            </label>
        </div>
    </div>
</template>

<style>
.rm-file-picker {
    @apply border border-gray-400 bg-gray-100 dark:bg-gray-800 dark:border-gray-600  h-24 
           rounded-md flex flex-col justify-center items-center transition-transform 
           duration-300 transform relative overflow-hidden cursor-pointer;
}

.rm-file-picker_file {
    @apply rounded-md bg-gray-100 dark:bg-gray-800 dark:border-gray-600 flex items-center gap-2 
           justify-center h-24 max-w-[16rem] border border-gray-300 p-4;
}

.rm-file-picker * {
    @apply pointer-events-none;
}
</style>