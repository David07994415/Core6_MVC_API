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

            // 註冊 IHttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // 添加 Session 服務
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session 有效時間
                options.Cookie.HttpOnly = true; // 防止 JavaScript 訪問 Cookie
                options.Cookie.IsEssential = true; // 使 Session 在 EU 的 GDPR 中可用
            });

            //// 加入驗證 Token 服務
            //// 安裝 Microsoft.AspNetCore.Authentication.JwtBearer 套件

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
            //        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"), // 替換為你的發行者
            //        ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"), // 替換為你的觀眾
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key"))) // 替換為你的密鑰
            //    };

            //    // 可以進一步設定返回
            //    options.Events = new JwtBearerEvents
            //    {
            //        OnChallenge = context =>
            //        {
            //            // 如果請求未經授權，則重定向到登錄頁面
            //            context.Response.Redirect("/Home/Index"); // 替換成您的登錄頁面路由
            //            context.HandleResponse(); // 防止進一步處理
            //            return Task.CompletedTask;
            //        }
            //    };

            //});



            // 建立 JWT 的 DI 注入
            var secretKey = builder.Configuration.GetValue<string>("JwtSettings:Key");
            var issuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
            var audience = builder.Configuration.GetValue<string>("JwtSettings:Audience");
            builder.Services.AddSingleton(new JwtHelper(secretKey, issuer, audience));


            // 註冊 IHttpClientFactory
            builder.Services.AddHttpClient();

            // 建立 ApiList 的 DI 注入，並注入 JwtHelper、Http 實體
            builder.Services.AddSingleton<ApiList>(provider =>
            {
                var jwtHelper = provider.GetRequiredService<JwtHelper>();
                var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
                var apiUrl = builder.Configuration.GetValue<string>("DataApiUrl");
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                return new ApiList(jwtHelper, httpClient, apiUrl,httpContextAccessor);
            });

            // 建立 ApiList 的 DI 注入，並注入 JwtHelper 實體
            builder.Services.AddScoped<AuthFilter>(); 

            // Filter 不能 進行建構子
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

            // 在 UseRouting 之後，但在 UseAuthorization 之前添加 UseSession
            app.UseSession();

            // 進行 token 驗證(會員登入)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}