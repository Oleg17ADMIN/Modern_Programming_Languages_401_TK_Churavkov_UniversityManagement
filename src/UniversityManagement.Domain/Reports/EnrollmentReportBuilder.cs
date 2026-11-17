namespace UniversityManagement.Domain.Reports;

public static class EnrollmentReportBuilder
{
    public static List<EnrollmentReport> Build(IEnumerable<Course> courses)
    {
        ArgumentNullException.ThrowIfNull(courses);

        return courses
            .SelectMany(course => course.Enrollments.Select(e => new EnrollmentReport(
                e.Student.Name,
                $"{course.Code.Department}-{course.Code.Number}",
                e.Status.ToString(),
                e.HasGrade ? e.Grade : null,
                e.Status == EnrollmentStatus.Completed)))
            .ToList();
    }
}
