import axios from 'axios';
import { format } from 'date-fns';
import qs from 'qs';
import { removerCamposVazios } from '../../utils/string-functions';

const clienteHttp = axios.create({
    timeout: 30000,
    paramsSerializer: {
        serialize: params => qs.stringify(removerCamposVazios(params), { serializeDate: (d) => format(d, "yyyy-MM-dd"), skipNulls: true }),

    },
    baseURL: import.meta.env.VITE_API_URL
});

export const definirTokenBearer = (token: string) => {
    clienteHttp.defaults.headers['Authorization'] = 'Bearer ' + token;
};

export default clienteHttp;