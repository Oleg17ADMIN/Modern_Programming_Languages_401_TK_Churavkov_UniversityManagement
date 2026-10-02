namespace UniversityManagement.Domain;

public class Assignment
{
    public string Title { get; set; }
    public double MaxScore { get; set; }

    public Assignment(string title, double maxScore)
    {
        Title = title;
        MaxScore = maxScore;
    }
}