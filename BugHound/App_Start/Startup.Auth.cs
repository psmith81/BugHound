using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using BugHound.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin.Security.Providers.LinkedIn;

namespace BugHound
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            // Facebooks, 
            //app.UseFacebookAuthentication(
            //   appId: "650303211754733",
            //   appSecret: "6fc7ca569b2788c9367d593fb908fef8");

            //app.UseGoogleAuthentication(
            //    "848034825051-a2puvvv3trq1b0o36tcd17hj56fdtun0.apps.googleusercontent.com",
            //    "-bffookm0eFBvb0fm_Z2dCds"
            //);

            //app.UseLinkedInAuthentication("77hbbwwgy2p8rg", "2eLZjbU6VAh7pHL5");

            Seed(new ApplicationDbContext());   // pbs. following added, source Microsoft
        }

        protected void Seed(ApplicationDbContext context)   // pbs. following added in class, source Microsoft
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Administrator"))
            {
                roleManager.Create(new IdentityRole("Administrator"));
                roleManager.Create(new IdentityRole("Developer"));
                roleManager.Create(new IdentityRole("Program Manager"));
                roleManager.Create(new IdentityRole("Support"));
            }
            var user = userManager.FindByName("CoderAdmin");
            if (user == null)
            {
                user = new ApplicationUser { UserName = "CoderAdmin", Email = "admin@coderfoundry.com" };
                var result = userManager.Create(user, "Password-1");

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }
            else
            {
                if (!userManager.IsInRole(user.Id, "Administrator"))
                {
                    userManager.AddToRole(user.Id, "Administrator");
                }
            }

        }
    }
}