import CepDTO from '../core/dtos/endereco/CepDTO';

const CepService = {
    buscarCep: (cep: string): Promise<CepDTO> => {
        return new Promise((resolve, reject) => {
            fetch(`https://viacep.com.br/ws/${cep.replace('-', '')}/json/`)
                .then(resp => resp.status == 200 ? resp.json() : Promise.reject(resp.body))
                .then(resp => {
                    if(resp.erro) {
                        return Promise.reject("CEP não encontrado");
                    }
                    else {
                        resolve(resp as CepDTO);
                    }
                })
                .catch(error => {
                    reject(error);
                });
        });
    }
};

export default CepService;