using Arcanic.Mediator.Command;
using Arcanic.Mediator.Query;
using Arcanic.Mediator.Request.Abstractions;
using CleanArchitecture.Application.Product.Commands.Add;
using CleanArchitecture.Application.Product.Commands.UpdatePrice;
using CleanArchitecture.Application.Product.Queries.Get;
using CleanArchitecture.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController(IMediator mediator)
        : ControllerBase
    {
        [HttpGet("{Id}")]
        public async Task<ProductDetails?> Get(int Id)
        {
            var response = await mediator.SendAsync(new GetProductQuery
            {
                Id = Id
            });

            if (response.Product is null)
            {
                return null;
            }

            return new ProductDetails()
            {
                Id = response.Product.Id,
                Name = response.Product.Name,
                Price = response.Product.Price,
            };
        }

        [HttpPost()]
        public async Task<int?> Add()
        {
            var response = await mediator.SendAsync(new AddProductCommand
            {
                Name = "Sample Product",
                Price = 9.99m
            });

            return null;
        }

        [HttpPut("{id}/price")]
        public async Task<int?> UpdatePrice(int id, [FromBody] UpdateProductPriceCommand command)
        {
            await mediator.SendAsync(new UpdateProductPriceCommand
            {
                Id = id,
                Price = command.Price
            });

            return null;
        }
    }
}
