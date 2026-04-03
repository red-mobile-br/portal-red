<script lang="ts" setup>
import { PropType, onMounted, ref, nextTick } from 'vue';
import Chart from 'chart.js/auto';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { useGradienteRadial } from '@/utils/utilitarios-grafico';
import ItemGraficoDTO from '@/core/dtos/ItemGraficoDTO';

const props = defineProps({
    items: {
        type: Array as PropType<ItemGraficoDTO[]>,
        required: true
    },
    displayLegend: {
        type: Boolean,
        default: false,
    }
});

const chart = ref<HTMLCanvasElement>();

const { colors, createRadialGradient3, } = useGradienteRadial();

function loadChart() {
    
    const ctx = chart.value as HTMLCanvasElement;
    const labels = props.items.map(i => i.rotulo);
    const values = props.items.flatMap(({ series }) => series?.[0]?.valor ?? 0);

    const chartEl = new Chart(ctx, {
        type: 'doughnut',
        plugins: [ChartDataLabels],
        options: {
            maintainAspectRatio: false,
            responsive: true,
            
            elements: {
                arc: { 
                    backgroundColor: (context) =>  {
                        if(!labels.length) {
                            return 'rgb(201, 203, 207)';
                        }
                        return createRadialGradient3(context, colors[context.dataIndex]);
                    }
                }
            },
            plugins: {
                legend: {
                    display: values.length < 5,
                    position: 'bottom',
                    labels: {
                        color: document.documentElement.classList.contains('dark') ? 'white' : '#171717',
                        usePointStyle: true,
                        pointStyle: 'rectRounded',
                    }
                },
                datalabels: {
                    color: 'white',
                    font: {
                        weight: 'bold',
                        size: 12
                    },
                    textShadowBlur: 5,
                    textShadowColor: 'black',
                    formatter: (value: any, context: any) => {
                        if(!values.length) return 'Nenhum resultado';
                        const label = labels[context.dataIndex];
                        const sum = (context.dataset.data as number[]).reduce((acc, el) => acc += el, 0);
                        const percentage = (value / sum) * 100;
                        return `${label}: ${Math.round(percentage)}%`;
                    }
                },
            }
        },
        data: {
            labels: labels.length ? labels : ["Nenhum resultado"],
            datasets: [
                {
                    data: values.length ? values : [1],
                    hoverOffset: 4,
                },
            ]
        }
    });

    window.addEventListener('darkModeToogle', () => {
        nextTick(() => {
            chartEl.options.plugins!.legend!.labels!.color = document.documentElement.classList.contains('dark') ? 'white' : '#171717';
            chartEl.update();
        });
    });
}

onMounted(() => loadChart());
</script>

<template>
    <canvas ref="chart" class="absolute top-0 right-0 bottom-0 left-0" />
</template>