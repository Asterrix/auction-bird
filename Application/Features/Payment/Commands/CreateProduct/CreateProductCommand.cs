using Domain.Items;
using MediatR;
using Stripe;

namespace Application.Features.Payment.Commands.CreateProduct;

public record CreateProductCommand(Item Item) : IRequest<Product>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly ProductService _productService = new();

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        ProductCreateOptions productOption = new()
        {
            Name = request.Item.Name,
            Active = true,
            Images = [request.Item.Images.First().ImageUrl],
            DefaultPriceData = new ProductDefaultPriceDataOptions
            {
                Currency = "usd",
                UnitAmountDecimal = request.Item.Bids.Max(i => i.Amount) * 100
            },
        };

        Product product = await _productService.CreateAsync(productOption, cancellationToken: cancellationToken);

        return product;
    }
}