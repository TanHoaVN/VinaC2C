﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using VinaC2C.Data.Context;
using VinaC2C.Data.Services;
using VinaC2C.MVC.Models;
using VinaC2C.Ultilities.Helpers;

namespace VinaC2C.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        private readonly VinaC2CContext _context;

        public UserService userService;

        public SendRegisterInforService sendRegisterInforService;

        public UserController(VinaC2CContext context, IWebHostEnvironment environment)
        {
            this._context = context;
            this._environment = environment;
            userService = new UserService(context);
            sendRegisterInforService = new SendRegisterInforService();
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Detail()
        {
            return View("Register");
        }

        public async Task<IActionResult> Register(UserModel model)
        {
            if(ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Mật Khẩu Xác Nhận Không Khớp");
                    return View("Register");
                }
                else if (userService.IsExistEmail(model.Email).Result)
                {
                    ModelState.AddModelError("", "Email Đã Được Sử Dụng");
                    return View("Register");
                }
                else if (userService.IsExistUsername(model.Username).Result)
                {
                    ModelState.AddModelError("", "Tên Đăng Nhập Đã Được Sử Dụng");
                    return View("Register");
                }
                else
                {
                    var registerUser = new Data.Models.User();
                    MapObjectHelper.MapDefault(model, out registerUser);
                    await userService.RegisterUserDefautl(registerUser);
                    //For test only
                    string templatePath = _environment.WebRootPath + Ultilities.AppInfor.AppGlobal.RegisterMailTemplatePath;
                    sendRegisterInforService.SendConfirmLink(registerUser, templatePath);
                    
                }
                return View("Index");
            }
            else
                return View("Register");
        }
        
        public IActionResult ResetPassword()
        {
            return View();
        }

        [AcceptVerbs("POST")]
        public ActionResult Login(UserModel model)
        {
            if(model != null)
            {
                if(userService.LoginByUsernameAndPassword(model.Username,model.Password).Result)
                {
                    var user = userService.GetByUsername(model.Username);
                    var claims = ClaimHelper.GetListFromObject(user);
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);
                    return Json(new { result = "Valid", message = Url.Action("Index", "Dashboard") });
                }
                else
                {
                    return Json( new {result = "Invalid", message = "Tên Đăng Nhập Hoặc Mật Khẩu Không Hợp Lệ" });
                }
            }
            else
            {
                return Json(new { result = "Invalid", message = "" });
            }
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ListView()
        {
            return View();
        }

        public async Task<ActionResult> GetAllAsyncToList()
        {
            return Json(await userService.GetAllToListAsync());
        }
    }
}