using hellodoc;
using Microsoft.EntityFrameworkCore;
using hellodoc.Repositories.Repository;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.Repositories.Services.Interface;
using hellodoc.Repositories.Services;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<hellodoc.DbEntity.DataContext.ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);

//builder.Services.AddScoped<IAdminDashRepository, AdminDashRepository>();
builder.Services.AddScoped<IPatientLogin, PatientLogin>();
builder.Services.AddScoped<IRequests, RequestAll>();
builder.Services.AddScoped<IPatientDashboard, PatientDashboard>();
builder.Services.AddScoped<IAdminDashRepository, AdminDashRepository>();
builder.Services.AddScoped<IAuthManager , AuthManager>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAdminProviders, AdminProvides>();
builder.Services.AddScoped<IAdminAccess, AdminAccess>();
builder.Services.AddScoped<IAdminRecords, AdminRecords>();
builder.Services.AddScoped<IAdminPartners, AdminPartners>();
builder.Services.AddScoped<IAdminProviderLocation, AdminProviderLocation>();    
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patient}/{action=send_request}/{id?}");

app.Run();
