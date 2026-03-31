<script lang="ts" setup>
defineProps({
    title: {
        type: String,
        required: false,
        default: ''
    }
});
document.documentElement.classList.remove('dark');
</script>

<template>
    <div class="print-page bg-white shadow-lg mb-8 last:mb-0 print:shadow-none print:mb-0 mx-auto pl-1 pr-0">
        <table class="w-full border-collapse">
            <thead>
                <tr>
                    <td class="p-0 relative">
                        <div class="py-2 flex items-center justify-between border-b border-gray-300 mb-1">
                            <img src="@/assets/img/logo.svg" class="w-14">
                            <p v-if="title" class="text-sm font-semibold">
                                {{ title }}
                            </p>
                        </div>
                    </td>
                </tr>
            </thead>

            <tbody>
                <tr>
                    <td class="p-0">
                        <slot />
                    </td>
                </tr>
            </tbody>

            <tfoot>
                <tr>
                    <td>
                        <div style="height: 1cm" class="print:hidden" />
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
</template>

<style>
.print-page {
    min-height: 297.1mm;
}
@media print {
    .print-page {
        min-height: auto;
    }
    .print-page * {
        -webkit-print-color-adjust: exact;
        -moz-print-color-adjust: exact;
        print-color-adjust: exact;
    }
}
</style>