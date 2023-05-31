using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aspnet02_boardapp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["NoScroll"] = "true";// 게시판은 메인스크롤 안생김
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            ModelState.Remove("PhoneNumber");
            if(ModelState.IsValid)
            {
                //ASP.NET user - aspnetusers 테이블에 데이터 넣기
                var user = new IdentityUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber=model.PhoneNumber //핸드폰 번호 추가
                };
                //aspnetusers 테이블에 사용자 데이터 삽입
                var result = await _userManager.CreateAsync(user,model.Password);

                if(result.Succeeded)
                {
                    //회원가입 성공 시 로그인 후 localhost:7125/Home/Index
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["succeed"] = "회원가입에 성공했습니다.";//성공 메시지
                    return RedirectToAction("Index","Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model); // 회원가입 실패하면 그대로
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["NoScroll"] = "true";// 게시판은 메인스크롤 안생김
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                //파라미터 순서 : 아이디,패스워드,IsPersistent=RememberMe, 실패 시 계쩡 잠금
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if(result.Succeeded)
                {
                    TempData["succeed"] = "로그인 성공.";//성공 메시지
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "로그인 실패");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["succeed"] = "로그아웃 성공.";//성공 메시지
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string userName)
        {
            ViewData["NoScroll"] = "true";// 게시판은 메인스크롤 안생김
            Debug.WriteLine(userName);
            var curUser = await _userManager.FindByNameAsync(userName);
            if(curUser == null)
            {
                TempData["error"] = "사용자가 없습니다.";
                return RedirectToAction("Index", "Home");
            }
            var model = new RegisterModel()
            {
                Email = curUser.Email,
                PhoneNumber = curUser.PhoneNumber,
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                user.PhoneNumber=model.PhoneNumber;
                user.Email=model.Email;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                var result = await _userManager.UpdateAsync(user); 

                if (result.Succeeded)
                {
                    //회원가입 성공 시 로그인 후 localhost:7125/Home/Index
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["succeed"] = "프로필변경에 성공했습니다.";//성공 메시지
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model); // 프로필변경 실패하면 화면유지
        }
    }
}
