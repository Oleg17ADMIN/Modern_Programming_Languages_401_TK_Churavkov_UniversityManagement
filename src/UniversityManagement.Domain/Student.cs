namespace UniversityManagement.Domain;

public class Student
{
    // Приватні поля: значення потрапляє сюди лише після перевірки в init-аксесорах
    private readonly string _name = string.Empty;
    private readonly string _email = string.Empty;

    public Guid Id { get; } = Guid.NewGuid();

    // required + init: значення задається лише під час створення об'єкта і одразу перевіряється
    public required string Name
    {
        get => _name;
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));   // порожнє ім'я заборонено
            _name = value.Trim();
        }
    }

    public required string Email
    {
        get => _email;
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Email));

            // мінімальна перевірка формату: щось@щось
            var at = value.IndexOf('@');
            if (at <= 0 || at == value.Length - 1)
                throw new ArgumentException("Email має бути у форматі name@domain.", nameof(Email));

            _email = value.Trim();
        }
    }

    public string? MiddleName { get; init; }
}
