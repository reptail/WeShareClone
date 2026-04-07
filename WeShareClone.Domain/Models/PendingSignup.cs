namespace WeShareClone.Domain.Models;

public record PendingSignup(int Id, string Email, string Name, string? Phone, DateTime CreatedAtUtc);