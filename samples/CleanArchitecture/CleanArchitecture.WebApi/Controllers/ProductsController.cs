using Arcanic.Mediator.Request.Abstractions;
using CleanArchitecture.Application.Product.Commands.Add;
using CleanArchitecture.Application.Product.Commands.Update;
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
        public async Task<int> Add([FromBody] AddProductCommand command)
        {
            var response = await mediator.SendAsync(command);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<int> Update(int id, [FromBody] UpdateProductCommand command)
        {
            // Ensure the ID from route matches the command
            command.Id = id;

            var response = await mediator.SendAsync(command);
            return response;
        }
    }
}
