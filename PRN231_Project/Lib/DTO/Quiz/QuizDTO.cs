namespace Lib.DTO.Quiz;

public class QuizDTO
{
    public QuizDTO()
    {
    }

    public QuizDTO(int minute, List<QuizQuestionDTO> questions)
    {
        this.minute = minute;
        this.questions = questions;
    }

    public int minute { get; set; }
    public List<QuizQuestionDTO> questions { get; set; }
}