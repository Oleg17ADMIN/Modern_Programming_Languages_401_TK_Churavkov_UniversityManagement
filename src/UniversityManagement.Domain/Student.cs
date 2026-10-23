namespace UniversityManagement.Domain;

public class Student
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? MiddleName { get; init; }
}