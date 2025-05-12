using DuoClassLibrary.Services;
using DuoClassLibrary.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var handler = new HttpClientHandler

var apiBase = builder.Configuration["Api:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBase))

{
    throw new InvalidOperationException("Missing Api:BaseUrl in appsettings.json");
}

builder.Services
    .AddHttpClient<IQuizServiceProxy, QuizServiceProxy>(client =>
    {
        client.BaseAddress = new Uri(apiBase);
    });

builder.Services
    .AddSingleton<IQuizService, QuizService>();

builder.Services
    .AddHttpClient<ICourseServiceProxy, CourseServiceProxy>(client =>
    {
        client.BaseAddress = new Uri(apiBase);
    });

builder.Services
    .AddHttpClient<IExerciseServiceProxy, ExerciseServiceProxy>(c =>
    {
        c.BaseAddress = new Uri(apiBase);
    });


builder.Services.AddSingleton(httpClient);
builder.Services.AddScoped<ICourseServiceProxy, CourseServiceProxy>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISectionServiceProxy, SectionServiceProxy>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IQuizServiceProxy, QuizServiceProxy>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();

// ✅ CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("https://localhost:7037")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .WithExposedHeaders("Content-Type", "Accept");
    });
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "courses",
    pattern: "Course/{action=ViewCourses}/{id?}");

app.Run();
