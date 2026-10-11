namespace UniversityManagement.Domain;

public class Student
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }

    // Конструктор без параметрів, який встановлює ім'я за замовчуванням
    public Student()
    {
        Name = "Іван Іванов";
    }

    // Залишаємо старий конструктор для гнучкості
    public Student(string name)
    {
        Name = name;
    }
}