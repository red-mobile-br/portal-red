<script lang="ts" setup>
import { AngleDoubleLeft } from '../../../icons';

defineProps({
    contracted: {
        type: Boolean,
        required: true,
    }
});

</script>
<template>
    <div class="h-12 flex items-center justify-center cursor-pointer hover:bg-black/10">
        <div class="transition-transform duration-300"
             :class="contracted ? 'rotate-180' : 'rotate-'">
            <AngleDoubleLeft class="w-6 fill-gray-400" />

        </div>
    </div>
</template>