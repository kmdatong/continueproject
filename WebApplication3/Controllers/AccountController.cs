using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApplication3.Domains;
using WebApplication3.Filter;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IRepstoriy<Account> _account;
        private IMemoryCache _cache;

        public AccountController(
            IRepstoriy<Account> account,
            IMemoryCache cache
            )
        {
            _account = account;
            _cache = cache;
        }
        [HttpGet]
        [LoginRequired]
        public IActionResult Edit(int id=0)
        {
            AccountModel model = new AccountModel();
            if (id == 0)
            {
                model.Password = "123456";
                return View(model);
            }
              

            var entity = _account.GetById(id);
            if (entity == null)
            {
                model.Password = "123456";
                return View(model);
            }

            model.Id = entity.Id;
            model.Age = entity.Age;
            model.Name = entity.Name;
            model.Remark = entity.Remark;
            model.LoginName = entity.LoginName;

            return View(model);
        }
        [HttpPost]
        [LoginRequired]
        public ActionResult Edit(AccountModel model)
        {
            
            var entity = _account.GetById(model.Id);
            if (entity == null)
            {
                int exsitCount = _account.Quyer()
                    .Where(s => s.LoginName == model.LoginName.Trim() || s.Name == model.Name.Trim())
                    .Count();
                if (exsitCount > 0)
                {
                    ModelState.AddModelError("", "登录名或用户已存在");
                    return View(model);
                }

                 entity = new Account();
                entity.Password = "123456";
                _account.Inserter(entity);
            }
            
            entity.Age = model.Age;
            entity.Name = model.Name;
            entity.Remark = model.Remark;
            entity.LoginName = model.LoginName;

            _account.Save();

           var list = _account.Quyer().ToList().Select(s => {
                var m = new AccountModel();
                m.Id = s.Id;
                m.Age = s.Age;
                m.Name = s.Name;
                m.Remark = s.Remark;
                return m;
            }).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
              // Keep in cache for this time, reset time if accessed.
              .SetSlidingExpiration(TimeSpan.FromDays(3));

            _cache.Remove("userlist");
            _cache.Set("userlist", list, cacheEntryOptions);

            return RedirectToAction("list");
        }

        [LoginRequired]
        public IActionResult List()
        {

            List<AccountModel> list;

            if (!_cache.TryGetValue("userlist", out var x))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
             // Keep in cache for this time, reset time if accessed.
             .SetSlidingExpiration(TimeSpan.FromDays(3));

                list = _account.Quyer().ToList().Select(s => {
                    var m = new AccountModel();
                    m.Id = s.Id;
                    m.Age = s.Age;
                    m.Name = s.Name;
                    m.Remark = s.Remark;
                    return m;
                }).ToList();

                // Save data in cache.
                _cache.Set("userlist", list, cacheEntryOptions);
            }
            else {
                list = _cache.Get<List<AccountModel>>("userlist");
            }

            return View(list);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if(!ModelState.IsValid)
                return View();

            var entity = _account.Quyer().FirstOrDefault(s=>s.LoginName == model.LoginName.Trim());
            if (entity == null || entity.Password != model.Password.Trim())
            {
                ModelState.AddModelError("","用户不存在或密码不对！");
                return View();
            }

            // 登录
            var claims = new Claim[] {
                new Claim("LoginName",entity.LoginName),
                new Claim("UserName",entity.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal user = new ClaimsPrincipal(claimsIdentity);

            //登录用户，相当于ASP.NET中的FormsAuthentication.SetAuthCookie
            HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            user).Wait();

            return RedirectToAction("list");
        }

        public IActionResult LoginOut()
        {
           HttpContext.SignOutAsync().Wait();
            return RedirectToAction("login");
        }
    }
}