using System.Text;
using UniversityManagement.Domain;
using UniversityManagement.Domain.Interfaces;

// Встановлюємо підтримку української мови в консолі
Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Modern Programming Course");
Console.WriteLine("Student: Чураков Олег Вячеславович");
Console.WriteLine("Group: 401 ТК");
Console.WriteLine("Variant: 3");
Console.WriteLine("Domain: University Management");
Console.WriteLine(".NET: 10\n");

// --- Демонстрація ООП (Лабораторна 2) ---
var course = new Course("Сучасні мови програмування");
var student = new Student();
var assignment = new Assignment("Лабораторна 1", 100);

course.AddAssignment(assignment);
course.EnrollStudent(student);
Console.WriteLine($"Студент {student.Name} успішно записаний на курс '{course.Name}'.");

var enrollment = course.Enrollments.First();
enrollment.AssignGrade(85);
enrollment.Complete();

IGradingStrategy percentageStrategy = new PercentageGradingStrategy();
IGradingStrategy passFailStrategy = new PassFailGradingStrategy();

Console.WriteLine($"\nОцінка студента ({percentageStrategy.GetType().Name}): {percentageStrategy.GetGrade(enrollment.Grade)}");
Console.WriteLine($"Оцінка студента ({passFailStrategy.GetType().Name}): {passFailStrategy.GetGrade(enrollment.Grade)}");