using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core_MVC
{
    public class JwtHelper
    {
        // 加入套件：Microsoft.AspNetCore.Authentication.JwtBearer
        // AppSetting 加入相關設定

        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtHelper(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public  string GenerateToken()  // string userId
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Token 有效期
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string tokenInput)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey); // 確保這裡使用的是正確的密鑰
            try
            {
                // 驗證 JWT
                tokenHandler.ValidateToken(tokenInput, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true, // 需要驗證發行者
                    ValidIssuer = _issuer, // 設置為您預期的發行者
                    ValidateAudience = true, // 需要驗證觀眾
                    ValidAudience = _audience, // 設置為您預期的觀眾
                    ValidateLifetime = true, // 驗證 Token 是否過期
                    ClockSkew = TimeSpan.Zero // 禁用時鐘偏差
                }, out SecurityToken validatedToken);


                //// 如果需要，您可以在這裡進一步處理已驗證的 Token，例如提取 Claims
                //var jwtToken = (JwtSecurityToken)validatedToken;
                //var userId = jwtToken.Claims.First(claim => claim.Type == "id").Value; // 假設您在 Token 中有 id Claim

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token 過期的處理
                // 例如，可以重定向到登錄頁面或返回未授權的狀態

                return false;
            }
            catch (Exception ex)
            {
                // 其他驗證錯誤的處理
                return false;
            }
        }

    }
}
