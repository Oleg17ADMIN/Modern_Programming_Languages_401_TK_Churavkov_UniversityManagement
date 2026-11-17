namespace UniversityManagement.Domain.Reports;

public sealed record EnrollmentReport(
    string StudentName,
    string CourseCode,
    string Status,
    double? FinalGrade,
    bool Passed);
