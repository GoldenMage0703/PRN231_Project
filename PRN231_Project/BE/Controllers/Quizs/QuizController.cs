using Lib.DTO.Quiz;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers.Quizs;

public class QuizController : ControllerBase
{
    private readonly IRepository<Question> _questionRepository;
    private readonly PRN231_ProjectContext _context;

    public QuizController(IRepository<Question> questionRepository, PRN231_ProjectContext context)
    {
        _context = context;
        _questionRepository = questionRepository;
    }
    
    [HttpGet("CreateQuiz")]
    public async Task<IActionResult> CreateQuiz(int time , int noQuestion, int courseID)
    {
        Random random = new Random();
        var listQuestion = await _questionRepository.FindIncludeAsync(x=>x.Options,x => x.Course == courseID);
        //get 5 random item from listQuestion
        var randomQuestions = listQuestion.Select(x=>new QuizQuestionDTO
        {
            question = x
        }).OrderBy(x => random.Next()).Take(noQuestion).ToList();
        QuizDTO quizDTO = new QuizDTO
        {
            minute = time,
            questions = randomQuestions
        };
        return Ok(quizDTO);
    }
    
}