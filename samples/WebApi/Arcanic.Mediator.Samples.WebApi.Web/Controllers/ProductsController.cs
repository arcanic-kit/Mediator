using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.Add;
using Arcanic.Mediator.Sample.WebApi.Application.Product.Commands.UpdatePrice;
using Arcanic.Mediator.Sample.WebApi.Application.Product.Queries.Get;
using Arcanic.Mediator.Samples.WebApi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Arcanic.Mediator.Samples.WebApi.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ICommandMediator _commandMediator;
        private readonly IQueryMediator _queryMediator;

        public ProductsController(ICommandMediator commandMediator, IQueryMediator queryMediator)
        {
            _commandMediator = commandMediator;
            _queryMediator = queryMediator;
        }

        [HttpGet("{Id}")]
        public async Task<ProductDetails?> Get(int Id)
        {
            var response = await _queryMediator.SendAsync(new GetProductQuery
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
            var response = await _commandMediator.SendAsync(new AddProductCommand
            {
                Name = "Sample Product",
                Price = 9.99m
            });

            return null;
        }

        [HttpPut("{id}/price")]
        public async Task<int?> UpdatePrice(int id, [FromBody] UpdateProductPriceCommand command)
        {
            await _commandMediator.SendAsync(new UpdateProductPriceCommand
            {
                Id = id,
                Price = command.Price
            });

            return null;
        }

        
    }
}
