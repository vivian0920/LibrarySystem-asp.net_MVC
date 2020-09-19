using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibrarySystem.Controllers
{
    public class BookController : Controller
    {
        Models.CodeService codeService = new Models.CodeService();

        /// <summary>
        /// 新增書籍畫面
        /// </summary>
        [HttpGet()]
        public ActionResult InsertBook()
        {
            ViewBag.bookClassData = this.codeService.GetBookClassDropdownlist();
            return View(new Models.Book());
        }

        /// <summary>
        /// 新增書籍畫面
        /// </summary>
        [HttpPost()]
        public ActionResult InsertBook(Models.Book book)
        {
            ViewBag.bookClassData = this.codeService.GetBookClassDropdownlist();
            Models.BookService library = new Models.BookService();
            if (library.InsertBook(book) == 1)
            {
                //成功新增一筆資料
                ModelState.Clear();
                return RedirectToAction("InsertBook");
            }
            else {
                //新增一筆資料失敗
                TempData["insertSuccessfully"] = "系統出現問題";
            }
            return View(book);
        }

        /// <summary>
        /// 書籍資料查詢
        /// </summary>
        [HttpGet()]
        public ActionResult SearchBook()
        {
            ViewBag.bookClassData = this.codeService.GetBookClassDropdownlist();
            ViewBag.borrowerData = this.codeService.GetBorrowerDropdownlist();
            ViewBag.statusData = this.codeService.GetBookStatusDropdownlist();
            return View();
        }

        /// <summary>
        /// 書籍資料查詢
        /// </summary>
        [HttpPost()]
        public ActionResult SearchBook(Models.SearchBook book)
        {
            Models.BookService library = new Models.BookService();
            ViewBag.bookClassData = this.codeService.GetBookClassDropdownlist();
            ViewBag.borrowerData = this.codeService.GetBorrowerDropdownlist();
            ViewBag.statusData = this.codeService.GetBookStatusDropdownlist();
            ViewBag.searchResult = library.GetBookByData(book);
            return View();
        }

        /// <summary>
        /// 刪除書籍
        /// </summary>
        [HttpPost()]
        public JsonResult DeleteBook(string book_Id)
        {
            try
            {
                Models.BookService library = new Models.BookService();
                library.DeleteBook(book_Id);
                return this.Json(true);
            }

            catch (Exception ex)
            {
                return this.Json(false);
            }
        }



        /// <summary>
        /// 書籍資料修改
        /// </summary>
        [HttpPost()]
        public ActionResult UpdateBook(Models.Book book)
        {
            Models.BookService library = new Models.BookService();
            ViewBag.bookClassData = this.codeService.GetBookClassDropdownlist();
            ViewBag.borrowerData = this.codeService.GetBorrowerDropdownlist();
            ViewBag.statusData = this.codeService.GetBookStatusDropdownlist();
            if (library.UpdateBook(book) == 1 && (book.book_Status == "B" || book.book_Status == "C"))
            { 
                //成功更新一筆資料且為借出狀態，則新增一筆借閱紀錄
                if (library.InsertBookIntoLendRecord(book) == 1)
                {
                    ModelState.Clear();
                }
                else {
                    TempData["updateSuccessfully"] = "系統出現問題";
                }
            }
            else if(library.UpdateBook(book) == 1)
            {   //更新書籍內容，並未更新借閱狀態
                ModelState.Clear();
            }
            else
            {
                //更新一筆資料失敗
                TempData["updateSuccessfully"] = "系統出現問題";
            }


            return View(book);
        }

        /// <summary>
        /// 書籍資料修改
        /// </summary>
        [HttpGet()]
        public ActionResult UpdateBook(string book_Id)
        {
            Models.BookService library = new Models.BookService();
            ViewBag.bookClassData = this.codeService.GetBookClassDropdownlist();
            ViewBag.borrowerData = this.codeService.GetBorrowerDropdownlist();
            ViewBag.statusData = this.codeService.GetBookStatusDropdownlist();
            Models.Book book = library.GetBookById(book_Id);
            return View(book);
        }

        /// <summary>
        /// 明細畫面
        /// </summary>
        /// <param name="book_Id">find the book information according to book id</param>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult ShowBookInfo(string book_Id)
        {
            Models.BookService library = new Models.BookService();
            Models.Book book = library.GetBookById(book_Id);
            return View(book);
        }


        /// <summary>
        /// 借閱紀錄
        /// </summary>
        /// <param name="book_Id">find the lendings records according to book id</param>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult LendRecord(string book_Id)
        {
            Models.BookService library = new Models.BookService();
            ViewBag.recordData = library.GetLendRecordById(book_Id);
            return View();
        }


    }
}