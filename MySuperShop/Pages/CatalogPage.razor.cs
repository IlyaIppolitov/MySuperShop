using MySuperShop.Models;

namespace MySuperShop.Pages
{
	public partial class CatalogPage
	{
		private Product[] products;

		protected override async Task OnInitializedAsync()
		{
			products = await client.GetProducts();
		}

		void NavToProductCard(Guid guid)
		{
			Navigation.NavigateTo($"/products/{guid.ToString()}");
		}
	}
}
