namespace LearnIT.Application.Models
{
    public class TokenValidationResponse
    {
        public TokenValidationResponse(TokenValidationProblems validationProblem) 
        {
            TokenValidationProblem = validationProblem;
        }

        public TokenValidationResponse(int userId, TokenValidationProblems validationProblem)
        {
            UserId = userId;
            TokenValidationProblem = validationProblem;
        }
        public TokenValidationProblems TokenValidationProblem { get; set; }

        public int UserId { get; set; }
    }
}
