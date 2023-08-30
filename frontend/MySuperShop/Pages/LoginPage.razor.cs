using Microsoft.AspNetCore.Components;
using MudBlazor;
using MySuperShop.HttpApiClient.Exceptions;
using MySuperShop.HttpModels.Requests;

namespace MySuperShop.Pages
{
    public partial class LoginPage
    {
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        private LoginRequest _model = new LoginRequest();
        private bool _loginInProgress = false;

        private async Task ProcessRegistration()
        {
            if (_loginInProgress)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("Пожалуйста, подождите...", Severity.Info);
                return;
            }
            _loginInProgress = true;
            try
            {
                await Client.Login(_model);
                await DialogService.ShowMessageBox(
                    "Успех!",
                    $"Вы успешно вошли! Молодец!",
                    yesText: "Ok!");
                NavigationManager.NavigateTo("/accounts/current");
            }
            catch (MySuperShopApiException ex)
            {
                _loginInProgress = false;
                await DialogService.ShowMessageBox(
                    "Ошибка!",
                    $"Ошибка входа: {ex.Message}");
            }
            finally
            {
                _loginInProgress = false;
            }
        }
    }
    
    
}
