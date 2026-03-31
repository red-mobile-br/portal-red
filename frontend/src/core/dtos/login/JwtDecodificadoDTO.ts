export default interface JwtDecodificadoDTO {
    aud: string;
    exp: number;
    iss: string;
    nbf: number;
    sub: string
    tipoUsuario: "Representante" | "Gerente" | "Administrador";
}
