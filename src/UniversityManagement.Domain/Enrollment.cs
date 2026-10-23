namespace UniversityManagement.Domain;

public class Enrollment
{
    public Student Student { get; }
    public double Grade { get; private set; }
    public bool IsCompleted { get; private set; }
    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;

    public Enrollment(Student student)
    {
        Student = student;
    }

    public void AssignGrade(double grade)
    {
        if (IsCompleted || Status == EnrollmentStatus.Cancelled)
            throw new InvalidOperationException("Неможливо змінити оцінку для неактивного курсу.");

        if (grade < 0 || grade > 100)
            throw new ArgumentException("Оцінка має бути від 0 до 100.");

        Grade = grade;
    }

    public void Complete()
    {
        IsCompleted = true;
        Status = Grade >= 60 ? EnrollmentStatus.Completed : EnrollmentStatus.Failed;
    }

    public void Cancel()
    {
        Status = EnrollmentStatus.Cancelled;
    }

    // Pattern matching 1: switch expression для опису статусу
    public string GetEnrollmentStatusDescription() => Status switch
    {
        EnrollmentStatus.Active => "Активний запис",
        EnrollmentStatus.Completed => "Успішно завершено",
        EnrollmentStatus.Failed => "Курс не складено",
        EnrollmentStatus.Cancelled => "Запис скасовано",
        _ => "Невідомий статус"
    };

    // Pattern matching 2: Relational patterns для категорії оцінки
public string GetGradeCategory() => Grade switch
{
    >= 90 and <= 100 => "Excellent",
    >= 74 and < 90 => "Good",
    >= 60 and < 74 => "Satisfactory",
    >= 0 and < 60 => "Unsatisfactory",
    _ => throw new ArgumentOutOfRangeException(nameof(Grade), "Некоректна оцінка")
};
}