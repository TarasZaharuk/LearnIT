namespace LearnIT.Application.Models
{
    public enum EmailSendingIssues
    {
        None,
        InvalidRecipient,
        MailboxFull,
        MailboxUnavailable,
        SenderRejected,
        ConnectionFailed,
        RecipientServerBlocked,
        SmtpProtocolError,
        UnexpectedError
    }
}
