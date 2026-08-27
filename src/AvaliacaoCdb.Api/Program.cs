using AvaliacaoCdb.Api.ErrorHandling;
using AvaliacaoCdb.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<CalculationExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new()
{
    Title = "Avaliação CDB API",
    Version = "v1",
    Description = "Calcula os resultados bruto e líquido de um investimento em CDB."
}));
builder.Services.AddSingleton<ITaxRatePolicy, RegressiveTaxRatePolicy>();
builder.Services.AddSingleton<ICdbCalculator, CdbCalculator>();
builder.Services.AddCors(options => options.AddPolicy("AngularDevelopment", policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "Avaliação CDB API";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Avaliação CDB API v1");
});

if (app.Environment.IsDevelopment())
{
    app.UseCors("AngularDevelopment");
}

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.RunAsync();
