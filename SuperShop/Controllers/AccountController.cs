using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
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
            return View();
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
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username
                    };

                    //adicionar o user
                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    //se n conseguir criar o user -> aparece msg de erro.
                    if(result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model); //retorna a view q é o modelo -> n apaga os dados
                    }

                    //Se conseguir criar o user, fica automaticament logado. login por codigo
                    var loginViewModel = new LoginViewModel
                    {
                        //password do user q já está criado
                        Password = model.Password,
                        RememberMe = false,
                        Username = model.Username
                    };

                    //fazer o sign in
                    var result2 = await _userHelper.LoginAsync(loginViewModel);

                    //se conseguir fazer o login
                    if(result2.Succeeded)
                    {
                        //retornar para a página home
                        if(result2.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    //se n conseguir fazer o login -> envia msg de erro
                    ModelState.AddModelError(string.Empty, "The user couldn't be logged.");                    
                }
            }
            return View(model);
        }
    }
}
