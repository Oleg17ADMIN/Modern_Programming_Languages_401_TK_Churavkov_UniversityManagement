using System.Text;
using System.Text.Json;
using UniversityManagement.Domain;
using UniversityManagement.Domain.Reports;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Лабораторна робота №7: JSON, файли та конфігурація");
Console.WriteLine("Student: Чураков Олег Вячеславович, група 401 ТК, варіант 3\n");

var students = new InMemoryRepository<Student>();
var courses = new InMemoryRepository<Course>();
TestDataSeeder.Seed(students, courses);

string filePath = Path.Combine(AppContext.BaseDirectory, "data", "report.json");
var storage = new EnrollmentReportStorage(filePath);

Console.WriteLine("1. Формування звіту");
var report = EnrollmentReportBuilder.Build(courses.GetAll());
Console.WriteLine($"Записів у звіті: {report.Count}");

Console.WriteLine("\n2. Збереження в JSON");
try
{
    await storage.SaveAsync(report);
    Console.WriteLine($"Файл збережено: {storage.FilePath}");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Помилка: немає доступу до каталогу для запису.");
}
catch (IOException ex)
{
    Console.WriteLine($"Помилка запису файлу: {ex.Message}");
}

Console.WriteLine("\n3. Читання з JSON");
var loaded = await TryLoadAsync(storage);
if (loaded is not null)
{
    foreach (var r in loaded)
    {
        string grade = r.FinalGrade?.ToString("F1") ?? "-";
        Console.WriteLine($"{r.StudentName,-20} {r.CourseCode,-9} {r.Status,-10} {grade,5}  {(r.Passed ? "так" : "ні")}");
    }

    Console.WriteLine($"\nПрочитано записів: {loaded.Count}");
    Console.WriteLine($"Успішно завершено навчань: {loaded.Count(r => r.Passed)}");
    Console.WriteLine($"Дані збігаються зі збереженими: {loaded.SequenceEqual(report)}");
}

string dataDir = Path.GetDirectoryName(filePath)!;

Console.WriteLine("\n4. Файлу не існує");
await TryLoadAsync(new EnrollmentReportStorage(Path.Combine(dataDir, "missing.json")));

Console.WriteLine("\n5. Пошкоджений JSON");
string brokenPath = Path.Combine(dataDir, "broken.json");
await File.WriteAllTextAsync(brokenPath, "[ { \"StudentName\": \"Іваненко\", ");
await TryLoadAsync(new EnrollmentReportStorage(brokenPath));

Console.WriteLine("\nПрограма завершилась без аварійного завершення.");

static async Task<List<EnrollmentReport>?> TryLoadAsync(EnrollmentReportStorage storage)
{
    try
    {
        return await storage.LoadAsync();
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine($"Помилка: файл не знайдено ({ex.FileName}).");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Помилка: некоректний JSON. {ex.Message.Split('.')[0]}.");
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("Помилка: немає доступу до файлу.");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"Помилка читання файлу: {ex.Message}");
    }

    return null;
}
