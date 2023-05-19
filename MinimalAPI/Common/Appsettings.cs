using Microsoft.Extensions.Configuration;
using System;
namespace MinimalAPI.Common
{
    public class Appsettings
    {
        private static IConfigurationSection _configuration;
        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }
        public static string JWTSecretKey => _configuration["JWTSecretKey"];
        public static string BaseUrl => _configuration["BaseUrl"];
       // public static string DefaultProfilePic => BaseUrl + FolderNames.BaseFolderName + "/images/defaultAvatar.png";
        // public static string GoogleAPIKey => _configuration["GoogleApiKey"];
        //public static string SocialLoginKey => _configuration["SocialLoginKey"];

        //SMTP
        public static string SMTPFrom => _configuration["SMTPFrom"];
        public static string SMTPHost => _configuration["SMTPHost"];
        public static string SMTPDisplayName => _configuration["SMTPDisplayName"];
        public static int SMTPPort => Convert.ToInt32(_configuration["SMTPPort"]);
        public static string SMTPUsername => _configuration["SMTPUsername"];
        public static string SMTPPassword => _configuration["SMTPPassword"];
        public static string SupportEmail => _configuration["SupportEmail"];



        public static string TwilioAccountSid => _configuration["TwilioAccountSid"];
        public static string TwilioAccountToken => _configuration["TwilioAccountToken"];
        public static string MessagingServiceSid => _configuration["MessagingServiceSid"];
        public static string FromNumber => _configuration["FromNumber"];
        public static string StripeApiKey => _configuration["StripeSecretKey"];
        public static string AppleAppSecret => _configuration["AppleAppSecret"];
        public static string Stripe_Secret_key => _configuration["Stripe_Secret_key"];
        public static bool Stripe_Live => Convert.ToBoolean(_configuration["Stripe_Live"]);
        public static bool AppleInAppPurchaseLive => Convert.ToBoolean(_configuration["AppleInAppPurchaseLive"]);
    }
}
