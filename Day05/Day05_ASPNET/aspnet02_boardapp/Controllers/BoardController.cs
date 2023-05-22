using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace aspnet02_boardapp.Controllers
{
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BoardController(ApplicationDbContext db)
        {
            _db = db; // DB 연결
        }

        public IActionResult Index()//게시판 내용 뿌리기
        {
            IEnumerable<Board> objBoardList = _db.Boards.ToList();//SELECT 역할
            return View(objBoardList);
        }
        //GetMethod로 화면 부를 때의 액션
        [HttpGet]
        public IActionResult Create()//게시판 글쓰기
        {
            return View();
        }
        //Submit 발생 후의 액션
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Board board) 
        {
            board.UserId = "auto-gen";//임의 생성
            _db.Boards.Add(board);//INSERT
            _db.SaveChanges();//COMMIT
            return RedirectToAction("Index","Board");
        }
    }
}
