using Core_MVC.Filter;
using Core_MVC.GetApiLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Core_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // ���U IHttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // �K�[ Session �A��
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session ���Įɶ�
                options.Cookie.HttpOnly = true; // ���� JavaScript �X�� Cookie
                options.Cookie.IsEssential = true; // �� Session �b EU �� GDPR ���i��
            });

            //// �[�J���� Token �A��
            //// �w�� Microsoft.AspNetCore.Authentication.JwtBearer �M��

            //builder.Services.AddAuthentication(options => {
            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options => {
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"), // �������A���o���
            //        ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"), // �������A���[��
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key"))) // �������A���K�_
            //    };

            //    // �i�H�i�@�B�]�w��^
            //    options.Events = new JwtBearerEvents
            //    {
            //        OnChallenge = context =>
            //        {
            //            // �p�G�ШD���g���v�A�h���w�V��n������
            //            context.Response.Redirect("/Home/Index"); // �������z���n����������
            //            context.HandleResponse(); // ����i�@�B�B�z
            //            return Task.CompletedTask;
            //        }
            //    };

            //});



            // �إ� JWT �� DI �`�J
            var secretKey = builder.Configuration.GetValue<string>("JwtSettings:Key");
            var issuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
            var audience = builder.Configuration.GetValue<string>("JwtSettings:Audience");
            builder.Services.AddSingleton(new JwtHelper(secretKey, issuer, audience));


            // ���U IHttpClientFactory
            builder.Services.AddHttpClient();

            // �إ� ApiList �� DI �`�J�A�ê`�J JwtHelper�BHttp ����
            builder.Services.AddSingleton<ApiList>(provider =>
            {
                var jwtHelper = provider.GetRequiredService<JwtHelper>();
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
                var apiUrl = builder.Configuration.GetValue<string>("DataApiUrl");
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                return new ApiList(jwtHelper, httpClient, apiUrl,httpContextAccessor);
            });

            // �إ� ApiList �� DI �`�J�A�ê`�J JwtHelper ����
            builder.Services.AddScoped<AuthFilter>(); 

            // Filter ���� �i��غc�l
            //builder.Services.AddSingleton<AuthFilter>(provider =>
            //{
            //    var jwtHelper = provider.GetRequiredService<JwtHelper>();
            //    return new AuthFilter(jwtHelper);
            //});




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

            // �b UseRouting ����A���b UseAuthorization ���e�K�[ UseSession
            app.UseSession();

            // �i�� token ����(�|���n�J)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}