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

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name); //encontrar o user

            //criar o modelo - para aparecem os dados -> é como o edit
            var model = new ChangeUserViewModel();
            if(user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }
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
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

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
    }
}
