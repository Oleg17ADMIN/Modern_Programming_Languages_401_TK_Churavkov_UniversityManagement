namespace UniversityManagement.Domain.Application;

public class ConsoleStudentNotifier : IStudentNotifier
{
    public void Notify(Student student, string message)
    {
        Console.WriteLine($"[Консоль] {student.Name}: {message}");
    }
}
