namespace UniversityManagement.Domain.External;

public sealed record ExternalUser(
    int Id,
    string Name,
    string Email);
