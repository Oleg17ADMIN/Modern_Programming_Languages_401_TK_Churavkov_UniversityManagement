namespace UniversityManagement.Domain.Application;

public class InMemoryStudentNotifier : IStudentNotifier
{
    private readonly List<string> _messages = new();

    public IReadOnlyList<string> Messages => _messages;

    public void Notify(Student student, string message)
    {
        _messages.Add($"{student.Name}: {message}");
    }
}
