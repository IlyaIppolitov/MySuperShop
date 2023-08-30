using MySuperShop.Domain.Entities;
using MySuperShop.Domain.Repositories;

namespace MySuperShop.Domain.Services;

public class CartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
    }

    public virtual async Task AddProduct(Guid accountId, Product product, double quantity = 1d)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        var cart = await _cartRepository.GetById(accountId);
        
        var existedItem = cart.Items.FirstOrDefault(item => item.ProductId == product.Id);
        // В метода GetCartByAccountId должен быть использован Include(nameof(Cart.Items));
        if (existedItem is null)
        {
            cart.Items.Add(new Cart.CartItem(Guid.Empty, product.Id, quantity));
        }
        else
        {
            existedItem.Quantity += quantity;
        }
        
        
        await _cartRepository.Update(cart);
    }

    public virtual Task<Cart> GetAccountCart(Guid accountId)
    {
        return _cartRepository.GetById(accountId);
    }
}