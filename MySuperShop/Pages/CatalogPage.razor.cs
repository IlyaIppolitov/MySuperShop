using MySuperShop.Models;

namespace MySuperShop.Pages
{
	public partial class CatalogPage
	{
		private Product[]? _products;
		private CancellationTokenSource _cts = new();

		protected override async Task OnInitializedAsync()
		{
			_products = await Client.GetProducts(_cts.Token);
		}

		void NavToProductCard(Guid guid)
		{
			Navigation.NavigateTo($"/products/{guid.ToString()}");
		}

		void Dispose()
		{
			// Выполнится в момент закрытия/переключения страницы
			_cts.Cancel();
		}
	}
}
