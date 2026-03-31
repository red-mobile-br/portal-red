<script lang="ts" setup>
import { PropType, onMounted, ref, nextTick } from 'vue';
import Chart from 'chart.js/auto';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import ItemGraficoDTO from '@/core/dtos/ItemGraficoDTO';

const props = defineProps({
    items: {
        type: Array as PropType<ItemGraficoDTO[]>,
        required: true
    },
    stepSize: {
        type: Number,
        default: 1,
    }
});

const chart = ref<HTMLCanvasElement>();

function loadChart() {
    const ctx = chart.value as HTMLCanvasElement;
    const labels = props.items.map(i => i.rotulo);
    const values = props.items.flatMap(({ series }) => series[0].valor);
    
    const chartEl = new Chart(ctx, {
        type: 'bar',
        plugins: [ChartDataLabels],
        options: {
            scales: {
                y: {
                    border: {
                        display: false,
                    },
                    ticks: {
                        precision: 0,
                        maxTicksLimit: 8,
                        color: document.documentElement.classList.contains('dark') ? 'white' : '#171717',
                        stepSize: props.stepSize,
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
                        color: document.documentElement.classList.contains('dark') ? 'white' : '#171717'
                    },
                    grid: {
                        display: false
                    }
                }
            },
            maintainAspectRatio: false,
            responsive: true,
            plugins: {
                datalabels: {
                    display: false
                },
                legend: {
                    display: false,
                    labels: {
                        color: 'red'
                    }
                },
            },
            elements: {
                bar: {
                    borderRadius: 8,
                    
                }
            }
        },
        data: {
            labels: labels,
            datasets: [
                {
                    data: values,
                    backgroundColor: "rgba(246, 106, 112, 1)",
                    hoverOffset: 4,
                    barPercentage: .7,
                },
            ]
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