namespace UniversityManagement.Domain;

// Власний виняток для порушення правила предметної області.
// Стандартного InvalidOperationException тут недостатньо:
//  - його кидають і самі бібліотеки (наприклад, LINQ: "Sequence contains no elements"),
//    тому catch (InvalidOperationException) не відрізнить бізнес-правило від програмної помилки;
//  - він не несе коду правила (властивість Rule), за яким програма може відреагувати.
public sealed class DomainRuleViolationException : Exception
{
    public string Rule { get; }

    public DomainRuleViolationException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    public DomainRuleViolationException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        Rule = rule;
    }
}
