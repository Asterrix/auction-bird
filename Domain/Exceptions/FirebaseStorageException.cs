namespace Domain.Exceptions;

public sealed class FirebaseStorageException(string? message) : Exception(message);