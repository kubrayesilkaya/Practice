using Microsoft.IdentityModel.Tokens;
using Practice.Model.Core;
using Practice.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Practice.Services
{
    public class TokenHandler : ITokenHandler
    {

        //kaynak https://www.gencayyildiz.com/blog/asp-net-core-3-1-ile-token-bazli-kimlik-dogrulamasi-ve-refresh-token-kullanimijwt/ 

        private readonly IConfiguration configuration;
        public TokenHandler(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public JwtResponseModel CreateAccessToken(int idUSer)
        {
            JwtResponseModel token = new();

            //Security  Key'in simetriğini alıyoruz.
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));

            //Şifrelenmiş kimliği oluşturuyoruz.
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Oluşturulacak token ayarlarını veriyoruz.
            token.expiration = DateTime.Now.AddMinutes(5);
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: configuration["Token:Issuer"],
                audience: configuration["Token:Audience"],
                expires: token.expiration,//Token süresini 5 dk olarak belirliyorum
                notBefore: DateTime.Now,//Token üretildikten ne kadar süre sonra devreye girsin ayarlıyouz.
                signingCredentials: signingCredentials
                );

            //Token oluşturucu sınıfında bir örnek alıyoruz.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            //Token üretiyoruz.
            token.accessToken = tokenHandler.WriteToken(securityToken);

            //Refresh Token üretiyoruz.
            token.refreshToken = CreateRefreshToken();

            return token;
        }

        //Refresh Token üretecek metot.
        public string CreateRefreshToken()
        {
            byte[] number = new byte[32];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(number);
                return Convert.ToBase64String(number);
            }
        }


    }
}
