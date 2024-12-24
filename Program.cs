

////    // Add DbContext
////    services.AddDbContext<WeatherDbContext>(options =>
////        options.UseSqlServer(
////            configuration.GetConnectionString("DefaultConnection"),
////            b => b.MigrationsAssembly("WeatherAPI")));

////    // Add GraphQL Services
////    services.AddGraphQL(builder => builder
////        .AddSystemTextJson()
////        .AddGraphTypes(typeof(WeatherSchema).Assembly)
////        .AddSchema<WeatherSchema>()
////        .AddGraphTypes()
////        .AddDataLoader()
////        .AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = true));

////    // Add Memory Cache
////    services.AddMemoryCache();

////    // Add HTTP Client
////    services.AddHttpClient();

////    // Add Controller support (if needed)
////    services.AddControllers();
////}

////// App Configuration Method
////void ConfigureApp(WebApplication app)
////{
////    if (app.Environment.IsDevelopment())
////    {
////        app.UseDeveloperExceptionPage();
////    }

////    // Use CORS
////    app.UseCors("AllowAll");

////    // Use GraphQL endpoints
////    app.UseGraphQL("/graphql");
////    // Use routing and endpoints
////    app.UseRouting();
////    app.UseEndpoints(endpoints =>
////    {
////        endpoints.MapControllers();
////        endpoints.MapGraphQL();
////    });
////}
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Weather_Acknowledgement.GraphQL_Types;
using Weather_Acknowledgement.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<WeatherMutation>()
    .AddType<WeatherStationType>()
    .AddType<WeatherReadingType>()
    .AddErrorFilter(error =>
    {
        return error.WithMessage($"Custom Error: {error.Message}");
    });
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
builder.Services.AddScoped<WeatherStationType>();
builder.Services.AddScoped<WeatherReadingType>();
builder.Services.AddScoped<WeatherQuery>();
builder.Services.AddScoped<WeatherMutation>();
builder.Services.AddScoped<WeatherSchema>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapSwagger();
    endpoints.MapGraphQL();
});

app.Run();