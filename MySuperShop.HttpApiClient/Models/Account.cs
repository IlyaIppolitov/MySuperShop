namespace MySuperShop.HttpApiClient.Models;

public class Account
{
    public Account(string? name, string? email, string? password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
    }

    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    
    public string? Password { get; set; }

}