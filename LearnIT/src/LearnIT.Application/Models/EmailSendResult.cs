namespace LearnIT.Application.Models
{
    public class EmailSendResult
    {
        public EmailSendResult(EmailSendingIssues sendingIssue, string errorMessege) 
        {
            SendingIssue = sendingIssue;
            ErrorMessege = errorMessege;
        }

        public EmailSendResult(EmailSendingIssues sendingIssue)
        {
            SendingIssue = sendingIssue;
        }

        public EmailSendingIssues SendingIssue;
        public string? ErrorMessege { get; set; }
    }
}
