<script lang="ts" setup>
import { ref, watch, onMounted } from 'vue';

const props = defineProps({
    modelValue: {
        type: Number,
        required: true
    },
    prefix: {
        type: String,
        default: 'R$ '
    },
    suffix: {
        type: String,
        default: ''
    },
    disabled: {
        type: Boolean,
        default: false
    },
    min: {
        type: Number,
        default: 0
    },
    max: {
        type: Number,
        default: Infinity
    },
    precision: {
        type: Number,
        default: 2
    }
});

const emit = defineEmits<{
    (e: 'update:modelValue', value: number): void
}>();

const inputRef = ref<HTMLInputElement>();
const displayValue = ref('');
const isFocused = ref(false);

const formatNumber = (value: number): string => {
    if (isNaN(value)) return '';
    const formatted = value.toLocaleString('pt-BR', { minimumFractionDigits: props.precision, maximumFractionDigits: props.precision });
    return formatted + (props.suffix ? ' ' + props.suffix : '');
};

const parseValue = (text: string): number => {
    // Remove prefix, suffix, then convert PT-BR format to parseable number
    const cleaned = text
        .replace(props.prefix, '')
        .replace(props.suffix, '')
        .replace(/\./g, '')   // remove dots (thousands separator in PT-BR)
        .replace(',', '.')     // convert comma to dot (decimal separator)
        .trim();
    const parsed = parseFloat(cleaned);
    return isNaN(parsed) ? 0 : Math.max(props.min, Math.min(props.max, parsed));
};

const updateDisplayValue = (value: number, preserveCursor: boolean = false) => {
    const formatted = formatNumber(value);

    if (!isFocused.value) {
        displayValue.value = props.prefix + formatted;
        return;
    }

    // When focused, preserve cursor position
    if (preserveCursor && inputRef.value) {
        const cursorPos = inputRef.value.selectionStart || 0;
        const oldValue = displayValue.value;
        const newValue = props.prefix + formatted;

        displayValue.value = newValue;

        // Calculate new cursor position
        requestAnimationFrame(() => {
            if (inputRef.value) {
                const diff = newValue.length - oldValue.length;
                const maxPos = newValue.length - (props.suffix ? props.suffix.length + 1 : 0);
                const newPos = Math.max(props.prefix.length, Math.min(maxPos, cursorPos + diff));
                inputRef.value.setSelectionRange(newPos, newPos);
            }
        });
    } else {
        displayValue.value = props.prefix + formatted;
    }
};

const handleInput = (event: Event) => {
    const target = event.target as HTMLInputElement;
    const cursorPos = target.selectionStart || 0;
    let value = target.value;

    // Ensure prefix is always present
    if (!value.startsWith(props.prefix)) {
        value = props.prefix + value.replace(props.prefix, '');
        displayValue.value = value;
        requestAnimationFrame(() => {
            if (inputRef.value) {
                inputRef.value.setSelectionRange(props.prefix.length, props.prefix.length);
            }
        });
        return;
    }

    // Don't allow cursor before prefix
    if (cursorPos < props.prefix.length) {
        requestAnimationFrame(() => {
            if (inputRef.value) {
                inputRef.value.setSelectionRange(props.prefix.length, props.prefix.length);
            }
        });
        return;
    }

    displayValue.value = value;

    const numericValue = parseValue(value);
    emit('update:modelValue', numericValue);
};

const handleFocus = () => {
    isFocused.value = true;
    if (displayValue.value === '' || displayValue.value === props.prefix) {
        displayValue.value = props.prefix + formatNumber(props.modelValue);
    }
    // Move cursor to end before suffix
    requestAnimationFrame(() => {
        if (inputRef.value) {
            const pos = displayValue.value.length - (props.suffix ? props.suffix.length + 1 : 0);
            inputRef.value.setSelectionRange(pos, pos);
        }
    });
};

const handleBlur = () => {
    isFocused.value = false;
    const numericValue = parseValue(displayValue.value);
    emit('update:modelValue', numericValue);
    displayValue.value = props.prefix + formatNumber(numericValue);
};

const handleKeyDown = (event: KeyboardEvent) => {
    const target = event.target as HTMLInputElement;
    const cursorPos = target.selectionStart || 0;

    // Prevent deleting the prefix
    if ((event.key === 'Backspace' || event.key === 'Delete') && cursorPos <= props.prefix.length) {
        event.preventDefault();
    }

    // Only allow numbers, decimal point, and navigation keys
    const allowedKeys = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab', 'Home', 'End'];
    if (!allowedKeys.includes(event.key) &&
        !/^\d$/.test(event.key) &&
        event.key !== ',') {
        if (!event.ctrlKey && !event.metaKey) {
            event.preventDefault();
        }
    }
};

// Initialize display value
onMounted(() => {
    displayValue.value = props.prefix + formatNumber(props.modelValue);
});

// Watch for external changes
watch(() => props.modelValue, (newValue) => {
    if (!isFocused.value) {
        const clamped = Math.max(props.min, Math.min(props.max, newValue));
        displayValue.value = props.prefix + formatNumber(clamped);
    }
});
</script>

<template>
    <input
        ref="inputRef"
        v-model="displayValue"
        type="text"
        :disabled="disabled"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeyDown"
    />
</template>
