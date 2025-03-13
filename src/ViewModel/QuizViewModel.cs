using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using QuizAppWeb.Models;

namespace QuizAppWeb.ViewModel
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Quiz>? Quizzes { get; set; }
    }
}
