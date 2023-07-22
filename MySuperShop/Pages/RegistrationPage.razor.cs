using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MySuperShop.HttpModels.Requests;

namespace MySuperShop.Pages
{
    public partial class RegistrationPage
    {
        [Inject] private IDialogService DialogService { get; set; }
        private RegisterAccountForm model = new RegisterAccountForm();
        private bool success;

        public class RegisterAccountForm
        {
            [Required]
            [StringLength(20, MinimumLength = 3, ErrorMessage = "Имя должно быть от 3 до 20 символов")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "Пароль минимум 8 символов", MinimumLength = 8)]
            public string Password { get; set; }

            [Required]
            [Compare(nameof(Password))]
            public string Password2 { get; set; }

        }

        private async Task ProcessRegistration()
        {
            try
            {
                await Client.Register(new RegisterRequest
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password
                });
                success = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageBox(
                    "Warning", 
                    $"Что-то пошло не так во время регистрации:\n{ex.Message}");
            }
        }
    }
    
    
}
