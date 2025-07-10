namespace Shared.AddUserResponse
{
    public class AddUserResponse
    {
        public AddUserResponse()
        {

        }
        public AddUserResponse(AddingUserIssue issue) 
        {
            Issue = issue;
        }

        public AddUserResponse(AddingUserIssue issue, Fields field, string errorMessege)
        {
            Issue = issue;
            FieldErrors.Add(new FieldError(field,errorMessege));
        }

        public AddingUserIssue Issue { get; set; }

        public List<FieldError> FieldErrors { get; set; } = [];
    }
}
