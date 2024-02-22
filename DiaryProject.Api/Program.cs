using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DiaryProject.Api.Context;
using DiaryProject.Api.Context.Repository;
using DiaryProject.Api.Extension;
using DiaryProject.Api.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DiaryContext>(option =>
    {
        var connectionString = builder.Configuration.GetConnectionString("MemoConnection");
        option.UseSqlite(connectionString);
    }).AddUnitOfWork<DiaryContext>()
    .AddCustomRepository<Memo, MemoRepository>()
    .AddCustomRepository<User, UserRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IMemoService, MemoService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddControllers();

var mapperConfig = new MapperConfiguration(c => c.AddProfile(new ServerMapperProfile()));
builder.Services.AddSingleton(mapperConfig.CreateMapper());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();