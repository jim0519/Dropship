using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Dropship.Models.User;
using DropshipCommon.Models;
using DropshipBusiness.User;
using DropshipFramework.Controllers;
using DropshipFramework;

namespace Dropship.Controllers
{
    //[Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        public UserController(IUserService userService,
            IWorkContext workContext)
        {
            _userService = userService;
            _workContext = workContext;
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model,string returnURL)
        {
            if (ModelState.IsValid)
            {
                _userService.SignIn(model.Email, model.Password);
            }

            if (String.IsNullOrEmpty(returnURL) || !Url.IsLocalUrl(returnURL))
                return RedirectToRoute("Default");

            return Redirect(returnURL);

        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var newUser = new T_User();
                newUser.Email = model.Email;
                newUser.Name = model.Name;
                newUser.Password = model.Password;

                _userService.RegisterUser(newUser);

                _userService.SignIn(model.Email, model.Password);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }



        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _userService.SignOut();

            return RedirectToAction("Login", "User");
        }

        
    }
}