using UniversityManagement.Domain;
using UniversityManagement.Domain.Services;

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

    [Fact]
    public void AssignGrade_ValidScore_RaisesEventOnce()
    {
        var enrollment = new Enrollment(TestData.CreateStudent());
        int eventCount = 0;
        enrollment.GradeAssigned += (sender, e) => eventCount++;

        enrollment.AssignGrade(85);

        Assert.Equal(1, eventCount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void AssignGrade_InvalidScore_DoesNotRaiseEvent(double score)
    {
        var enrollment = new Enrollment(TestData.CreateStudent());
        int eventCount = 0;
        enrollment.GradeAssigned += (sender, e) => eventCount++;

        Assert.Throws<ArgumentOutOfRangeException>(() => enrollment.AssignGrade(score));

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void AssignGrade_TwoSubscribers_BothAreCalled()
    {
        var enrollment = new Enrollment(TestData.CreateStudent());
        int firstCount = 0;
        int secondCount = 0;
        enrollment.GradeAssigned += (sender, e) => firstCount++;
        enrollment.GradeAssigned += (sender, e) => secondCount++;

        enrollment.AssignGrade(85);

        Assert.Equal(1, firstCount);
        Assert.Equal(1, secondCount);
    }

    [Fact]
    public void AssignGrade_WithAuditService_WritesOneEntry()
    {
        var enrollment = new Enrollment(TestData.CreateStudent());
        var audit = new AuditService(message => { });
        enrollment.GradeAssigned += audit.Handle;

        enrollment.AssignGrade(85);

        Assert.Single(audit.Entries);
    }
}
