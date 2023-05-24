using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aspnet02_boardapp.Models
{
    //게시판을 위한 테이블 매핑 정보
    public class Board
    {
        [Key] // PK
        public int Id { get; set; }
        [Required(ErrorMessage ="아이디를 입력하세요.")] // Not Null
        [DisplayName("아이디")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "이름을 입력하세요.")] // Not Null
        [DisplayName("이름")]
        public string Name { get; set; }
        [Required(ErrorMessage = "제목을 입력하세요.")] // Not Null
        [MaxLength(200)] // varchar(200)
        [DisplayName("제목")]
        public string Title { get; set; }
        [DisplayName("조회수")]
        public int ReadCount { get; set; }
        [DisplayName("작성일")]
        public DateTime PostDate { get; set; } = DateTime.Now;
        [DisplayName("작성글")]
        public string Contents { get; set; }
    }
}
