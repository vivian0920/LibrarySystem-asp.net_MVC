using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace LibrarySystem.Models
{
    public class Book
    {
        /// <summary>
        /// 書籍編號
        /// </summary>
        ///[MaxLength(5)]
        [DisplayName("書籍編號")]
        public int book_Id { get; set; }

        /// <summary>
        /// 書名
        /// </summary>
        [DisplayName("*書名")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_Name { get; set; }


        /// <summary>
        /// 作者
        /// </summary>
        [DisplayName("*作者")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_Author { get; set; }

        /// <summary>
        /// 出版商
        /// </summary>
        [DisplayName("*出版商")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_Publisher { get; set; }

        /// <summary>
        /// 內容簡介
        /// </summary>
        [DisplayName("*內容簡介")]
        [Required(ErrorMessage = "此欄位必填")]
        [StringLength(1200, ErrorMessage = "字數超過上限")]
        public string book_Content { get; set; }

        /// <summary>
        /// 購書日期
        /// </summary>
        [DisplayName("*購書日期")]
        [Required(ErrorMessage = "此欄位必填")]
        public DateTime book_BoughtDate { get; set; }


        /// <summary>
        /// 圖書類別
        /// </summary>
        [DisplayName("*圖書類別")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_TypeId { get; set; }

        /// <summary>
        /// 圖書類別
        /// </summary>
        [DisplayName("*圖書類別")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_TypeName { get; set; }


        /// <summary>
        /// 借閱狀態
        /// </summary>
        [DisplayName("*借閱狀態")]
        public string book_Status { get; set; }

        /// <summary>
        /// 借閱狀態名稱
        /// </summary>
        [DisplayName("*借閱狀態")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_Status_Name { get; set; }

        /// <summary>
        /// 借閱人編號
        /// </summary>
        [DisplayName("借閱人")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_Borrower { get; set; }

        /// <summary>
        /// 借閱人
        /// </summary>
        [DisplayName("借閱人")]
        [Required(ErrorMessage = "此欄位必填")]
        public string book_Borrower_Name { get; set; }

        /// <summary>
        /// 借閱日期
        /// </summary>
        [DisplayName("借閱日期")]
        public string book_Lend_date { get; set; }

        /// <summary>
        /// 英文姓名
        /// </summary>
        [DisplayName("英文姓名")]
        public string book_Borrower_Ename { get; set; }

        /// <summary>
        /// 中文姓名
        /// </summary>
        [DisplayName("中文姓名")]
        public string book_Borrower_Cname { get; set; }

    }
}
