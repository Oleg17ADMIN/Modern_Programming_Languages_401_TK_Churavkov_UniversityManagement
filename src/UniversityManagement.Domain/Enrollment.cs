namespace UniversityManagement.Domain;

public class Enrollment
{
    public Student Student { get; }
    public double Grade { get; private set; }
    public bool HasGrade { get; private set; }   // чи виставлено оцінку (потрібно, щоб не завершити запис без оцінки)
    public bool IsCompleted { get; private set; }
    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;

    // Publisher: подія виникає після того, як оцінку виставлено
    public event EventHandler<GradeAssignedEventArgs>? GradeAssigned;

    public Enrollment(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        Student = student;
    }

    // bonusPolicy: Func<double, double>, політика бонусу (за замовчуванням без бонусу)
    // log: Action<string>, необов'язковий логер
    public void AssignGrade(double rawScore, Func<double, double>? bonusPolicy = null, Action<string>? log = null)
    {
        // 1) Перевірка аргументу. double.IsNaN потрібен, бо NaN не менший і не більший за будь-яке число,
        //    тому звичайні порівняння його пропустили б (а double.TryParse("NaN") це число розпізнає).
        if (double.IsNaN(rawScore) || rawScore < 0 || rawScore > 100)
            throw new ArgumentOutOfRangeException(nameof(rawScore), rawScore, "Оцінка має бути від 0 до 100.");

        // 2) Перевірка стану: оцінку можна змінювати лише в активному записі
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException(
                $"Оцінку не можна змінити: запис має статус {Status}.");

        var policy = bonusPolicy ?? GradeBonusPolicies.NoBonus;
        var finalGrade = Math.Clamp(policy(rawScore), 0, 100);   // підсумок не може перевищити 100

        // Усі перевірки пройдено: тепер змінюємо стан
        Grade = finalGrade;
        HasGrade = true;
        log?.Invoke($"Оцінка для {Student.Name}: вхідний бал {rawScore:F1}, підсумкова {finalGrade:F1}");

        OnGradeAssigned(new GradeAssignedEventArgs(this, rawScore, finalGrade));   // подія лише після успіху
    }

    // ?. означає: якщо підписників немає, нічого не відбувається
    private void OnGradeAssigned(GradeAssignedEventArgs e) => GradeAssigned?.Invoke(this, e);

    public void Complete(Action<string>? log = null)
    {
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException(
                $"Запис уже має статус {Status}: завершити його повторно неможливо.");

        if (!HasGrade)
            throw new InvalidOperationException("Не можна завершити запис, поки не виставлено оцінку.");

        IsCompleted = true;
        Status = Grade >= 60 ? EnrollmentStatus.Completed : EnrollmentStatus.Failed;
        log?.Invoke($"Запис студента {Student.Name} завершено, статус: {Status}");
    }

    public void Cancel(Action<string>? log = null)
    {
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException(
                $"Запис зі статусом {Status} скасувати неможливо.");

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
