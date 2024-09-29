using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 加入 Cores 服務
            //builder.Services.AddAntiforgery
            builder.Services.AddCors(options => {
                options.AddPolicy("MVC_Front", builder1 =>
                {
                    builder1.WithOrigins(builder.Configuration.GetValue<string>("HostsList"))
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                });
            });

            // 加入驗證 Token 服務
            // 安裝 Microsoft.AspNetCore.Authentication.JwtBearer 套件

            builder.Services.AddAuthentication(options => { 
                options.DefaultScheme =JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, 
                    ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"), // 替換為你的發行者
                    ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"), // 替換為你的觀眾
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key"))) // 替換為你的密鑰
                };

                // 可以進一步設定返回的錯誤代碼
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized__access" }));
                        context.HandleResponse(); // 防止進一步處理
                        return Task.CompletedTask;
                    }
                };

            });


            // 建立 JWT 的 DI 注入
            var secretKey = builder.Configuration.GetValue<string>("JwtSettings:Key");
            var issuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
            var audience = builder.Configuration.GetValue<string>("JwtSettings:Audience");
            builder.Services.AddSingleton(new JwtHelper(secretKey, issuer, audience));



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //});
            }

            app.UseHttpsRedirection();


            // app.UseRouting(); // 啟用路由 額外加的

            app.UseCors("MVC_Front"); // 加入 Cors
            app.UseAuthentication(); // 在路由之前使用認證
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
