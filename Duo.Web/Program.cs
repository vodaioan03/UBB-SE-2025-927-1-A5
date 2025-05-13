using DuoClassLibrary.Services;
using DuoClassLibrary.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure HttpClient to allow untrusted SSL (e.g., for localhost APIs with self-signed certs)
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};
builder.Services.AddSingleton(new HttpClient(handler));

// Register Course services
builder.Services.AddScoped<ICourseServiceProxy, CourseServiceProxy>();
builder.Services.AddScoped<ICourseService, CourseService>();

// Register Exercise services with proxy injection
builder.Services.AddScoped<IExerciseServiceProxy>(sp =>
    new ExerciseServiceProxy(sp.GetRequiredService<HttpClient>()));
builder.Services.AddScoped<IExerciseService>(sp =>
    new ExerciseService(sp.GetRequiredService<IExerciseServiceProxy>()));

// Register Quiz services with proxy injection
builder.Services.AddScoped<IQuizServiceProxy>(sp =>
    new QuizServiceProxy(sp.GetRequiredService<HttpClient>()));
builder.Services.AddScoped<IQuizService>(sp =>
    new QuizService(sp.GetRequiredService<IQuizServiceProxy>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();
