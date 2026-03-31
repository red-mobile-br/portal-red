<script lang="ts" setup>
import { PropType, onMounted, ref, nextTick } from 'vue';
import Chart from 'chart.js/auto';
import ItemGraficoDTO from '@/core/dtos/ItemGraficoDTO';
import { useGradienteLinear } from '@/utils/utilitarios-grafico';

const props = defineProps({
    items: {
        type: Array as PropType<ItemGraficoDTO[]>,
        required: true
    },
    lineColor: {
        type: String,
        default: 'rgba(152, 216, 213, 1)'
    },
    startColor: {
        type: String,
        default: 'rgba(255, 255, 255, .0)'
    },
    endColor: {
        type: String,
        default: 'rgba(152, 216, 213, .8)'
    }
});
const { getGradient } = useGradienteLinear();
const chart = ref<HTMLCanvasElement>();

function loadChart() {
    const ctx = chart.value as HTMLCanvasElement;
    const labels = props.items.map(i => i.rotulo);

    const chartEl = new Chart(ctx, {
        plugins: [],
        options: {
            interaction: {
                mode: 'index',
                intersect: false,
            },
            maintainAspectRatio: false,
            responsive: true,
            plugins: {
                legend: {
                    display: false
                },
            },
            scales: {
                y: {
                    border: {
                        display: false,
                    },
                    ticks: {
                        color: document.documentElement.classList.contains('dark') ? 'white' : '#171717',
                        stepSize: 100,
                        precision: 0,
                        maxTicksLimit: 8,
                    },
                    grid: {
                        lineWidth(ctx, _) {
                            if(ctx.index == 0) return 2;
                            return 1;
                        },
                    }
                },
                x: {
                    border: {
                        display: false,
                    },
                    ticks: {
                        color: document.documentElement.classList.contains('dark') ? 'white' : '#171717',
                    },
                    grid: {
                        display: false
                    }
                }
            },
            elements: {
                line: {
                    tension: .4,
                    backgroundColor(context) {
                        const chart = context.chart;
                        const { ctx, chartArea } = chart;
                        return getGradient(
                            ctx, chartArea ?? { bottom: 0, height: 0, left: 0, right: 0, top: 0, width: 0 },
                            props.startColor,
                            props.endColor,
                        );
                    },
                },
            }
        },
        data: {
            labels: labels,

            datasets: props.items.length 
                ? Array.from({ length: props.items[0].series.length }, (_, i) => {
                    return {
                        type: 'line',
                        label: props.items[0].series[i].nome,
                        data: props.items.flatMap(({ series }) => series[i].valor),
                        borderColor: props.lineColor,
                        order: props.items[0].series.length - i,
                        fill: 'origin',
                    };
                }) 
                : []
        }
    });

    window.addEventListener('darkModeToogle', () => {
        nextTick(() => {
            chartEl.options.scales!['x']!.ticks!.color = document.documentElement.classList.contains('dark') ? 'white' : '#171717';
            chartEl.options.scales!['y']!.ticks!.color = document.documentElement.classList.contains('dark') ? 'white' : '#171717';
            chartEl.update();
        });
    });
}

onMounted(() => loadChart());
</script>

<template>
    <canvas ref="chart" class="absolute top-0 right-0 bottom-0 left-0" />
</template>