namespace UniversityManagement.Domain;

// Політики бонусів до оцінки. Кожна політика це lambda типу Func<double, double>:
// приймає вхідний бал і повертає підсумковий.
public static class GradeBonusPolicies
{
    public static readonly Func<double, double> NoBonus = score => score;

    public static readonly Func<double, double> PlusFivePoints = score => score + 5;

    public static readonly Func<double, double> PlusTenPercent = score => score * 1.10;
}
