using System;
using System.Collections.Generic;
using System.Text;
using UniversityManagement.Domain.Interfaces;

namespace UniversityManagement.Domain
{
    public class PercentageGradingStrategy : IGradingStrategy
    {
        public string GetGrade(double score)
        {
            if (score < 0 || score > 100) throw new ArgumentException("Оцінка має бути від 0 до 100.");
            return $"{score}%";
        }
    }
}
