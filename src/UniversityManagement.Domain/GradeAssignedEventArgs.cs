namespace UniversityManagement.Domain;

// Дані, які publisher передає subscribers разом з подією.
// Стандартний .NET pattern: клас-нащадок EventArgs.
public sealed class GradeAssignedEventArgs : EventArgs
{
    public Enrollment Enrollment { get; }
    public double RawScore { get; }
    public double FinalGrade { get; }

    public GradeAssignedEventArgs(Enrollment enrollment, double rawScore, double finalGrade)
    {
        Enrollment = enrollment;
        RawScore = rawScore;
        FinalGrade = finalGrade;
    }
}
