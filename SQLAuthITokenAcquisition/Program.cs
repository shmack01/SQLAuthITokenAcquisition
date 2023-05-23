using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

//IConfiguration Configuration = null;

// Add services to the container.
builder.Services.AddControllersWithViews().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder() // Require all users to authenticate
                  .RequireAuthenticatedUser()
                  .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();
builder.Services.AddControllersWithViews();
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd").EnableTokenAcquisitionToCallDownstreamApi(ca => new ConfidentialClientApplicationOptions
{
    AzureCloudInstance = AzureCloudInstance.AzureUsGovernment,

},
   new[] { @"https://sql.azuresynapse.usgovcloudapi.net/.default" }
).AddInMemoryTokenCaches();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


