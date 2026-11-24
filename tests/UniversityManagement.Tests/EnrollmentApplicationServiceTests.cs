using Microsoft.Extensions.Logging.Abstractions;
using UniversityManagement.Domain;
using UniversityManagement.Domain.Application;

namespace UniversityManagement.Tests;

public class EnrollmentApplicationServiceTests
{
    [Fact]
    public void AssignGrade_ValidScore_SendsOneNotification()
    {
        var notifier = new InMemoryStudentNotifier();
        var service = new EnrollmentApplicationService(notifier, NullLogger<EnrollmentApplicationService>.Instance);
        var enrollment = new Enrollment(TestData.CreateStudent());

        service.AssignGrade(enrollment, 85);

        Assert.Equal(85, enrollment.Grade);
        Assert.Single(notifier.Messages);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void AssignGrade_InvalidScore_ThrowsAndSendsNoNotification(double score)
    {
        var notifier = new InMemoryStudentNotifier();
        var service = new EnrollmentApplicationService(notifier, NullLogger<EnrollmentApplicationService>.Instance);
        var enrollment = new Enrollment(TestData.CreateStudent());

        Assert.Throws<ArgumentOutOfRangeException>(() => service.AssignGrade(enrollment, score));

        Assert.Empty(notifier.Messages);
        Assert.False(enrollment.HasGrade);
    }
}
