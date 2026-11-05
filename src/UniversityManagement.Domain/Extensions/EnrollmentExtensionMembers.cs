namespace UniversityManagement.Domain.Extensions;

// Додаткове завдання: extension block (C# 14).
// Усередині блоку extension(...) оголошуються члени, які виглядають як члени Enrollment.
// Тут це extension property: enrollment.IsExcellent
public static class EnrollmentExtensionMembers
{
    extension(Enrollment enrollment)
    {
        public bool IsExcellent => enrollment.Grade >= 90;
    }
}
