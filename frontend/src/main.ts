import { createApp } from 'vue';
import App from './App.vue';
import 'v-calendar/style.css';
import router from './router';
import alert from './hooks/alerta';
import toast from './hooks/toast';
import { Money3Directive } from 'v-money3';

// import VueApexCharts from "vue3-apexcharts";
import datePicker from './hooks/date-picker';
import 'mapbox-gl/dist/mapbox-gl.css';

import './assets/styles/main.css';
import './assets/styles/transitions.css';

createApp(App)
    .use(router)
    .use(alert)
    .use(toast)
    .use(datePicker)
    .directive('money', Money3Directive)
    // .use(VueApexCharts)
    .mount('#app');
