namespace UniversityManagement.Domain.Application;

public interface IStudentNotifier
{
    void Notify(Student student, string message);
}
