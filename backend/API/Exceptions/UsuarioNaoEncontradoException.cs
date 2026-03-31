namespace RedMobilePedidos.API.Exceptions;

public sealed class UsuarioNaoEncontradoException(string message) : Exception(message);