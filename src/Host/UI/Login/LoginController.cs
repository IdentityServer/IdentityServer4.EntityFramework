using IdentityModel;
using IdentityServer4;
using IdentityServer4.Quickstart;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Host.UI.Login
{
    public class LoginController : Controller
    {
        private readonly InMemoryUserLoginService _loginService;
        private readonly IUserInteractionService _interaction;

        public LoginController(
            InMemoryUserLoginService loginService,
            IUserInteractionService interaction)
        {
            _loginService = loginService;
            _interaction = interaction;
        }

        [HttpGet("ui/login", Name = "Login")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new LoginViewModel(HttpContext);

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                vm.Username = context.LoginHint;
                vm.ReturnUrl = returnUrl;
            }

            return View(vm);
        }

        [HttpPost("ui/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                if (_loginService.ValidateCredentials(model.Username, model.Password))
                {
                    var user = _loginService.FindByUsername(model.Username);
                    await IssueCookie(user, "idsvr", "password");

                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            var vm = new LoginViewModel(HttpContext, model);
            return View(vm);
        }

        private async Task IssueCookie(
            InMemoryUser user, 
            string idp,
            string amr, 
            string sid = null)
        {
            var name = user.Claims.Where(x => x.Type == JwtClaimTypes.Name).Select(x => x.Value).FirstOrDefault() ?? user.Username;

            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Subject, user.Subject),
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.IdentityProvider, idp),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
                new Claim("role", "some_role")
            };
            if (sid != null)
            {
                claims.Add(new Claim(OidcConstants.EndSessionRequest.Sid, sid));
            }

            var ci = new ClaimsIdentity(claims, amr, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, cp);
        }

        [HttpGet("/ui/external/{provider}", Name = "External")]
        public IActionResult External(string provider, string returnUrl)
        {
            if (returnUrl != null)
            {
                returnUrl = UrlEncoder.Default.Encode(returnUrl);
            }
            returnUrl = "/ui/external-callback?returnUrl=" + returnUrl;

            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        [HttpGet("/ui/external-callback")]
        public async Task<IActionResult> ExternalCallback(string returnUrl)
        {
            var tempUser = await HttpContext.Authentication.AuthenticateAsync("Temp");
            if (tempUser == null)
            {
                throw new Exception();
            }

            var claims = tempUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x=>x.Type==JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            claims.Remove(userIdClaim);

            var provider = userIdClaim.Issuer;
            var userId = userIdClaim.Value;

            var user = _loginService.FindByExternalProvider(provider, userId);
            if (user == null)
            {
                user = _loginService.AutoProvisionUser(provider, userId, claims);
            }

            var sid = claims.FirstOrDefault(x => x.Type == OidcConstants.EndSessionRequest.Sid)?.Value;
            await IssueCookie(user, provider, "external", sid);
            await HttpContext.Authentication.SignOutAsync("Temp");

            if (_interaction.IsValidReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");

        }
    }
}
