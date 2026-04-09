import { ClienteDetalhadoDTO } from "@/core/dtos/cliente/ClienteDetalhadoDTO";
import { cepValido, cnpjValido, cpfValido, emailValido, telefoneValido } from "@/utils/validadores";
import { computed } from "vue";

export type FormGroupStatus = "pending" | "active" | "invalid" | "done";

export function useFormStatus(form: ClienteDetalhadoDTO) {

    const _validate = (fields: string[], validations: boolean[]) => {
        const fillStatus = fields.map(field => !!field.length);
        const isPeding = fillStatus.every(e => !e);
        if(isPeding) return "pending";

        const isActive = fillStatus.some(e => !e);
        if(isActive) return "active";

        const isValid = validations.every(e => e);

        if(!isValid) return "invalid";

        return  "done";
    };

    /**
     * Validate customer data status
     */
    const customerDataStatus = computed<FormGroupStatus>(() => {
        return _validate(
            [
                form.razaoSocial ?? '',
                form.nomeFantasia ?? '',
                form.cnpj ?? '',
                form.capitalSocial ?? '',
                form.suframa ?? '',
                form.dataFundacao ?? '',
                form.cnae ?? '',
                form.inscricaoEstadual ?? '',
                form.telefoneCobranca ?? '',
            ],
            [
                !cnpjValido(form.cnpj ?? '').length,
                !telefoneValido(form.telefoneCobranca ?? '').length
            ]
        );
    });

    const contactsDataStatus = computed<FormGroupStatus>(() => {
        return _validate(
            [
                form.nomeContato ?? '',
                form.celular ?? '',
                form.email ?? '',
                form.telefone ?? '',
                form.fax ?? '',
                form.website ?? '',
            ],
            [
                !telefoneValido(form.celular ?? '').length,
                !emailValido(form.email ?? '').length,
                !telefoneValido(form.telefone ?? '').length,
            ]
        );
    });

    const billingAddressStatus = computed<FormGroupStatus>(() => {
        return _validate(
            [
                form.endereco?.cep ?? '',
                form.endereco?.logradouro ?? '',
                String(form.endereco?.numero ?? ''),
                form.endereco?.bairro ?? '',
                form.endereco?.cidade ?? '',
                form.endereco?.uf ?? '',
            ],
            [
            ]
        );
    });

    const partnersStatus = computed<FormGroupStatus>(() => {
        return _validate(
            (form.socios ?? []).flatMap(p => {
                return [p.cpf ?? '', p.nome ?? '', String(p.percentual ?? '')];
            }),
            (form.socios ?? []).flatMap(p => [!cpfValido(p.cpf ?? '').length])
        );
    });

    const shippingStatus = computed<FormGroupStatus>(() => {
        return _validate(
            [
                form.nomeContatoEntrega ?? '',
                form.telefoneEntrega ?? '',
                form.emailEntrega ?? '',
                form.enderecoEntrega?.cep ?? '',
                form.enderecoEntrega?.logradouro ?? '',
                String(form.enderecoEntrega?.numero ?? ''),
                form.enderecoEntrega?.bairro ?? '',
                form.enderecoEntrega?.cidade ?? '',
                form.enderecoEntrega?.uf ?? ''
            ],
            [
                !telefoneValido(form.telefoneEntrega ?? '').length,
                !cepValido(form.enderecoEntrega?.cep ?? '')
            ]
        );
    });

    const referencesStatus = computed<FormGroupStatus>(() => {
        return _validate(
            (form.referenciasComerciais ?? []).flatMap(reference => {
                return [
                    reference.nomeContato ?? '',
                    reference.nomeEmpresa ?? '',
                    reference.celular ?? '',
                    reference.telefone ?? ''
                ];
            }),
            (form.referenciasComerciais ?? []).flatMap(reference => [
                !telefoneValido(reference.celular ?? '').length,
                !telefoneValido(reference.telefone ?? '').length
            ])
        );
    });

    return {
        customerDataStatus,
        contactsDataStatus,
        billingAddressStatus,
        partnersStatus,
        shippingStatus,
        referencesStatus,
    };
}