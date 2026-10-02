namespace UniversityManagement.Domain;

public class Enrollment
{
    public Student Student { get; }
    public double Grade { get; private set; }
    public bool IsCompleted { get; private set; }

    public Enrollment(Student student)
    {
        Student = student;
    }

    public void AssignGrade(double grade)
    {
        if (IsCompleted) throw new InvalidOperationException("Неможливо змінити оцінку для завершеного курсу.");
        if (grade < 0 || grade > 100) throw new ArgumentException("Оцінка має бути від 0 до 100.");
        Grade = grade;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}