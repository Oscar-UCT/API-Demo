using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Recordatorios") ?? "Data Source=Recordatorios.db";
builder.Services.AddSqlite<RecordatoriosDb>(connectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "SimpleAPI";
    config.Title = "SimpleAPI v1";
    config.Version = "v1";
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/recordatorios", async (RecordatoriosDb db) =>

    await db.Recordatorios.ToListAsync()
);

app.MapPost("/recordatorios", async (Recordatorio recordatorio, RecordatoriosDb db) =>
{
    db.Recordatorios.Add(recordatorio);
    await db.SaveChangesAsync();
    return Results.Created($"/recordatorios/{recordatorio.Id}", recordatorio);
});

app.MapPut("/recordatorios/{id}", async (int id, Recordatorio inputRecordatorio, RecordatoriosDb db) =>
{
    var recordatorio = await db.Recordatorios.FindAsync(id);

    if (recordatorio is null) return Results.NotFound();

    recordatorio.Nombre = inputRecordatorio.Nombre;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/recordatorios/{id}", async (int id, RecordatoriosDb db) =>
{
    if (await db.Recordatorios.FindAsync(id) is Recordatorio recordatorio)
    {
        db.Recordatorios.Remove(recordatorio);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.MapPatch("/recordatorios/{id}", async (int id, Recordatorio recordatorioActualizado, RecordatoriosDb db) =>
{
    var recordatorio = await db.Recordatorios.FindAsync(id);
    if (recordatorio is null) return Results.NotFound();

    if (recordatorioActualizado.Nombre is not null)
        recordatorio.Nombre = recordatorioActualizado.Nombre;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
