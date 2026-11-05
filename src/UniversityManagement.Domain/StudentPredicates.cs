namespace UniversityManagement.Domain;

public static class StudentPredicates
{
    // Predicate<T> завжди повертає bool: «студент у зоні ризику, якщо середня оцінка < 60»
    public static readonly Predicate<StudentAverage> AverageBelow60 =
        studentAverage => studentAverage.Average < 60;
}
