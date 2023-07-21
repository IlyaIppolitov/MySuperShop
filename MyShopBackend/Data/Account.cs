﻿namespace MyShopBackend.Data;

public class Account : IEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    
    public string? Password { get; set; }

}