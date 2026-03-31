<script lang="ts" setup>
defineProps({
    background: {
        type: String,
        default: 'var(--primary-color)'
    }
});
</script>

<template>
    <div class="loading-container flex justify-center">
        <div class="loading">
            <div class="loading__circle --circle-1"
                 :style="{background}" />
            <div class="loading__circle --circle-2"
                 :style="{background}" />
            <div class="loading__circle --circle-3"
                 :style="{background}" />
            <div class="loading__circle --circle-4"
                 :style="{background}" />
        </div>
    </div>
</template>

<style>
.loading-container {
    transform: translateX(-8px)
}
@keyframes translateCircles {
    from {
        transform: translateX(0)
    }
    to {
        transform: translateX(12px)
    }
}
.loading {
    display: flex;
    animation: translateCircles .7s linear infinite;
}

.loading__circle {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    margin: 0 2px;
}

@keyframes circle1 {
    from {
        transform: scale(.3);
        opacity: 0;
    }
    to {
        transform: scale(.5);
        opacity: 1;
    }
}

@keyframes circle2 {
    from {
        transform: scale(.5)
    }
    to {
        transform: scale(1)
    }
}

@keyframes circle3 {
    from {
        transform: scale(1)
    }
    to {
        transform: scale(.5)
    }
}

@keyframes circle4 {
    from { 
        transform: scale(.5);
        opacity: 1;
    }
    to {
        transform: scale(.3);
        opacity: 0;
    }
}
.--circle-1 {
    animation: circle1 .7s linear infinite;
}
.--circle-2 {
    animation: circle2 .7s linear infinite;
}
.--circle-3 {
    animation: circle3 .7s linear infinite;
}
.--circle-4 {
    animation: circle4 .7s linear infinite;
}
</style>