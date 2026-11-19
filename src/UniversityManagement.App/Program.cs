using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using UniversityManagement.Domain;
using UniversityManagement.Domain.External;
using UniversityManagement.Domain.Reports;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Лабораторні роботи №7 та №8: JSON, конфігурація, async та HTTP");
Console.WriteLine("Student: Чураков Олег Вячеславович, група 401 ТК, варіант 3\n");

var students = new InMemoryRepository<Student>();
var courses = new InMemoryRepository<Course>();
TestDataSeeder.Seed(students, courses);

var config = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false)
    .Build();

string relativePath = config["Storage:FilePath"]
    ?? throw new InvalidOperationException("Storage:FilePath is not configured.");
string filePath = Path.Combine(AppContext.BaseDirectory, relativePath);

bool writeIndented = true;
if (bool.TryParse(config["Storage:WriteIndented"], out var parsed))
    writeIndented = parsed;

var storage = new EnrollmentReportStorage(filePath, writeIndented);

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

string baseUrl = config["ExternalApi:BaseUrl"]
    ?? throw new InvalidOperationException("ExternalApi:BaseUrl is not configured.");

using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
var users = new ExternalUserClient(http);
int[] ids = { 1, 2, 3 };

await RunHttpAsync("\n6. Зовнішні контакти студентів (Task.WhenAll)", async () =>
{
    using var cts = NewTimeout();
    Console.WriteLine("Loading external contacts...");

    var watch = Stopwatch.StartNew();
    var contacts = await users.GetUsersAsync(ids, cts.Token);
    watch.Stop();

    Console.WriteLine($"Received: {contacts.Count} ({watch.ElapsedMilliseconds} мс)");
    foreach (var u in contacts)
        Console.WriteLine($"{u.Id} | {u.Name} | {u.Email}");
});

await RunHttpAsync("\n7. Послідовно і одночасно (додаткове завдання)", async () =>
{
    using var cts = NewTimeout();
    await users.GetUserAsync(1, cts.Token);

    var watch = Stopwatch.StartNew();
    await users.GetUsersSequentiallyAsync(ids, cts.Token);
    long sequential = watch.ElapsedMilliseconds;

    watch.Restart();
    await users.GetUsersAsync(ids, cts.Token);
    long concurrent = watch.ElapsedMilliseconds;

    Console.WriteLine($"Три послідовні запити: {sequential} мс");
    Console.WriteLine($"Три запити через Task.WhenAll: {concurrent} мс");
});

await RunHttpAsync("\n8. Сервер відповів помилкою (користувача 9999 не існує)", async () =>
{
    using var cts = NewTimeout();
    await users.GetUserAsync(9999, cts.Token);
});

await RunHttpAsync("\n9. Сервер недоступний", async () =>
{
    using var badHttp = new HttpClient { BaseAddress = new Uri("https://nonexistent.invalid/") };
    using var cts = NewTimeout();
    await new ExternalUserClient(badHttp).GetUserAsync(1, cts.Token);
});

await RunHttpAsync("\n10. Запит скасовано", async () =>
{
    using var cts = new CancellationTokenSource();
    cts.Cancel();
    await users.GetUserAsync(1, cts.Token);
});

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

static CancellationTokenSource NewTimeout() => new(TimeSpan.FromSeconds(10));

static async Task RunHttpAsync(string title, Func<Task> operation)
{
    Console.WriteLine(title);
    try
    {
        await operation();
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Запит скасовано або час очікування вичерпано.");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Помилка HTTP: {ex.Message}");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Помилка JSON: {ex.Message.Split('.')[0]}.");
    }
}
