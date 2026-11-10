namespace UniversityManagement.Domain;

public class Course
{
    public string Name { get; }
    public CourseCode Code { get; }

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;

    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments;

    // Код правила предметної області, який потрапляє у DomainRuleViolationException.Rule
    public const string DuplicateEnrollmentRule = "DuplicateEnrollment";

    public Course(string name, CourseCode code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);   // назва курсу обов'язкова
        ArgumentNullException.ThrowIfNull(code);
        Name = name.Trim();
        Code = code;
    }

    // log: Action<string>, необов'язковий логер, який передає викликач
    public void EnrollStudent(Student student, Action<string>? log = null)
    {
        ArgumentNullException.ThrowIfNull(student);   // 1) перевірка аргументу

        if (_enrollments.Any(e => e.Student.Id == student.Id))   // 2) перевірка стану курсу
            throw new DomainRuleViolationException(
                DuplicateEnrollmentRule,
                $"Студента {student.Name} уже записано на курс «{Name}»: повторне зарахування заборонене.");

        _enrollments.Add(new Enrollment(student));
        log?.Invoke($"Студента {student.Name} записано на курс «{Name}»");
    }

    public void RemoveStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        var enrollment = _enrollments.FirstOrDefault(e => e.Student.Id == student.Id);
        if (enrollment != null) _enrollments.Remove(enrollment);
    }

    public void AddAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        _assignments.Add(assignment);
    }
}
