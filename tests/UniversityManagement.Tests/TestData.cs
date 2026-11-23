using UniversityManagement.Domain;

namespace UniversityManagement.Tests;

public static class TestData
{
    public static Student CreateStudent(string name = "Іван Петренко")
    {
        return new Student { Name = name, Email = "student@example.com" };
    }

    public static Course CreateCourse()
    {
        return new Course("Алгоритми", new CourseCode("CS", 101));
    }
}
