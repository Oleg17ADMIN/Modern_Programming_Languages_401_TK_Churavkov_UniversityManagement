namespace UniversityManagement.Domain;

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
