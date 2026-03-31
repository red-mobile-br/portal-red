import TabelaPrecosDTO from "../core/dtos/tabelasPrecos/TabelaPrecosDTO";
import { obter } from "./base-service";

const tabelaPrecosService = {
    obterTabelaPrecos: (estado: string) => {
        return obter<TabelaPrecosDTO>(`/tabelaPrecos/${estado}`);
    }
};

export default tabelaPrecosService;
