using Arcanic.Mediator.Command.Abstractions;
using Arcanic.Mediator.Query.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Arcanic.Mediator.Samples.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICommandMediator _commandMediator;
        private readonly IQueryMediator _queryMediator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICommandMediator commandMediator, IQueryMediator queryMediator)
        {
            _logger = logger;
            _commandMediator = commandMediator;
            _queryMediator = queryMediator;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var product = await _queryMediator.SendAsync(new Application.Product.Queries.Get.GetProductQuery
            {
                Id = 1
            });

            var response = await _commandMediator.SendAsync(new Application.Product.Commands.Add.AddProductCommand
            {
                Name = "Sample Product",
                Price = 9.99m
            });

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
