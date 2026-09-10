using DataAccess;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IOdemeServisi, OdemeServisi>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Tek satırla her şey (DbContext ve Servisler) içeri yüklenir!
builder.Services.AddDataAccessServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();