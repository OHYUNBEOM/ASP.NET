using aspnet02_boardapp.Data;
using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace aspnet02_boardapp.Controllers
{
    public class BoardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BoardController(ApplicationDbContext db)
        {
            _db = db; // DB 연결
        }

        public IActionResult Index(int page=1)//게시판 내용 뿌리기
        {
            //IEnumerable<Board> objBoardList = _db.Boards.ToList();//SELECT 역할
            //var objBoardList = _db.Boards.FromSql($"SELECT * FROM boards").ToList(); // SQL로도 가능
            var totalCount = _db.Boards.Count();
            var pageSize = 10;//한 페이지에 게시글 10개씩
            var totalPage = totalCount / pageSize;
            if (totalCount % pageSize > 0) totalPage++; //나머지가 있으면 2페이지 이상이란거임
            //if(totalPage<page) page=totalPage;

            var countPage = 10;
            var startPage = ((page - 1) / countPage) * countPage + 1;
            var endPage = startPage + countPage - 1;
            if(totalPage<endPage) endPage=totalPage;

            int startCount = ((page - 1) * countPage) + 1;
            int endCount = startCount + (pageSize-1);

            // HTML 화면에서 사용하기 위해 선언 -> ViewData,TempData와 동일한 역할
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.StartCount=startCount; //게시판 번호를 위해 추가

            var StartCount = new MySqlParameter("StartCount", startCount);
            var EndCount = new MySqlParameter("EndCount", endCount);

            var objBoardList = _db.Boards.FromSql($"CALL New_PagingBoard({StartCount},{EndCount})").ToList();

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
            board.PostDate = DateTime.Now;//현재 일자 할당
            _db.Boards.Add(board);//INSERT
            _db.SaveChanges();//COMMIT

            TempData["succeed"] = "새 게시글이 저장되었습니다.";

            return RedirectToAction("Index","Board"); //localhost/Board/Index로 페이지 이동
        }
        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if(Id==null || Id==0)
            {
                return NotFound();
            }
            var board = _db.Boards.Find(Id); //SELECT * FROM WHERE Id=@id

            if(board==null)
            {
                return NotFound();
            }
            return View(board);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Board board)
        {
            board.PostDate = DateTime.Now;//날짜 다시 할당

            _db.Boards.Update(board); // UPDATE 쿼리문 실행
            _db.SaveChanges(); // COMMIT

            TempData["succeed"] = "게시글이 수정되었습니다.";

            return RedirectToAction("Index", "Board");
        }
        [HttpGet]
        public IActionResult Delete(int? Id) 
        {// Edit 로직과 완전 동일
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var board = _db.Boards.Find(Id); //SELECT * FROM WHERE Id=@id

            if (board == null)
            {
                return NotFound();
            }
            return View(board);
        }
        [HttpPost]
        public IActionResult DeletePost(int? Id)
        {
            var board = _db.Boards.Find(Id);
            if(board == null)
            {
                return NotFound();
            }
            _db.Boards.Remove(board); // DELETE 쿼리 실행
            _db.SaveChanges(); // COMMIT

            TempData["succeed"] = "게시글을 삭제했습니다.";

            return RedirectToAction("Index", "Board");
        }
        [HttpGet]
        public IActionResult Details(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var board = _db.Boards.Find(Id); //SELECT * FROM WHERE Id=@id

            if (board == null)
            {
                return NotFound();
            }

            board.ReadCount++;//조회수를 1 증가
            _db.Boards.Update(board);//UPDATE 
            _db.SaveChanges();//COMMIT

            return View(board);
        }
    }
}
