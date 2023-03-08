using System.Collections.Concurrent;

namespace CloudMockApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var cache = new ConcurrentDictionary<string, MockApiEntry>();

        app.MapGet("/", () => 
        {
            return Results.Redirect("/swagger");
        })
        .WithName("Home")
        .WithOpenApi();

        app.MapGet("/list", () =>
        {
            return cache.ToList();
        })
        .WithName("List")
        .WithOpenApi();

        app.MapGet("/list/{id}", (string id) =>
        {
            if (cache.TryGetValue(id, out MockApiEntry? entry))
            {
                return Results.Ok(entry);
            }

            return Results.NotFound();
        })
            .WithName("list-entry")
            .WithOpenApi();

        app.MapPost("/list/create", (MockApiCreateModel create) =>
        {
            if (!create.IsValid())
                return Results.BadRequest();

            var toAdd = new MockApiEntry(create);
            if (cache.TryAdd(toAdd.Id, toAdd))
            {
                return Results.Created($"/list/{toAdd.Id}", toAdd);
            }

            return Results.Problem();
        })
            .WithName("create")
            .WithOpenApi();

        app.MapGet("/test/{id}", (string id) =>
        {
            if (cache.TryGetValue(id, out MockApiEntry? entry))
            {
                switch (entry.Status)
                {
                    case ResponseStatus.Ok:
                        {
                            if (entry.Content != null)
                                return Results.Ok(entry.Content);
                            return Results.Ok();
                        }
                    case ResponseStatus.Accepted:
                        return Results.Accepted();
                }
            }

            return Results.NotFound();
        });

        app.Run();
    }
}