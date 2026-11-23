using UniversityManagement.Domain;

namespace UniversityManagement.Tests;

public class CourseTests
{
    [Fact]
    public void EnrollStudent_NewStudent_AddsEnrollment()
    {
        var course = TestData.CreateCourse();
        var student = TestData.CreateStudent();

        course.EnrollStudent(student);

        Assert.Single(course.Enrollments);
        Assert.Equal(student, course.Enrollments.First().Student);
    }

    [Fact]
    public void EnrollStudent_SameStudentTwice_ThrowsAndKeepsCount()
    {
        var course = TestData.CreateCourse();
        var student = TestData.CreateStudent();
        course.EnrollStudent(student);

        Assert.Throws<DomainRuleViolationException>(() => course.EnrollStudent(student));

        Assert.Single(course.Enrollments);
    }
}
