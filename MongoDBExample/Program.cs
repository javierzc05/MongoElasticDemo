using MongoDBExample.Entities;
using MongoDBExample.Repositories.Interfaces;
using MongoDBExample.Repositories;
using MongoDBExample.Services;
using MongoDBExample.Services.Profiles;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDBExample.Services.Interfaces;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDBExample.Services.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// AppSettings
builder.Services.Configure<MongoDbExampleDatabaseSettings>(
    builder.Configuration.GetSection("MongoDbExampleDatabase"));
builder.Configuration.AddEnvironmentVariables().AddUserSecrets(Assembly.GetExecutingAssembly(), true);
                     
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = builder.Configuration["MongoDbExampleDatabase:ConnectionString"],
        DatabaseName = builder.Configuration["MongoDbExampleDatabase:DatabaseName"]
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
    }
};

builder.Services.ConfigureMongoDbIdentity<User, Role, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoleManager<RoleManager<Role>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(x =>
{
    x.DefaultChallengeScheme = "Bearer";
    x.DefaultAuthenticateScheme = "Bearer";
}).AddJwtBearer(x =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["MongoDbExampleDatabase:Jwt:Secret"]);

    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["MongoDbExampleDatabase:Jwt:Issuer"],
        ValidAudience = builder.Configuration["MongoDbExampleDatabase:Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<GenreProfile>();
    config.AddProfile<ConcertProfile>();
});


builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IConcertRepository, ConcertRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSingleton<GenreService>();
builder.Services.AddSingleton<ConcertService>();
builder.Services.AddElasticSearch(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<IFileUploader, FileUploader>();
} else
{
    builder.Services.AddTransient<IFileUploader, FirebaseStorageUploader>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
