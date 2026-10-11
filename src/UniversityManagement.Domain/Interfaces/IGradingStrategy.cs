namespace UniversityManagement.Domain.Interfaces;

// Абстракція для оцінювання
public interface IGradingStrategy
{
	string GetGrade(double score);
}