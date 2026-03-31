import { Plugin, createApp } from 'vue';
import RmDatePicker, { SelectDateArgs } from '../components/RmDatePicker.vue';

type FuncaoSelecionarData = (args: SelectDateArgs) => Promise<Date | null>;

let useDatePicker: () => { selecionarData: FuncaoSelecionarData };

const datePicker: Plugin = {
    install: () => {
        const datePickerRoot = createApp(RmDatePicker).mount(document.body.appendChild(document.createElement('div')));
        useDatePicker = () => {
            return {
                selecionarData: (datePickerRoot as any).selectDate
            };
        };
    }
};

export { useDatePicker };

export default datePicker;
