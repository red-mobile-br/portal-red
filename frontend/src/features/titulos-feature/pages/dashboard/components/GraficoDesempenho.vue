<script lang="ts" setup>
import { PropType, onMounted, ref, nextTick } from 'vue';
import Chart from 'chart.js/auto';
import ItemGraficoDTO from '@/core/dtos/ItemGraficoDTO';

const props = defineProps({
    items: {
        type: Array as PropType<ItemGraficoDTO[]>,
        required: true
    },
});

const chart = ref<HTMLCanvasElement>();

function loadChart() {
    const ctx = chart.value as HTMLCanvasElement;
    const labels = props.items.map(i => i.rotulo ?? '');
    
    const colors = ['#F66A70', '#98D8D5', '#E2CA4A'];
    const isDark = () => document.documentElement.classList.contains('dark');
    const textColor = () => isDark() ? 'white' : '#171717';

    const nomeSeries = [0, 1, 2].map(i => props.items[0]?.series?.[i]?.nome ?? '');

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
                    display: true,
                    labels: {
                        color: textColor(),
                        font: { size: 11 },
                        boxWidth: 12,
                        boxHeight: 12,
                        padding: 12,
                    }
                },
            },
            scales: {
                percentageScale: {
                    type: 'linear',
                    position: 'right',
                    ticks: {
                        color: textColor(),
                        font: { size: 10 },
                        callback: (value) => `${value}%`,
                    },
                    grid: {
                        display: false,
                    }
                },
                y: {
                    beginAtZero: true,
                    border: {
                        display: false,
                    },
                    ticks: {
                        color: textColor(),
                        count: 5,
                        font: { size: 10 }
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
                        color: textColor(),
                        font: { size: 10 }
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
                },
                bar: {
                    borderRadius: 8,   
                    
                }
            }
        },
        data: {
            labels: labels,
            datasets:[
                {
                    type: 'bar',
                    label: nomeSeries[0],
                    data: props.items.flatMap(({ series }) => (series ?? [])[0]?.valor ?? 0),
                    backgroundColor: colors[0],
                    order: 3,
                    yAxisID: 'y',
                    barPercentage: .7,
                },
                {
                    type: 'bar',
                    label: nomeSeries[1],
                    data: props.items.flatMap(({ series }) => (series ?? [])[1]?.valor ?? 0),
                    backgroundColor: colors[1],
                    order: 2,
                    yAxisID: 'y',
                },
                {
                    type: 'line',
                    label: nomeSeries[2],
                    data: props.items.flatMap(({ series }) => (series ?? [])[2]?.valor ?? 0),
                    backgroundColor: colors[2],
                    borderColor: colors[2],
                    order: 1,
                    yAxisID: 'percentageScale'
                },
            ]
        }
    });
    
    window.addEventListener('darkModeToogle', () => {
        nextTick(() => {
            const cor = textColor();
            chartEl.options.scales!['x']!.ticks!.color = cor;
            chartEl.options.scales!['y']!.ticks!.color = cor;
            chartEl.options.scales!['percentageScale']!.ticks!.color = cor;
            chartEl.options.plugins!.legend!.labels!.color = cor;
            chartEl.update();
        });
    });
}

onMounted(() => loadChart());
</script>

<template>
    <canvas ref="chart" class="absolute top-0 right-0 bottom-0 left-0" />
</template>