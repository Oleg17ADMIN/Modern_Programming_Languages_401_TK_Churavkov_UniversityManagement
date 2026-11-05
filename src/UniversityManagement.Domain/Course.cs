namespace UniversityManagement.Domain;

public class Course
{
    public string Name { get; }
    public CourseCode Code { get; }

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;

    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments;

    public Course(string name, CourseCode code)
    {
        Name = name;
        Code = code;
    }

    // log: Action<string>, необов'язковий логер, який передає викликач
    public void EnrollStudent(Student student, Action<string>? log = null)
    {
        if (_enrollments.Any(e => e.Student.Id == student.Id))
            throw new InvalidOperationException("Студент вже записаний на цей курс.");

        _enrollments.Add(new Enrollment(student));
        log?.Invoke($"Студента {student.Name} записано на курс «{Name}»");
    }

    public void RemoveStudent(Student student)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.Student.Id == student.Id);
        if (enrollment != null) _enrollments.Remove(enrollment);
    }

    public void AddAssignment(Assignment assignment)
    {
        _assignments.Add(assignment);
    }
}
