<script lang="ts" setup>
defineProps({
    columns: {
        type: Number,
        required: true
    }
});
</script>

<template>
    <tr>
        <td v-for="column in columns" :key="column">
            <div class="h-5 w-full rounded animate-pulse bg-gray-200 dark:bg-gray-800 my-0.5" />
        </td>
    </tr>
</template>