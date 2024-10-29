using Microsoft.AspNetCore.Mvc;

namespace Lib.DTO.Quiz;
[Route("api/[controller]")]
[ApiController]
public class QuizQuestionDTO
{
    public Models.Question question { get; set; }   
}