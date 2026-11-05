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

    // bonusPolicy: Func<double, double>, політика бонусу (за замовчуванням без бонусу)
    // log: Action<string>, необов'язковий логер
    public void AssignGrade(double rawScore, Func<double, double>? bonusPolicy = null, Action<string>? log = null)
    {
        if (IsCompleted || Status == EnrollmentStatus.Cancelled)
            throw new InvalidOperationException("Неможливо змінити оцінку для неактивного курсу.");

        if (rawScore < 0 || rawScore > 100)
            throw new ArgumentException("Оцінка має бути від 0 до 100.");

        var policy = bonusPolicy ?? GradeBonusPolicies.NoBonus;
        var finalGrade = Math.Clamp(policy(rawScore), 0, 100);   // підсумок не може перевищити 100

        Grade = finalGrade;
        log?.Invoke($"Оцінка для {Student.Name}: вхідний бал {rawScore:F1}, підсумкова {finalGrade:F1}");
    }

    public void Complete(Action<string>? log = null)
    {
        IsCompleted = true;
        Status = Grade >= 60 ? EnrollmentStatus.Completed : EnrollmentStatus.Failed;
        log?.Invoke($"Запис студента {Student.Name} завершено, статус: {Status}");
    }

    public void Cancel(Action<string>? log = null)
    {
        Status = EnrollmentStatus.Cancelled;
        log?.Invoke($"Запис студента {Student.Name} скасовано");
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
        >= 74 and < 90   => "Good",
        >= 60 and < 74   => "Satisfactory",
        >= 0  and < 60   => "Unsatisfactory",
        _ => throw new ArgumentOutOfRangeException(nameof(Grade), "Некоректна оцінка")
    };
}
