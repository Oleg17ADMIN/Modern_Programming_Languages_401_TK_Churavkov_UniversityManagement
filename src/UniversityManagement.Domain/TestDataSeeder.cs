namespace UniversityManagement.Domain;

// Helper для створення тестових даних: 15 студентів, 4 курси, 8 завдань, 31 запис на курс
public static class TestDataSeeder
{
    public static void Seed(InMemoryRepository<Student> students, InMemoryRepository<Course> courses)
    {
        string[] names =
        {
            "Шевченко Андрій", "Коваленко Марія", "Бондаренко Олексій", "Ткаченко Софія", "Кравченко Дмитро",
            "Мельник Анна", "Бойко Ігор", "Лисенко Оксана", "Мороз Назар", "Петренко Юлія",
            "Савченко Богдан", "Гриценко Катерина", "Руденко Максим", "Остапенко Вероніка", "Поліщук Тарас"
        };

        var studentList = names
            .Select((name, i) => new Student { Name = name, Email = $"student{i + 1}@univ.edu.ua" })
            .ToList();
        foreach (var student in studentList) students.Add(student);

        var math = new Course("Вища математика", new CourseCode("MATH", 101));
        var prog = new Course("Сучасні мови програмування", new CourseCode("CS", 401));
        var db = new Course("Бази даних", new CourseCode("DB", 301));
        var phys = new Course("Фізика", new CourseCode("PHYS", 201));

        math.AddAssignment(new Assignment("Модуль 1", 100));
        math.AddAssignment(new Assignment("Модуль 2", 100));
        prog.AddAssignment(new Assignment("Лабораторна 3", 100));
        prog.AddAssignment(new Assignment("Лабораторна 4", 100));
        prog.AddAssignment(new Assignment("Лабораторна 5", 100));
        db.AddAssignment(new Assignment("Курсовий проєкт", 100));
        db.AddAssignment(new Assignment("Лабораторна SQL", 50));
        phys.AddAssignment(new Assignment("Контрольна 1", 100));

        foreach (var course in new[] { math, prog, db, phys }) courses.Add(course);

        // Запис: курс, номер студента (1..15), оцінка. Без оцінки = студент ще навчається (Active).
        Enroll(math, 1, 92); Enroll(math, 2, 85); Enroll(math, 3, 58); Enroll(math, 4, 74);
        Enroll(math, 5, 90); Enroll(math, 6, 67); Enroll(math, 7); Enroll(math, 8, 81);
        Enroll(math, 9);

        Enroll(prog, 1, 88); Enroll(prog, 2, 95); Enroll(prog, 4, 61); Enroll(prog, 5, 79);
        Enroll(prog, 7, 54); Enroll(prog, 8, 86); Enroll(prog, 10, 91); Enroll(prog, 11, 73);
        Enroll(prog, 12); Enroll(prog, 13);

        Enroll(db, 3, 77); Enroll(db, 6, 84); Enroll(db, 9, 69); Enroll(db, 10, 96);
        Enroll(db, 12, 82); Enroll(db, 14); Enroll(db, 15, 55);

        Enroll(phys, 2, 83); Enroll(phys, 11, 78); Enroll(phys, 13, 88);
        Enroll(phys, 14, 71); Enroll(phys, 15, 90);

        void Enroll(Course course, int studentNumber, double? grade = null)
        {
            course.EnrollStudent(studentList[studentNumber - 1]);

            if (grade is double value)
            {
                var enrollment = course.Enrollments.Last();
                enrollment.AssignGrade(value);
                enrollment.Complete();
            }
        }
    }
}