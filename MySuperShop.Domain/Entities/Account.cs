using System.ComponentModel.DataAnnotations;

namespace MySuperShop.Domain.Entities;

public class Account : IEntity
{
    private Guid _id;
    private string? _name;
    private string? _email;
    private string? _hashedPassword;

    public Account(Guid id, string name, string email, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(email));
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(hashedPassword));

        if (!new EmailAddressAttribute().IsValid(email))
        {
            throw new AggregateException("Value is not a valid email" + email);
        }
        _id = id;
        _name = name;
        _email = email;
        _hashedPassword = hashedPassword;
    }

    public Guid Id
    {
        get => _id;
        set => _id = value;
    }

    public string? Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is null or whitespace" + nameof(value));
            _name = value;
        }
    }

    public string? Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is null or whitespace" + nameof(value));
            if (!new EmailAddressAttribute().IsValid(value))
            {
                throw new AggregateException("Value is not a valid email" + value);
            }
            _email = value;
        }
    }

    public string? Password
    {
        get => _hashedPassword;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is null or whitespace" + nameof(value));
            _hashedPassword = value;
        }
    }
}