namespace RobotApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders(
                        "Content-Disposition", 
                        "Color-1", "Color-2", "Color-3", "Color-4", "Color-5",
                        "Color-6", "Color-7", "Color-8", "Color-9", "Color-10"
                    );
            });
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}