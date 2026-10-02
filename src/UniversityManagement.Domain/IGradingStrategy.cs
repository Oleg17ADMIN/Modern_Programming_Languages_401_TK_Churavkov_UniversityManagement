namespace UniversityManagement.Domain;

// Абстракція для оцінювання
public interface IGradingStrategy
{
	string GetGrade(double score);
}

// Реалізація 1: Оцінювання у відсотках
public class PercentageGradingStrategy : IGradingStrategy
{
	public string GetGrade(double score)
	{
		if (score < 0 || score > 100) throw new ArgumentException("Оцінка має бути від 0 до 100.");
		return $"{score}%";
	}
}

// Реалізація 2: Зараховано/Не зараховано
public class PassFailGradingStrategy : IGradingStrategy
{
	public string GetGrade(double score)
	{
		if (score < 0 || score > 100) throw new ArgumentException("Оцінка має бути від 0 до 100.");
		return score >= 50 ? "Pass" : "Fail";
	}
}