using MySuperShop.Models;

namespace MySuperShop.Pages
{
	public partial class CatalogPage
	{
		private List<Product>? products;

		protected override async Task OnInitializedAsync()
		{
			products = await Catalog.GetProductsAsync(CurrentTime);
		}

		void NavToProductCard(Guid guid)
		{
			Navigation.NavigateTo($"/products/{guid.ToString()}");
		}
	}
}
