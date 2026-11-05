namespace UniversityManagement.Domain;

// Студент і його середня оцінка (record: значення порівнюються за вмістом)
public record StudentAverage(Student Student, double Average);
