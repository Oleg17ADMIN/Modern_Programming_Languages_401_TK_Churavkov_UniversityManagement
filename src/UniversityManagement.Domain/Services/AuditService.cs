namespace UniversityManagement.Domain.Services;

// Subscriber №2: веде журнал аудиту виставлених оцінок
public class AuditService
{
    private readonly Action<string> _log;
    private readonly List<string> _entries = new();

    public IReadOnlyList<string> Entries => _entries;

    public AuditService(Action<string> log)
    {
        _log = log;
    }

    public void Handle(object? sender, GradeAssignedEventArgs e)
    {
        var entry = $"{e.Enrollment.Student.Name}: {e.RawScore:F1} -> {e.FinalGrade:F1}";
        _entries.Add(entry);
        _log($"[Аудит] записано: {entry}");
    }
}
