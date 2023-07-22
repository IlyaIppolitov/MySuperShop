namespace MySuperShop.Domain.Exceptions;

public class EmailAlreadyExistsException : ArgumentException
{
    public string Value { get; }
    public EmailAlreadyExistsException(string message, string val) : base(message)
    {
        Value = val;
    }
}