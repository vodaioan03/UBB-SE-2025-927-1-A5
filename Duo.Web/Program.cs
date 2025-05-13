using DuoClassLibrary.Services;
using DuoClassLibrary.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Load API base URL from configuration
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
builder.Services.AddRazorPages();

// ✅ CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:7037")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials()
                     .WithExposedHeaders("Content-Type", "Accept");
    });
});

// Register HTTP clients for proxies
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

builder.Services.AddHttpClient<RoadmapServiceProxy>();
builder.Services.AddScoped<IRoadmapService, RoadmapService>();
builder.Services.AddScoped<IRoadmapServiceProxy, RoadmapServiceProxy>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ISectionServiceProxy, SectionServiceProxy>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IUserServiceProxy, UserServiceProxy>(client =>
{
    client.BaseAddress = new Uri(apiBase);
});


// Register services
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<ISectionService, SectionService>();

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

app.MapControllerRoute(
    name: "exercises",
    pattern: "Exercise/{action=Index}/{id?}",
    defaults: new { controller = "Exercise" });

app.MapControllerRoute(
    name: "coursePreview",
    pattern: "Course/{id?}",
    defaults: new { controller = "Course", action = "CoursePreview" });

app.Run();
