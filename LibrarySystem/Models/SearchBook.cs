using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LibrarySystem.Models
{
    public class SearchBook
    {

        /// <summary>
        /// 
        /// 書籍編號
        /// </summary>
        ///[MaxLength(5)]
        [DisplayName("書籍編號")]
        public string book_Id { get; set; }

        /// <summary>
        /// 書名
        /// </summary>
        [DisplayName("書名")]
        public string book_Name { get; set; }

        /// <summary>
        /// 圖書類別
        /// </summary>
        [DisplayName("圖書類別")]
        public string book_Type { get; set; }

        /// <summary>
        /// 借閱人
        /// </summary>
        [DisplayName("借閱人")]
        public string book_Borrower { get; set; }

        /// <summary>
        /// 借閱狀態
        /// </summary>
        [DisplayName("借閱狀態")]
        public string book_Status { get; set; }

        /// <summary>
        /// 圖書類別名稱
        /// </summary>
        [DisplayName("圖書類別")]
        public string book_TypeName { get; set; }



    }
}