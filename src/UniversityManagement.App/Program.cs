using System.Text;
using UniversityManagement.Domain;
using UniversityManagement.Domain.Interfaces;

// Встановлюємо правильне кодування для української мови
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Modern Programming Course - Lab 3");
Console.WriteLine("Student: Чураков Олег Вячеславович");
Console.WriteLine("Group: 401 ТК");
Console.WriteLine("Variant: 3");
Console.WriteLine(".NET: 10\n");

// Демонстрація Lab 3: Створення Value Object (Record)
var courseCode = new CourseCode("CS", 401);
var course = new Course("Сучасні мови програмування", courseCode);

// Використання object initia lizer завдяки required та init
var student = new Student
{
    Name = "Олег Чураков",
    Email = "oleg@university.edu.ua",
    MiddleName = null // Nullable type
};

var assignment = new Assignment("Лабораторна 3", 100);

course.AddAssignment(assignment);
course.EnrollStudent(student);

var enrollment = course.Enrollments.First();
Console.WriteLine($"Статус до оцінювання: {enrollment.GetEnrollmentStatusDescription()}");

enrollment.AssignGrade(85);
enrollment.Complete();

Console.WriteLine($"\nСтудент: {student.Name} ({student.Email})");
Console.WriteLine($"Курс: {course.Name} [{course.Code.Department}-{course.Code.Number}]");
Console.WriteLine($"Статус після завершення: {enrollment.GetEnrollmentStatusDescription()}");
Console.WriteLine($"Категорія оцінки: {enrollment.GetGradeCategory()}");

// Демонстрація патерну Стратегія
IGradingStrategy percent = new PercentageGradingStrategy();
IGradingStrategy passFail = new PassFailGradingStrategy();

Console.WriteLine($"Оцінка (відсотки): {percent.GetGrade(enrollment.Grade)}");
Console.WriteLine($"Оцінка (залік):    {passFail.GetGrade(enrollment.Grade)}");