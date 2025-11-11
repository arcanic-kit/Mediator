using System.Reflection;
using Arcanic.Mediator;
using Arcanic.Mediator.Command;
using Arcanic.Mediator.Event;
using Arcanic.Mediator.Query;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Example: Add Arcanic Mediator with modules
builder.Services.AddArcanicMediator(moduleRegistry =>
{
    moduleRegistry.AddCommandModule(commandModuleBuilder =>
    {
        commandModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });

    moduleRegistry.AddQueryModule(queryModuleBuilder =>
    {
        queryModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });

    moduleRegistry.AddEventModule(eventModuleBuilder =>
    {
        eventModuleBuilder.RegisterFromAssembly(Assembly.GetExecutingAssembly());
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
