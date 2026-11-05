namespace UniversityManagement.Domain.Services;

// Subscriber №1: повідомляє студента про оцінку
public class StudentNotificationService
{
    private readonly Action<string> _log;

    public StudentNotificationService(Action<string> log)
    {
        _log = log;
    }

    // Сигнатура відповідає EventHandler<GradeAssignedEventArgs>
    public void Handle(object? sender, GradeAssignedEventArgs e)
    {
        _log($"[Сповіщення] {e.Enrollment.Student.Email}: вам виставлено оцінку {e.FinalGrade:F1}");
    }
}
