<script lang="ts" setup>
import { createApp, defineComponent, h, nextTick, onMounted, ref } from 'vue';
import type { PropType } from 'vue';
import mapboxgl from 'mapbox-gl';

import PinCliente from './PinCliente.vue';
import { coordenadasEstadosBrasileiros, estadosBrasileiros } from '@/utils/estados-brasileiros';

const props = defineProps({
    clients: {
        type: Array as PropType<{uf: string; quantidadeClientes: number}[]>,
        required: true
    }
});

let map: mapboxgl.Map;

const selectedState = ref('');

const loadClients = () => {
    const max = Math.max(...props.clients.map(el => el.quantidadeClientes));

    props.clients.forEach(client => {
        const coordinates = coordenadasEstadosBrasileiros[client.uf];

        if(coordinates) {
            const pinPlaceholder = document.createElement('button');
            pinPlaceholder.setAttribute('id', client.uf);
            pinPlaceholder.setAttribute('class', 'map-pin');
            pinPlaceholder.onclick = () => {
                const pins = document.querySelectorAll<HTMLElement>('.map-pin');
                pins.forEach(pin => pin.style.zIndex = "0");
                pinPlaceholder.style.zIndex = "9999";
            };

            const scale = (1 + (client.quantidadeClientes / max) * 0.5).toFixed(2);
            const state = client.uf;
            const clientsAmount = client.quantidadeClientes;
            const stateName = estadosBrasileiros.find(el => el.initials == state)?.name;
            

            new mapboxgl.Marker(pinPlaceholder)
                .setLngLat(coordinates)
                .addTo(map);

            const pin = defineComponent({
                render(){
                    return h(PinCliente, {
                        state,
                        selectedState: selectedState.value,
                        scale: +scale,
                        clientsAmount,
                        stateName,
                        onSelect: (value) => selectedState.value = value,
                    });

                }
            });

            nextTick(() => {
                createApp(pin).mount('#'+client.uf);
            });
        }
    });
};

const loadMap = () => {
    try {
        mapboxgl.accessToken = import.meta.env.VITE_MAPBOX!;
    
        map = new mapboxgl.Map({
            container: 'map', // container ID
            style: 'mapbox://styles/igor-dantas4505/ckdxunyjf1inm19se2u9q9fft',
            center: [-65, -15],
            zoom: 3,
            pitch: 45
        });
    
        loadClients();
        
    } catch (error) {
        console.log(error);
    }
};

onMounted(() => loadMap());
</script>

<template>
    <div id="map" class="h-full w-full transform" />
</template>