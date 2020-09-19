using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LibrarySystem.Models
{
    public class BookService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        /// <returns></returns>
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }
        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <returns>回傳有幾筆資料受影響，為了驗證使用</returns>
        public int InsertBook(Models.Book book)
        {
            string sql =@"INSERT INTO BOOK_DATA
                                 (
                                        BOOK_NAME,BOOK_AUTHOR,BOOK_PUBLISHER,BOOK_NOTE,BOOK_BOUGHT_DATE,
                                        BOOK_CLASS_ID,BOOK_STATUS,BOOK_KEEPER
                                 )
                                 VALUES
                                 (
                                        @name,@author,@publisher,@content,@boughtDate,
                                        @typeId,@status,@keeper
                                  )";

            int numberOfData;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@name", book.book_Name));
                cmd.Parameters.Add(new SqlParameter("@author", book.book_Author == null ? string.Empty : book.book_Author));
                cmd.Parameters.Add(new SqlParameter("@publisher", book.book_Publisher == null ? string.Empty : book.book_Publisher));
                cmd.Parameters.Add(new SqlParameter("@content", book.book_Content == null ? string.Empty : book.book_Content));
                cmd.Parameters.Add(new SqlParameter("@boughtDate", book.book_BoughtDate));
                cmd.Parameters.Add(new SqlParameter("@typeId", book.book_TypeId));
                cmd.Parameters.Add(new SqlParameter("@status", "A"));
                cmd.Parameters.Add(new SqlParameter("@keeper", ""));
                //從Conn物件啟用transaction
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                //把transaction指給command物件
                cmd.Transaction = sqlTransaction;
                try
                {
                     numberOfData = cmd.ExecuteNonQuery();
                    //成功Commit
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    //若失敗則恢復
                    sqlTransaction.Rollback();
                    throw;
                }
                finally
                {
                    //最後要把連線關閉
                    conn.Close();
                }
                
            }
            return numberOfData;
        }


        /// <summary>
        /// 根據資料查找書籍
        /// </summary>
        public List<Models.Book> GetBookByData(Models.SearchBook searchBook)
        {

            DataTable dt = new DataTable();
            string sql = @"SELECT 
                                CLASS.BOOK_CLASS_NAME as TypeName,
                                DATA.BOOK_NAME as Name, 
                                CONVERT(VARCHAR(10),DATA.BOOK_BOUGHT_DATE,111)AS BoughtDate,
                                DATA.BOOK_STATUS as Status,
                                CODE.CODE_NAME as StatusName,
                                (MEM.USER_CNAME+'-'+MEM.USER_ENAME)AS Borrower ,
                                data.BOOK_ID AS  Id                                
                          FROM  BOOK_DATA AS data 
                          INNER  JOIN BOOK_CLASS AS CLASS 
                                ON DATA.BOOK_CLASS_ID=CLASS.BOOK_CLASS_ID
                          LEFT  JOIN MEMBER_M AS MEM 
                                ON DATA.BOOK_KEEPER=MEM.USER_ID
                          INNER JOIN BOOK_CODE AS CODE
                                ON DATA.BOOK_STATUS=CODE.CODE_ID AND CODE.CODE_TYPE='BOOK_STATUS'
                          WHERE  
                                (DATA.BOOK_CLASS_ID=@classID OR @classID='') AND
                                (UPPER (DATA.BOOK_NAME) LIKE UPPER('%'+@name+'%') or @name='') AND
                                (DATA.BOOK_KEEPER=@borrower OR @borrower='') AND
                                (DATA.BOOK_STATUS = @status OR @status='') 
                          ORDER BY BoughtDate DESC";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@classID", searchBook.book_TypeName == null ? string.Empty : searchBook.book_TypeName));
                cmd.Parameters.Add(new SqlParameter("@name", searchBook.book_Name == null ? string.Empty : searchBook.book_Name));
                cmd.Parameters.Add(new SqlParameter("@borrower", searchBook.book_Borrower == null ? string.Empty : searchBook.book_Borrower));
                cmd.Parameters.Add(new SqlParameter("@status", searchBook.book_Status == null ? string.Empty : searchBook.book_Status));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();

            }

            return this.MapBookByData(dt);
        }

        /// <summary>
        /// 根據Id查找書籍
        /// </summary>
        public Models.Book GetBookById(string book_Id)
        {

            DataTable dt = new DataTable();
            string sql = @"SELECT 
                                  CLASS.BOOK_CLASS_NAME as  TypeName,
                                  DATA.BOOK_CLASS_ID as TypeID,
                                  DATA.BOOK_NAME as Name,
                                  CONVERT(VARCHAR(10),DATA.BOOK_BOUGHT_DATE,111)AS BoughtDate,
                                  DATA.BOOK_STATUS as Status,
                                  CODE.CODE_NAME as Status_Name,
                                  MEM.USER_ID AS Borrower,
                                  (MEM.USER_CNAME+'-'+MEM.USER_ENAME)AS Borrower_Name,
                                  DATA.BOOK_ID AS Id,
                                  DATA.BOOK_AUTHOR as Author,
                                  DATA.BOOK_PUBLISHER as Publisher,
                                  DATA.BOOK_NOTE as Content
                          FROM    BOOK_DATA AS DATA
                          INNER  JOIN BOOK_CLASS AS CLASS 
                                ON DATA.BOOK_CLASS_ID=CLASS.BOOK_CLASS_ID                         
                          LEFT  JOIN MEMBER_M AS MEM 
                                ON DATA.BOOK_KEEPER=MEM.USER_ID
                          INNER JOIN BOOK_CODE AS CODE 
                                ON DATA.BOOK_STATUS=CODE.CODE_ID AND CODE.CODE_TYPE='BOOK_STATUS' 
                          WHERE  DATA.BOOK_ID=@id";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@id", book_Id));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();

            }

            return this.MapBookModelById(dt);
        }

        /// <summary>
        /// 根據ID修改書籍資訊
        /// </summary>
        /// <returns>回傳有幾筆資料受影響，為了驗證使用</returns>
        public int UpdateBook(Models.Book book)
        {
            string sql = @"UPDATE BOOK_DATA     
                           SET BOOK_NAME=@name,BOOK_AUTHOR=@author,BOOK_PUBLISHER=@publisher,
                               BOOK_NOTE=@note,BOOK_BOUGHT_DATE=@boughtDate,BOOK_CLASS_ID=@classId,
                               BOOK_STATUS=@status,BOOK_KEEPER=@keeper 
                           where BOOK_ID=@id";
            
            int numberOfData;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@id", book.book_Id));
                cmd.Parameters.Add(new SqlParameter("@name", book.book_Name == null ? string.Empty : book.book_Name));
                cmd.Parameters.Add(new SqlParameter("@author", book.book_Author == null ? string.Empty : book.book_Author));
                cmd.Parameters.Add(new SqlParameter("@publisher", book.book_Publisher == null ? string.Empty : book.book_Publisher));
                cmd.Parameters.Add(new SqlParameter("@note", book.book_Content == null ? string.Empty : book.book_Content));
                cmd.Parameters.Add(new SqlParameter("@boughtDate", book.book_BoughtDate));
                cmd.Parameters.Add(new SqlParameter("@classId", book.book_TypeId == null ? string.Empty : book.book_TypeId));
                cmd.Parameters.Add(new SqlParameter("@status", book.book_Status == null ? string.Empty : book.book_Status));
                cmd.Parameters.Add(new SqlParameter("@keeper", book.book_Borrower == null ? string.Empty : book.book_Borrower));
                //從Conn物件啟用transaction
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                //把transaction指給command物件
                cmd.Transaction = sqlTransaction;
                try
                {
                    numberOfData=cmd.ExecuteNonQuery();
                    //成功Commit
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    //若失敗則恢復
                    sqlTransaction.Rollback();
                    throw;
                }
                finally
                {
                    //最後要把連線關閉
                    conn.Close();
                }
            }return numberOfData;
        }
        /// <summary>
        /// 新增借閱紀錄
        /// </summary>
        /// <returns>回傳有幾筆資料受影響，為了驗證使用</returns>
        public int InsertBookIntoLendRecord(Models.Book book)
        {
            string sql = @"INSERT INTO BOOK_LEND_RECORD
                                 (
                                        BOOK_ID,KEEPER_ID,LEND_DATE
                                 )
                                 VALUES
                                 (
                                        @bookId,@keeperId,@lendDate
                                  )";

            int numberOfData;
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@bookId", book.book_Id));
                cmd.Parameters.Add(new SqlParameter("@keeperId", book.book_Borrower == null ? string.Empty : book.book_Borrower));
                cmd.Parameters.Add(new SqlParameter("@lendDate", DateTime.Now));
            
                //從Conn物件啟用transaction
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                //把transaction指給command物件
                cmd.Transaction = sqlTransaction;
                try
                {
                    numberOfData = cmd.ExecuteNonQuery();
                    //成功Commit
                    sqlTransaction.Commit();
                }
                catch (Exception)
                {
                    //若失敗則恢復
                    sqlTransaction.Rollback();
                    throw;
                }
                finally
                {
                    //最後要把連線關閉
                    conn.Close();
                }

            }
            return numberOfData;
        }
        /// <summary>
        /// 將書籍資料變成一個Mmodel(id查找書籍)
        /// </summary>
        public Models.Book MapBookModelById(DataTable dt)
        {
            Models.Book result = new Models.Book();
            foreach (DataRow row in dt.Rows)
            {
                result.book_Name = row["Name"].ToString();
                result.book_BoughtDate = DateTime.Parse(row["BoughtDate"].ToString());
                result.book_TypeId = row["TypeID"].ToString();
                result.book_TypeName = row["TypeName"].ToString();
                result.book_Status_Name = row["Status_Name"].ToString();
                result.book_Status = row["Status"].ToString();
                result.book_Borrower = row["Borrower"].ToString();
                result.book_Borrower_Name = row["Borrower_Name"].ToString();
                result.book_Id = (int)row["Id"];
                result.book_Author = row["Author"].ToString();
                result.book_Publisher = row["Publisher"].ToString();
                result.book_Content = row["Content"].ToString();
                result.book_BoughtDate = DateTime.Parse(row["BoughtDate"].ToString());

            }
            return result;
        }

        /// <summary>
        /// 將書籍資料變成一個list(資料查找書籍)
        /// </summary>
        public List<Models.Book> MapBookByData(DataTable dt)
        {
            List<Models.Book> result = new List<Models.Book>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new Book()
                {
                    book_Name = row["Name"].ToString(),
                    book_BoughtDate = DateTime.Parse(row["BoughtDate"].ToString()),
                    book_TypeName = row["TypeName"].ToString(),
                    book_Status_Name = row["StatusName"].ToString(),
                    book_Status = row["Status"].ToString(),
                    book_Borrower_Name = row["Borrower"].ToString(),
                    book_Id = (int)row["Id"]
                });
            }
            return result;
        }


        /// <summary>
        /// 根據id刪除書籍
        /// </summary>
        public void DeleteBook(string book_Id)
        {
            try
            {
                string sql = "Delete FROM BOOK_DATA Where BOOK_ID=@id";
                using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@id", book_Id));
                    //從Conn物件啟用transaction
                    SqlTransaction sqlTransaction = conn.BeginTransaction();
                    //把transaction指給command物件
                    cmd.Transaction = sqlTransaction;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        //成功Commit
                        sqlTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        //若失敗則恢復
                        sqlTransaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        //最後要把連線關閉
                        conn.Close();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根據Id查找借閱紀錄
        /// </summary>
        public List<Models.Book> GetLendRecordById(string book_Id)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
                                CONVERT(VARCHAR(10),lend.LEND_DATE,111)AS lend_date,
                                lend.KEEPER_ID AS keeper_ID,
                                mem.USER_CNAME as 'Cname',mem.USER_ENAME as Ename
                           FROM BOOK_LEND_RECORD lend
                           INNER JOIN MEMBER_M mem 
                                ON lend.KEEPER_ID=mem.USER_ID
                           WHERE  lend.BOOK_ID=@id";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@id", book_Id));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return MapBookRecordList(dt);
        }
        /// <summary>
        /// 將得到的借閱資料變成一個list
        /// </summary>
        public List<Models.Book> MapBookRecordList(DataTable dt)
        {
            List<Models.Book> result = new List<Models.Book>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new Book()
                {
                    book_Lend_date = row["lend_date"].ToString(),
                    book_Borrower = row["keeper_ID"].ToString(),
                    book_Borrower_Ename = row["Ename"].ToString(),
                    book_Borrower_Cname = row["Cname"].ToString(),
                });
            }
            return result;
        }

    }
}