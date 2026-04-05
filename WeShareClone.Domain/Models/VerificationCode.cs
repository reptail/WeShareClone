namespace WeShareClone.Domain.Models;

public record VerificationCode(
    int Id,
    string Email,
    string CodeHash,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc);