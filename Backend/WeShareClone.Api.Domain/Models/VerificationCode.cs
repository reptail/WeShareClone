namespace WeShareClone.Api.Domain.Models;

public record VerificationCode(
    int Id,
    string Email,
    string CodeHash,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc);