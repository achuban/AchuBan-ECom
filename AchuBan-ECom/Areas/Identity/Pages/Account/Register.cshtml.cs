using AchuBan_Ecom.DataAccess.Repository.IRepository;
using AchuBan_ECom.Models.Models;
using AchuBan_ECom.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace AchuBan_ECom.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "State")]
            public string State { get; set; }

            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Role { get; set; }
            [ValidateNever]
            public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RoleList { get; set; }

            public int? CompanyId { get; set; }
            [ValidateNever]
            public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> CompanyList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                }
                if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                }
                if (!await _roleManager.RoleExistsAsync(SD.Role_Employee))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                }
                if (!await _roleManager.RoleExistsAsync(SD.Role_Company))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
                }

                Input = new InputModel()
                {
                    RoleList = _roleManager.Roles.Select(r => r.Name).Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = r,
                        Value = r
                    }),
                    CompanyList = _unitOfWork.CompanyRepository.GetAll().Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Id.ToString()
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Role creation failed");
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Name = Input.Name;
                user.StreetAddress = Input.StreetAddress;
                user.City = Input.City;
                user.State = Input.State;
                user.PostalCode = Input.PostalCode;

                if(Input.Role==SD.Role_Company)
                {
                    user.CompanyId = Input.CompanyId;
                }


                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (String.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not abstract and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
