<script lang="ts" setup>
import { PropType, onMounted, ref, nextTick } from 'vue';
import Chart from 'chart.js/auto';
import ItemGraficoDTO from '@/core/dtos/ItemGraficoDTO';


const props = defineProps({
    items: {
        type: Array as PropType<ItemGraficoDTO[]>,
        required: true
    },
    types: {
        type: Array as PropType<('line' | 'bar')[]>,
        required: true,
    }
});

const chart = ref<HTMLCanvasElement>();

function loadChart() {
    const ctx = chart.value as HTMLCanvasElement;
    const labels = props.items.map(i => i.rotulo);
    
    const colors = ['#F66A70', '#98D8D5'];

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
                        stepSize: 1000,
                        font: {
                            size: 10
                        }
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
                        font: {
                            size: 10
                        }
                    },
                    grid: {
                        display: false
                    }
                }
            },
            elements: {
                point: {
                    pointStyle: 'circle',
                    radius: 5,
                    backgroundColor(ctx) {
                        return colors[ctx.datasetIndex];
                    }, 
                },
                line: {
                    tension: .4,
                    
                }
            }
        },
        data: {
            labels: labels,

            datasets: props.items.length
                ? Array.from({ length: props.items[0].series?.length ?? 0 }, (_, i) => {
                    return {
                        type: 'line',
                        label: props.items[0].series?.[i]?.nome ?? '',
                        data: props.items.flatMap(({ series }) => series?.[i]?.valor ?? 0),
                        borderColor: colors[i],
                        order: (props.items[0].series?.length ?? 0) - i
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