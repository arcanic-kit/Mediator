using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Query.Abstractions;
using Arcanic.Mediator.Samples.WebApi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Arcanic.Mediator.Samples.WebApi.Controllers
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
            var response = await _queryMediator.SendAsync(new Application.Product.Queries.Get.GetProductQuery
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
    }
}
