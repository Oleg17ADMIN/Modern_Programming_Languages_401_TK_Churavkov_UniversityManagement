using System.Text.Encodings.Web;
using System.Text.Json;

namespace UniversityManagement.Domain.Reports;

public class EnrollmentReportStorage
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public EnrollmentReportStorage(string filePath, bool writeIndented = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
        _options = new JsonSerializerOptions
        {
            WriteIndented = writeIndented,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public string FilePath => _filePath;

    public async Task SaveAsync(IReadOnlyCollection<EnrollmentReport> reports)
    {
        ArgumentNullException.ThrowIfNull(reports);

        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        string json = JsonSerializer.Serialize(reports, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<EnrollmentReport>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Файл звіту не знайдено.", _filePath);

        string json = await File.ReadAllTextAsync(_filePath);
        var reports = JsonSerializer.Deserialize<List<EnrollmentReport>>(json, _options);

        if (reports is null || reports.Any(r => r is null || string.IsNullOrWhiteSpace(r.StudentName)))
            throw new JsonException("У файлі відсутні обов'язкові дані.");

        return reports;
    }
}
