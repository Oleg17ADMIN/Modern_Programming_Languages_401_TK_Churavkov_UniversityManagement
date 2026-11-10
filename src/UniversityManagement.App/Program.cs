using System.Globalization;
using System.Text;
using UniversityManagement.Domain;
using UniversityManagement.Domain.Services;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Лабораторна робота №6: Обробка винятків та валідація даних");
Console.WriteLine("Student: Чураков Олег Вячеславович, група 401 ТК, варіант 3\n");

Action<string> logger = message => Console.WriteLine($"   [лог] {message}");

// ---------- Підготовка: курс, студент і підписники на подію GradeAssigned ----------
var course = new Course("Сучасні мови програмування", new CourseCode("CS", 401));
var student = new Student { Name = "Іваненко Петро", Email = "petro.ivanenko@univ.edu.ua" };

var auditService = new AuditService(logger);
int gradeAssignedCount = 0;          // скільки разів справді виникла подія GradeAssigned
Enrollment? enrollment = null;       // запис студента на курс (створюється в першій операції)

void Subscribe(Enrollment target)
{
    target.GradeAssigned += (_, _) => gradeAssignedCount++;
    target.GradeAssigned += auditService.Handle;
}

// Короткий «знімок» стану: за ним видно, чи змінилося щось після операції
string Snapshot() => enrollment is null
    ? $"записів на курсі {course.Enrollments.Count}, подій GradeAssigned {gradeAssignedCount}"
    : $"оцінка {enrollment.Grade:F1}, статус {enrollment.Status}, " +
      $"записів на курсі {course.Enrollments.Count}, подій GradeAssigned {gradeAssignedCount}";

// Перше речення повідомлення (у стандартних винятків далі йде "Parameter ..." та "Actual value ...")
string FirstLine(string text) => text.Split('\n')[0].TrimEnd('\r');

// Виконує операцію, перехоплює різні типи винятків і показує, чи змінився стан
void Run(string operation, Action action)
{
    Console.WriteLine($"Операція: {operation}");
    var before = Snapshot();
    try
    {
        action();
        Console.WriteLine("Результат: операцію виконано успішно");
    }
    // Порядок catch важливий: спочатку конкретніший тип, потім його базовий.
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"Помилка [{ex.GetType().Name}]: {FirstLine(ex.Message)}");
    }
    catch (ArgumentException ex)   // сюди потрапляє і ArgumentNullException (він наслідує ArgumentException)
    {
        Console.WriteLine($"Помилка [{ex.GetType().Name}]: {FirstLine(ex.Message)}");
    }
    catch (DomainRuleViolationException ex)
    {
        Console.WriteLine($"Помилка [{ex.GetType().Name}, правило {ex.Rule}]: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Помилка [{ex.GetType().Name}]: {ex.Message}");
    }
    finally   // виконується завжди: і після успіху, і після помилки
    {
        var after = Snapshot();
        Console.WriteLine($"Стан: {after}");
        Console.WriteLine($"Стан змінився: {before != after}\n");
    }
}

// ---------- 1. Коректні операції ----------
Console.WriteLine("1. Коректні операції (стан змінюється, подія виникає)\n");
Run("EnrollStudent(Іваненко Петро)", () =>
{
    course.EnrollStudent(student);
    enrollment = course.Enrollments.Single();
    Subscribe(enrollment);
});
Run("AssignGrade(85)", () => enrollment!.AssignGrade(85));

// ---------- 2. Некоректні вхідні дані (перевірка аргументів) ----------
Console.WriteLine("2. Некоректні вхідні дані: перевірка аргументів\n");
Run("new Student { Name = \"   \" }", () => _ = new Student { Name = "   ", Email = "test@univ.edu.ua" });
Run("new Student { Email = \"not-an-email\" }", () => _ = new Student { Name = "Тест", Email = "not-an-email" });
Run("EnrollStudent(null)", () => course.EnrollStudent(null!));
Run("AssignGrade(150)", () => enrollment!.AssignGrade(150));
Run("AssignGrade(NaN)", () => enrollment!.AssignGrade(double.NaN));   // NaN розпізнає й double.TryParse

// ---------- 3. Недопустимі операції (перевірка стану) ----------
Console.WriteLine("3. Недопустимі операції: перевірка стану об'єкта\n");
// Другий студент записаний на курс, але оцінки ще не має (підготовка, не демонстраційна операція)
var second = new Student { Name = "Коваль Марія", Email = "maria.koval@univ.edu.ua" };
course.EnrollStudent(second);
var secondEnrollment = course.Enrollments.Last();
Console.WriteLine($"Підготовка: {second.Name} записана на курс, оцінки ще немає");
Console.WriteLine();

Run("Complete() без виставленої оцінки (Коваль Марія)", () => secondEnrollment.Complete());
Run("Complete() (коректно, оцінка вже є)", () => enrollment!.Complete());
Run("AssignGrade(95) після завершення запису", () => enrollment!.AssignGrade(95));
Run("Complete() вдруге", () => enrollment!.Complete());
Run("Cancel() завершеного запису", () => enrollment!.Cancel());

// ---------- 4. Власний виняток DomainRuleViolationException ----------
Console.WriteLine("4. Власний виняток: повторне зарахування студента на той самий курс\n");
Run("EnrollStudent(Іваненко Петро) вдруге", () => course.EnrollStudent(student));

// ---------- 5. throw; та throw ex; ----------
Console.WriteLine("5. Повторне створення винятку: throw; зберігає стек, throw ex; скидає\n");
try
{
    try { enrollment!.AssignGrade(-5); }
    catch (ArgumentException ex)
    {
        logger($"{ex.GetType().Name} перехоплено, передаю далі через throw;");
        throw;
    }
}
catch (ArgumentException ex)
{
    Console.WriteLine($"   throw;    -> виняток виник у: {ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}");
}

try
{
    try { enrollment!.AssignGrade(-5); }
    catch (ArgumentException ex)
    {
        logger($"{ex.GetType().Name} перехоплено, передаю далі через throw ex;");
#pragma warning disable CA2200   // свідомо показуємо погану практику
        throw ex;   // так робити не варто: початковий стек викликів втрачається
#pragma warning restore CA2200
    }
}
catch (ArgumentException ex)
{
    Console.WriteLine($"   throw ex; -> виняток виник у: {ex.TargetSite?.DeclaringType?.Name}.{ex.TargetSite?.Name}\n");
}

// ---------- 6. Числове введення через TryParse ----------
Console.WriteLine("6. Введення оцінки з клавіатури (TryParse). Порожній рядок: вихід.\n");
var third = new Student { Name = "Сидоренко Олег", Email = "oleg.sydorenko@univ.edu.ua" };
course.EnrollStudent(third);
var thirdEnrollment = course.Enrollments.Last();
Subscribe(thirdEnrollment);

while (true)
{
    Console.Write($"Введіть оцінку для {third.Name} (0-100): ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) break;   // порожній рядок або кінець введення

    // Parse кинув би FormatException на звичайній помилці вводу, TryParse просто повертає false
    if (!double.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var score))
    {
        Console.WriteLine("Помилка: введіть число, наприклад 85 або 72,5.");
        continue;
    }

    try
    {
        thirdEnrollment.AssignGrade(score);
        Console.WriteLine($"Оцінку {thirdEnrollment.Grade:F1} збережено.");
        break;
    }
    catch (ArgumentOutOfRangeException ex)
    {
        // число розпізнано, але воно порушує правило предметної області
        Console.WriteLine($"Помилка: {FirstLine(ex.Message)}");
    }
}

// ---------- Підсумок ----------
Console.WriteLine("\nПідсумок");
Console.WriteLine($"   Подій GradeAssigned виникло: {gradeAssignedCount} (по одній на кожне успішне виставлення оцінки)");
Console.WriteLine($"   Записів в аудиті: {auditService.Entries.Count}");
Console.WriteLine($"   Оцінка першого студента не змінилася від помилкових спроб: {enrollment!.Grade:F1}");
