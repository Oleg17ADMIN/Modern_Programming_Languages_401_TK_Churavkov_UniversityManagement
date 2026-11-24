using Microsoft.Extensions.Logging;

namespace UniversityManagement.Domain.Application;

public class EnrollmentApplicationService
{
    private readonly IStudentNotifier _notifier;
    private readonly ILogger<EnrollmentApplicationService> _logger;

    public EnrollmentApplicationService(IStudentNotifier notifier, ILogger<EnrollmentApplicationService> logger)
    {
        _notifier = notifier;
        _logger = logger;
    }

    public void AssignGrade(Enrollment enrollment, double score)
    {
        string studentName = enrollment.Student.Name;
        _logger.LogInformation("Assigning grade {Score} to student {StudentName}", score, studentName);

        try
        {
            enrollment.AssignGrade(score);
        }
        catch (ArgumentOutOfRangeException)
        {
            _logger.LogWarning("Grade {Score} for student {StudentName} is out of range, grade was not changed", score, studentName);
            throw;
        }

        _notifier.Notify(enrollment.Student, $"Вам виставлено оцінку {enrollment.Grade}");
        _logger.LogInformation("Grade {Grade} assigned to student {StudentName}, notification sent", enrollment.Grade, studentName);
    }
}
