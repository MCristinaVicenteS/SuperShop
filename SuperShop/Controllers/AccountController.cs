using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace SuperShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly ICountryRepository _countryRepository;

        public AccountController(IUserHelper userHelper, IMailHelper mailHelper, IConfiguration configuration, ICountryRepository countryRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _countryRepository = countryRepository;
        }

        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
            return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if(result.Succeeded)
                {
                    if(this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }
                    return this.RedirectToAction("Index", "Home");
                }
            }
            this.ModelState.AddModelError(string.Empty, "Failded to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            var model = new RegisterNewUserViewModel
            {
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(0)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if(ModelState.IsValid)
            {
                //ver se o user já existe
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                //se o user n existir -> criar
                if (user == null)
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        CityId = model.CityId,
                        City = city,
                    };

                    //adicionar o user
                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    //se n conseguir criar o user -> aparece msg de erro.
                    if(result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model); //retorna a view q é o modelo -> n apaga os dados
                    }

                    ////Se conseguir criar o user, fica automaticament logado. login por codigo
                    //var loginViewModel = new LoginViewModel
                    //{
                    //    //password do user q já está criado
                    //    Password = model.Password,
                    //    RememberMe = false,
                    //    Username = model.Username
                    //};

                    ////fazer o sign in
                    //var result2 = await _userHelper.LoginAsync(loginViewModel);


                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);


                    //Gerar a response para enviar o mail(user, subject e body -> od vai o link do token)
                    Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"place click in this link:</br></br><a href =\"  {tokenLink}\">Confirm Email</a>");

                    //se conseguir fazer o login
                    if(response.IsSuccess)
                    {
                        ViewBag.Message = "The instructions to allow you user has been sent to email";
                        return View(model);
                    }

                    //se n conseguir fazer o login -> envia msg de erro
                    ModelState.AddModelError(string.Empty, "The user couldn't be logged.");                    
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //encontrar o user

            //criar o modelo - para aparecem os dados -> é como o edit
            var model = new ChangeUserViewModel();
            if(user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;

                var city = await _countryRepository.GetCityAsync(user.CityId);
                if(city != null)
                {
                    var country = await _countryRepository.GetCountryAsync(city);
                    if(country != null)
                    {
                        model.CountryId = country.Id;
                        model.Cities = _countryRepository.GetComboCities(country.Id);
                        model.Countries = _countryRepository.GetComboCountries();
                        model.CityId = user.CityId;
                    }
                }
            }

            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Countries = _countryRepository.GetComboCountries();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            //confirmar se o user existe
            if(ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //encontrar o user

                //verficar se user n é nulo
                if (user != null)
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.CityId = model.CityId;
                    user.City = city;

                    //fazer o update
                    var response = await _userHelper.UpdateUserAsync(user);

                    //se fizer o upate
                    if(response.Succeeded)
                    {
                        ViewBag.UserMessage = "User update!";
                    }

                    //se n fizer o update - envia msg de erro
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }
                        
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //verificar se o user existe
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                
                //se o user existir -> posso mudar a pass
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found.");
                }

            }

            return this.View(model);
        }

        //Criar token para api
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if(!result.Succeeded) 
            {
                return NotFound();
            }

            return View();
        }


        public IActionResult RecoverPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task <IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if(this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "Shop Password Reset", $"<h1>Shop Password Reset</h1>" +
                    $"To reset the password click in this link:</br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    this.ViewBag.Message = "The instruction to recover your password has been sent to email.";
                }

                return this.View();
            }

            return this.View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if(user !=null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful";
                    return View();
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found";
            return View(model);
        }


        public IActionResult NotAuthorized()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/GetCitiesAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);
            return Json(country.Cities.OrderBy(c => c.Name));
        }

    }
}
