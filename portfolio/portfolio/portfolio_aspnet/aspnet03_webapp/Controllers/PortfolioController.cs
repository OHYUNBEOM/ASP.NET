using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aspnet02_boardapp.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _db;

        //파일업로드 웹환경 객체(필수)
        private readonly IWebHostEnvironment _environment;

        public PortfolioController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var list = _db.Portfolios.ToList();//SELECT *
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // PortfolioModel(X) -> TempPortfolioModel(O)
            return View();
        }

        [HttpPost]
        public IActionResult Create(TempPortfolioModel temp)
        {
            //파일업로드, Temp->Model로 변경, DB 저장
            if(ModelState.IsValid)
            {
                //파일업로드
                string upFileName = UploadImageFile(temp);
                // TempPortfolio Model -> PortfolioModel 변경
                var portfolio = new PortfolioModel()
                {
                    Division = temp.Division,
                    Title = temp.Title,
                    Description = temp.Description,
                    Url = temp.Url,
                    FileName = upFileName//핵심
                };

                _db.Portfolios.Add(portfolio);
                _db.SaveChanges();

                TempData["succeed"] = "포트폴리오 저장 완료!";
                return RedirectToAction("Index","Portfolio");
            }
            return View(temp);
        }

        private string UploadImageFile(TempPortfolioModel temp)
        {
            var resultFileName = "";
            try
            {
                if (temp.PortfolioImage != null)
                {
                    // wwwroot 밑에 uploads라는 폴더 있어야함
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    resultFileName = Guid.NewGuid() + "_" + temp.PortfolioImage.FileName; //중복된 파일명 제거하기 위함
                    string filePath = Path.Combine(uploadFolder, resultFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        temp.PortfolioImage.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);            
            }
            
            return resultFileName;
        }
    }
}