using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddCors(option =>
{
    option.AddPolicy("___my_cors", policy =>
        policy.AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .WithExposedHeaders("x-suggested-filename")
            .AllowCredentials().SetPreflightMaxAge(TimeSpan.FromDays(30))
    );
});
var app = builder.Build();

app.UseCors("___my_cors");
app.Use(async (context, next) =>
{
    var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Test");
    var reader = new StreamReader(context.Request.Body, Encoding.UTF8);

    var requestBody = await reader.ReadToEndAsync();
    var info = new StringBuilder();
    info.AppendLine("{");
    info.Append("  \"method\": \"").Append(context.Request.Method).Append("\",").AppendLine();
    info.Append("  \"uri\": \"").Append(context.Request.Path.Value).Append("\",").AppendLine();
    info.Append("  \"host\": \"").Append(context.Request.Host.Value).Append("\",").AppendLine();
    info.Append("  \"headers\": {").AppendLine();
    for (var i = 0; i < context.Request.Headers.Count; i++)
    {
        var kv = context.Request.Headers.ElementAt(i);
        info.Append("    \"").Append(kv.Key).Append("\": \"").Append(kv.Value).Append('"');
        if (i < context.Request.Headers.Count - 1)
        {
            info.Append(',');
        }

        info.AppendLine();
    }

    info.AppendLine("  },");
    info.Append("  \"querystring\": {").AppendLine();
    for (var i = 0; i < context.Request.Query.Count; i++)
    {
        var kv = context.Request.Query.ElementAt(i);
        info.Append("    \"").Append(kv.Key).Append("\": \"").Append(kv.Value).Append('"');
        if (i < context.Request.Query.Count - 1)
        {
            info.Append(',');
        }

        info.AppendLine();
    }

    info.AppendLine("  },");
    info.Append("  \"body\": \"").Append(EscapeStringForJson(requestBody)).Append('"').AppendLine();
    info.AppendLine("}");
    logger.LogInformation("{Request}", info.ToString());
    await next(context);
});

await app.RunAsync();
return;

static string EscapeStringForJson(string input)
{
    return input.Replace("\\", @"\\").Replace("\"", "\\\"");
}