using System.Text;
using UniversityManagement.Domain;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Лабораторна робота №4: Generics та LINQ");
Console.WriteLine("Student: Чураков Олег Вячеславович, група 401 ТК, варіант 3\n");

// ---------- Generic repository + тестові дані ----------
var studentRepository = new InMemoryRepository<Student>();
var courseRepository = new InMemoryRepository<Course>();

TestDataSeeder.Seed(studentRepository, courseRepository);

// Демонстрація методів Add / Remove / GetAll
var temp = new Student { Name = "Тимчасовий Студент", Email = "temp@univ.edu.ua" };
studentRepository.Add(temp);
Console.WriteLine($"Студентів після Add:    {studentRepository.GetAll().Count}");
studentRepository.Remove(temp);
Console.WriteLine($"Студентів після Remove: {studentRepository.GetAll().Count}\n");

var students = studentRepository.GetAll();
var courses = courseRepository.GetAll();

// Усі записи на курси з усіх курсів (SelectMany "розгортає" вкладені списки)
var allEnrollments = courses.SelectMany(c => c.Enrollments).ToList();

// Записи, де вже є підсумкова оцінка (курс завершено)
var graded = allEnrollments.Where(e => e.IsCompleted).ToList();

Console.WriteLine($"Студентів: {students.Count}, курсів: {courses.Count}, записів на курси: {allEnrollments.Count}\n");

// Середня оцінка кожного студента (рахуємо лише завершені записи)
var studentAverages = graded
    .GroupBy(e => e.Student)
    .Select(g => new { Student = g.Key, Average = g.Average(e => e.Grade) })
    .ToList();

// ---------- 1. Active enrollments ----------
var activeEnrollments = allEnrollments
    .Where(e => e.Status == EnrollmentStatus.Active)
    .ToList();
Console.WriteLine($"1. Active enrollments: {activeEnrollments.Count}");
foreach (var e in activeEnrollments)
    Console.WriteLine($"   - {e.Student.Name}");

// ---------- 2. Студенти із середньою оцінкою > 80 ----------
var excellentStudents = studentAverages
    .Where(x => x.Average > 80)
    .OrderByDescending(x => x.Average)
    .ToList();
Console.WriteLine($"\n2. Студенти із середньою оцінкою > 80: {excellentStudents.Count}");
foreach (var x in excellentStudents)
    Console.WriteLine($"   - {x.Student.Name}: {x.Average:F1}");

// ---------- 3. Середня оцінка всіх студентів ----------
var overallAverage = graded.Average(e => e.Grade);
Console.WriteLine($"\n3. Середня оцінка всіх студентів: {overallAverage:F2}");

// ---------- 4. Top-5 студентів за average grade ----------
var top5 = studentAverages
    .OrderByDescending(x => x.Average)
    .Take(5)
    .ToList();
Console.WriteLine("\n4. Top-5 студентів за середньою оцінкою:");
foreach (var x in top5)
    Console.WriteLine($"   - {x.Student.Name}: {x.Average:F1}");

// ---------- 5. Кількість студентів на кожному course ----------
Console.WriteLine("\n5. Кількість студентів на курсах:");
var studentsPerCourse = courses.Select(c => new { c.Name, Count = c.Enrollments.Count });
foreach (var x in studentsPerCourse)
    Console.WriteLine($"   - {x.Name}: {x.Count}");

// ---------- 6. Усі assignments усіх courses (SelectMany) ----------
var allAssignments = courses.SelectMany(c => c.Assignments).ToList();
Console.WriteLine($"\n6. Усі assignments ({allAssignments.Count}):");
foreach (var a in allAssignments)
    Console.WriteLine($"   - {a.Title} (макс. {a.MaxScore})");

// ---------- 7. Courses з найбільшою кількістю students ----------
var maxStudents = courses.Max(c => c.Enrollments.Count);
var largestCourses = courses.Where(c => c.Enrollments.Count == maxStudents).ToList();
Console.WriteLine($"\n7. Курси з найбільшою кількістю студентів ({maxStudents}):");
foreach (var c in largestCourses)
    Console.WriteLine($"   - {c.Name}");

// ---------- 8. Students, які мають хоча б одну grade < 60 ----------
var strugglingStudents = graded
    .Where(e => e.Grade < 60)
    .Select(e => e.Student.Name)
    .Distinct()
    .ToList();
Console.WriteLine($"\n8. Студенти з хоча б однією оцінкою < 60: {strugglingStudents.Count}");
foreach (var name in strugglingStudents)
    Console.WriteLine($"   - {name}");

// ---------- 9. Чи всі completed enrollments мають final grade ----------
var allCompletedHaveGrade = allEnrollments
    .Where(e => e.Status == EnrollmentStatus.Completed)
    .All(e => e.Grade > 0);
Console.WriteLine($"\n9. Усі completed enrollments мають оцінку: {allCompletedHaveGrade}");

// ---------- 10. Dictionary: CourseCode -> AverageGrade ----------
var averageByCourse = courses.ToDictionary(
    c => c.Code,
    c => c.Enrollments
          .Where(e => e.IsCompleted)
          .Select(e => e.Grade)
          .DefaultIfEmpty(0)
          .Average());
Console.WriteLine("\n10. Середня оцінка по курсах (CourseCode -> AverageGrade):");
foreach (var (code, average) in averageByCourse)
    Console.WriteLine($"   - {code.Department}-{code.Number}: {average:F1}");