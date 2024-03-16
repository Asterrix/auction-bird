namespace Domain.Exceptions;

public sealed class InvalidStateException(string? message) : Exception(message);