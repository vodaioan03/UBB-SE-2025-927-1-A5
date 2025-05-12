using DuoClassLibrary.Services;
using DuoClassLibrary.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var apiBase = builder.Configuration["Api:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBase))
{
    throw new InvalidOperationException("Missing Api:BaseUrl in appsettings.json");
}

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IQuizServiceProxy, QuizServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpClient<ICourseServiceProxy, CourseServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpClient<IExerciseServiceProxy, ExerciseServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddHttpClient<ISectionServiceProxy, SectionServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});

builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();

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
