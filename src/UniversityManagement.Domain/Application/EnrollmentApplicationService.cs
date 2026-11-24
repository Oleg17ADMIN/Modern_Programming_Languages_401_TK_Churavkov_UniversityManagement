namespace UniversityManagement.Domain.Application;

public class EnrollmentApplicationService
{
    private readonly IStudentNotifier _notifier;

    public EnrollmentApplicationService(IStudentNotifier notifier)
    {
        _notifier = notifier;
    }

    public void AssignGrade(Enrollment enrollment, double score)
    {
        enrollment.AssignGrade(score);
        _notifier.Notify(enrollment.Student, $"Вам виставлено оцінку {enrollment.Grade}");
    }
}
