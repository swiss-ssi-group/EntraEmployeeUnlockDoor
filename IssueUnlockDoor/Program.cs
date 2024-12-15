using IssueUnlockDoor;

namespace EmployeeUnlockDoor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddSecurityHeaderPolicies()
         .SetPolicySelector(ctx => SecurityHeadersDefinitions
             .GetHeaderPolicyCollection(builder.Environment.IsDevelopment()));


        services.Configure<CredentialSettings>(configuration.GetSection("CredentialSettings"));
        services.AddHttpClient();
        services.AddScoped<IssuerService>();
        services.AddDistributedMemoryCache();

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => false;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddRazorPages();

        var app = builder.Build();

        app.UseSecurityHeaders();


        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}




