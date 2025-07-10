namespace Shared.AddUserResponse
{
    public enum AddingUserIssue
    {
        None,
        ValidationError,
        DuplicateEmail,
        EmailAddressDoesNotExist,
        DataBaseError,
        UnhandledError
    }
}
