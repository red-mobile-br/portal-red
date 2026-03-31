import { ChartArea, ScriptableContext } from "chart.js";
import ItemGraficoDTO from "../core/dtos/ItemGraficoDTO";
import { color as colorHelper } from 'chart.js/helpers';


export const mapearRotulosGrafico = (items:ItemGraficoDTO[]) => {
    return items.map(el => el.rotulo);
};

export const mapearSeriesRosca = (items: ItemGraficoDTO[]) => {
    return items.map(el => el.series[0].valor);
};

export function useGradienteRadial() {
    const chartColors = {
        red: 'rgb(255, 99, 132)',
        orange: 'rgb(255, 159, 64)',
        yellow: 'rgb(255, 205, 86)',
        green: 'rgb(75, 192, 192)',
        blue: 'rgb(54, 162, 235)',
        purple: 'rgb(153, 102, 255)',
        grey: 'rgb(201, 203, 207)'
    };
    const colors = [
        chartColors.green, 
        chartColors.red,
        chartColors.orange,
        chartColors.blue,
        chartColors.purple,
        chartColors.yellow,
        chartColors.grey,
        chartColors.green, 
        chartColors.red,
        chartColors.orange,
        chartColors.blue,
        chartColors.purple,
        chartColors.yellow,
        chartColors.grey,
    ];
    
    const cache = new Map();
    let width: number| null = null;
    let height: number | null = null;

    function createRadialGradient3(context: ScriptableContext<'doughnut'>, color: string) {
        const c1 = colorHelper(color).lighten(0.2).rotate(270).rgbString();
        const c2 = colorHelper(color).desaturate(0.2).lighten(0.2).rgbString();
        const c3 = colorHelper(color).rgbString();
        const chartArea = context.chart.chartArea;
        if (!chartArea) {
            // This case happens on initial chart load
            return;
        }
      
        const chartWidth = chartArea.right - chartArea.left;
        const chartHeight = chartArea.bottom - chartArea.top;
        if (width !== chartWidth || height !== chartHeight) {
            cache.clear();
        }
        let gradient = cache.get(c1 + c2 + c3);
        if (!gradient) {
            width = chartWidth;
            height = chartHeight;
            const centerX = (chartArea.left + chartArea.right) / 2;
            const centerY = (chartArea.top + chartArea.bottom) / 2;
            const r = Math.min(
                (chartArea.right - chartArea.left) / 2,
                (chartArea.bottom - chartArea.top) / 2
            );
            const ctx = context.chart.ctx;
            gradient = ctx.createRadialGradient(centerX, centerY, 0, centerX, centerY, r);
            gradient.addColorStop(0, c1);
            gradient.addColorStop(0.5, c2);
            gradient.addColorStop(1, c3);
            cache.set(c1 + c2 + c3, gradient);
        }
      
        return gradient;
    }

    return { colors, createRadialGradient3 };
}

export function useGradienteLinear() {
    let width: number, height: number, gradient: any;
    
    function getGradient(ctx:CanvasRenderingContext2D, chartArea: ChartArea, startColor: string, endColor: string) {

        const chartWidth = chartArea.right - chartArea.left;
        const chartHeight = chartArea.bottom - chartArea.top;

        if (!gradient || width !== chartWidth || height !== chartHeight) {
            width = chartWidth;
            height = chartHeight;
            gradient = ctx.createLinearGradient(width /2, chartArea.bottom, width / 2, chartArea.top);
            gradient.addColorStop(0, startColor);
            gradient.addColorStop(.4, endColor);
        }
      
        return gradient;
    }

    return { getGradient };
}