using UniversityManagement.Domain;

// Залишаємо вивід з першої лабораторної
Console.WriteLine("Modern Programming Course");
Console.WriteLine("Student: Чураков Олег Вячеславович");
Console.WriteLine("Group: 401 ТК");
Console.WriteLine("Variant: 3");
Console.WriteLine("Domain: University Management");
Console.WriteLine(".NET: 10\n");

// --- Демонстрація ООП (Лабораторна 2) ---

// 1. Створення об'єктів
var course = new Course("Сучасні мови програмування");
var student = new Student("Іван Іваненко");
var assignment = new Assignment("Лабораторна 1", 100);

// 2. Взаємодія об'єктів (Інкапсуляція та поведінка)
course.AddAssignment(assignment);
course.EnrollStudent(student);
Console.WriteLine($"Студент {student.Name} успішно записаний на курс '{course.Name}'.");

// 3. Зміна стану через методи
var enrollment = course.Enrollments.First();
enrollment.AssignGrade(85);
enrollment.Complete();

// 4. Демонстрація поліморфізму (виклик через interface)
IGradingStrategy percentageStrategy = new PercentageGradingStrategy();
IGradingStrategy passFailStrategy = new PassFailGradingStrategy();

Console.WriteLine($"\nОцінка студента ({percentageStrategy.GetType().Name}): {percentageStrategy.GetGrade(enrollment.Grade)}");
Console.WriteLine($"Оцінка студента ({passFailStrategy.GetType().Name}): {passFailStrategy.GetGrade(enrollment.Grade)}");