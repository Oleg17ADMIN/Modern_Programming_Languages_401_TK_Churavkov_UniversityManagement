using UniversityManagement.Domain;

namespace UniversityManagement.Tests;

public class EnrollmentTests
{
    [Fact]
    public void AssignGrade_ValidScore_SetsGrade()
    {
        var enrollment = new Enrollment(TestData.CreateStudent());

        enrollment.AssignGrade(85);

        Assert.True(enrollment.HasGrade);
        Assert.Equal(85, enrollment.Grade);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void AssignGrade_InvalidScore_ThrowsAndKeepsState(double score)
    {
        var enrollment = new Enrollment(TestData.CreateStudent());

        Assert.Throws<ArgumentOutOfRangeException>(() => enrollment.AssignGrade(score));

        Assert.False(enrollment.HasGrade);
        Assert.Equal(0, enrollment.Grade);
    }
}
