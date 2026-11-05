using System.Text;
using UniversityManagement.Domain;
using UniversityManagement.Domain.Extensions;
using UniversityManagement.Domain.Services;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Лабораторна робота №5: Delegates, Lambda, Events та Extension Methods");
Console.WriteLine("Student: Чураков Олег Вячеславович, група 401 ТК, варіант 3\n");

// ---------- Action<string>: логер (lambda) ----------
// Передаємо його в domain-операції та в сервіси: вони самі не знають, куди саме пишеться лог.
Action<string> logger = message => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");

// ---------- Тестові дані з Лаби 4 ----------
var studentRepository = new InMemoryRepository<Student>();
var courseRepository = new InMemoryRepository<Course>();
TestDataSeeder.Seed(studentRepository, courseRepository);

var courses = courseRepository.GetAll();
var allEnrollments = courses.SelectMany(c => c.Enrollments).ToList();

// ---------- 1. Func<double, double>: політики бонусів ----------
Console.WriteLine("1. Func<double, double>: політики бонусів (вхідний бал 85)");
var bonusPolicies = new Dictionary<string, Func<double, double>>
{
    ["NoBonus"] = GradeBonusPolicies.NoBonus,
    ["+5 points"] = GradeBonusPolicies.PlusFivePoints,
    ["+10%"] = GradeBonusPolicies.PlusTenPercent
};
foreach (var (name, policy) in bonusPolicies)
    Console.WriteLine($"   {name,-10} -> {policy(85):F1}");

// ---------- 2. Predicate<T>: середня оцінка < 60 ----------
Console.WriteLine("\n2. Predicate<StudentAverage>: студенти із середньою оцінкою < 60");
var studentAverages = allEnrollments
    .Where(e => e.IsCompleted)
    .GroupBy(e => e.Student)
    .Select(g => new StudentAverage(g.Key, g.Average(e => e.Grade)))
    .ToList();

// FindAll приймає саме Predicate<T>
var atRisk = studentAverages.FindAll(StudentPredicates.AverageBelow60);
foreach (var item in atRisk)
    Console.WriteLine($"   - {item.Student.Name}: {item.Average:F1}");
Console.WriteLine($"   Чи є хоча б один такий студент (Exists): {studentAverages.Exists(StudentPredicates.AverageBelow60)}");

// ---------- 3. Event: GradeAssigned, два subscribers ----------
Console.WriteLine("\n3. Event GradeAssigned: StudentNotificationService + AuditService");
var notificationService = new StudentNotificationService(logger);
var auditService = new AuditService(logger);

var activeEnrollments = allEnrollments
    .Where(e => e.Status == EnrollmentStatus.Active)
    .ToList();

// += підписує метод на подію. Підписуємо ОБИДВА сервіси на кожен активний запис.
foreach (var enrollment in activeEnrollments)
{
    enrollment.GradeAssigned += notificationService.Handle;
    enrollment.GradeAssigned += auditService.Handle;
}

// Кожному студенту виставляємо оцінку зі своєю політикою бонусу
var plan = new (double Score, Func<double, double> Policy)[]
{
    (55, GradeBonusPolicies.NoBonus),
    (72, GradeBonusPolicies.PlusFivePoints),
    (88, GradeBonusPolicies.PlusTenPercent),
    (91, GradeBonusPolicies.PlusTenPercent)   // 91 * 1.1 = 100.1, підсумок обмежується до 100
};

for (int i = 0; i < plan.Length; i++)
{
    Console.WriteLine();
    activeEnrollments[i].AssignGrade(plan[i].Score, plan[i].Policy, logger);   // тут виникає подія
    activeEnrollments[i].Complete(logger);
}

Console.WriteLine($"\n   Записів в аудиті: {auditService.Entries.Count}");

// ---------- 4. -= (відписка) ----------
Console.WriteLine("\n4. Відписка (-=): AuditService більше не слухає останній запис");
var lastEnrollment = activeEnrollments[^1];
lastEnrollment.GradeAssigned -= auditService.Handle;

lastEnrollment.AssignGrade(64, GradeBonusPolicies.PlusFivePoints, logger);   // реагує лише NotificationService
lastEnrollment.Complete(logger);
Console.WriteLine($"   Записів в аудиті (було 4, лишилось): {auditService.Entries.Count}");

// ---------- 5. Action<string> в інших domain-операціях ----------
Console.WriteLine("\n5. Action<string> logger в domain-операціях EnrollStudent / Cancel");
// Знаходимо курс за CourseCode: record порівнюється за значенням
var programmingCourse = courses.First(c => c.Code == new CourseCode("CS", 401));

var lateStudent = new Student { Name = "Новий Студент", Email = "new.student@univ.edu.ua" };
programmingCourse.EnrollStudent(lateStudent, logger);
programmingCourse.Enrollments.Last().Cancel(logger);

// ---------- 6. Extension methods ----------
Console.WriteLine("\n6. Extension methods: IsPassed(), GetSummary(), AverageGrade()");
foreach (var enrollment in activeEnrollments)
    Console.WriteLine($"   {enrollment.GetSummary()} | IsPassed: {enrollment.IsPassed()}");

Console.WriteLine($"   Скасований запис: {programmingCourse.Enrollments.Last().GetSummary()}");

var passed = allEnrollments.Count(e => e.IsPassed());
Console.WriteLine($"   Складено: {passed} з {allEnrollments.Count} записів");
Console.WriteLine($"   Середня оцінка по всіх завершених записах: {allEnrollments.AverageGrade():F2}");

// ---------- 7. C# 14 extension block ----------
Console.WriteLine("\n7. C# 14 extension block: властивість IsExcellent (>= 90)");
var excellent = allEnrollments.Where(e => e.IsExcellent).ToList();
Console.WriteLine($"   Відмінних оцінок: {excellent.Count}");
foreach (var enrollment in excellent)
    Console.WriteLine($"   - {enrollment.Student.Name}: {enrollment.Grade:F1}");
