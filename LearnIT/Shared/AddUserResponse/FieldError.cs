namespace Shared.AddUserResponse
{
    public class FieldError
    {
        public FieldError() 
        {
            
        }

        public FieldError(Fields field, string errorMessage)
        {
            Field = field;
            ErrorMessage = errorMessage;
        }
        public Fields Field { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
