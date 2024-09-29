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

            // �[�J Cores �A��
            //builder.Services.AddAntiforgery
            builder.Services.AddCors(options => {
                options.AddPolicy("MVC_Front", builder1 =>
                {
                    builder1.WithOrigins(builder.Configuration.GetValue<string>("HostsList"))
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                });
            });

            // �[�J���� Token �A��
            // �w�� Microsoft.AspNetCore.Authentication.JwtBearer �M��

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
                    ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"), // �������A���o���
                    ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:Audience"), // �������A���[��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Key"))) // �������A���K�_
                };

                // �i�H�i�@�B�]�w��^�����~�N�X
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized__access" }));
                        context.HandleResponse(); // ����i�@�B�B�z
                        return Task.CompletedTask;
                    }
                };

            });


            // �إ� JWT �� DI �`�J
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


            // app.UseRouting(); // �ҥθ��� �B�~�[��

            app.UseCors("MVC_Front"); // �[�J Cors
            app.UseAuthentication(); // �b���Ѥ��e�ϥλ{��
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
