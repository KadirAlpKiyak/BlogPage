﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using FİNALPROJE.Models;

namespace FİNALPROJE
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // E-posta göndermek için e-posta hizmetinizi buraya ekleyin.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Kısa mesaj göndermek için SMS hizmetinizi buraya ekleyin.
            return Task.FromResult(0);
        }
    }

    // Bu uygulamada kullanılan uygulama kullanıcı yöneticisini yapılandırın. UserManager ASP.NET Identity'de tanımlanır ve uygulama tarafından kullanılır.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Kullanıcı adları için doğrulama mantığını yapılandırın
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Parolalar için doğrulama mantığını yapılandırın
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Kullanıcı kilitleme varsayılanlarını yapılandırın
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // İki öğeli kimlik doğrulama sağlayıcılarını kaydedin. Bu uygulama, kullanıcıyı doğrulamak için bir kod almak üzere Telefon ve E-posta'yı kullanır
            // Kendi sağlayıcınızı yazabilir ve buraya ekleyebilirsiniz.
            manager.RegisterTwoFactorProvider("Telefon Kodu", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Güvenlik kodunuz: {0}"
            });
            manager.RegisterTwoFactorProvider("E-posta Kodu", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Güvenlik Kodu",
                BodyFormat = "Güvenlik kodunuz: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Uygulamada kullanılan uygulama oturum açma yöneticisini yapılandırın.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
