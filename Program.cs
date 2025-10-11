using CaffePOS.Data;
using CaffePOS.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));

//Đăng ký ItemService để DI có thể inject vào Controller
builder.Services.AddScoped<ItemsService>();
builder.Services.AddScoped<CategoryService>();


//Thêm Swagger để test API

builder.Services.AddControllers();             
builder.Services.AddEndpointsApiExplorer();    
builder.Services.AddSwaggerGen();             

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
