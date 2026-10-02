namespace UniversityManagement.Domain;

public class Student
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }

    public Student(string name)
    {
        Name = name;
    }
}