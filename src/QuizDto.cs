using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAppWeb
{
    public class QuizDto
    {
        public int QuizId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public int CreatedBy { get; set; }        

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
