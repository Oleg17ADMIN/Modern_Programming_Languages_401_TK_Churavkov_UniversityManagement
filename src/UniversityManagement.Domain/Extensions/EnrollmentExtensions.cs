namespace UniversityManagement.Domain.Extensions;

// Класичні extension methods: додаємо методи до Enrollment, не змінюючи сам клас.
// Перший параметр з this це об'єкт, до якого «прикріплено» метод.
public static class EnrollmentExtensions
{
    // enrollment.IsPassed()
    public static bool IsPassed(this Enrollment enrollment) =>
        enrollment.Status == EnrollmentStatus.Completed;

    // enrollment.GetSummary()
    public static string GetSummary(this Enrollment enrollment) => enrollment.Status switch
    {
        EnrollmentStatus.Active => $"{enrollment.Student.Name}: навчається, оцінка ще не підтверджена",
        EnrollmentStatus.Cancelled => $"{enrollment.Student.Name}: запис скасовано",
        _ => $"{enrollment.Student.Name}: {enrollment.Grade:F1} ({enrollment.GetGradeCategory()}), {enrollment.Status}"
    };

    // extension method для колекції: середня оцінка лише по завершених записах
    public static double AverageGrade(this IEnumerable<Enrollment> enrollments) =>
        enrollments
            .Where(e => e.IsCompleted)
            .Select(e => e.Grade)
            .DefaultIfEmpty(0)
            .Average();
}
