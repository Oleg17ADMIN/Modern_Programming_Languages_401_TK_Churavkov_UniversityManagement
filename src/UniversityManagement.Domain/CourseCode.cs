namespace UniversityManagement.Domain;

// Record автоматично забезпечує порівняння за значенням (value-based equality)
public record CourseCode(string Department, int Number);