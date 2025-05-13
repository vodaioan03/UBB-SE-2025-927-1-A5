using DuoClassLibrary.Services;
using DuoClassLibrary.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services
    .AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<ICoinsServiceProxy, CoinsServiceProxy>(); 
builder.Services.AddScoped<ICoinsService, CoinsService>(); 

builder.Services.AddScoped<IExerciseService, ExerciseService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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

app.MapControllerRoute(
    name: "coursePreview",
    pattern: "Course/{id?}",
    defaults: new { controller = "Course", action = "CoursePreview" });

app.Run();
