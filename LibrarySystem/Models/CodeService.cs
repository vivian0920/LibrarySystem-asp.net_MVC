using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibrarySystem.Models
{
    public class CodeService
    {
        /// <summary>
        /// 取得DB連線字串
        /// </summary>
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }

        /// <summary>
        /// 排好變成一個LIST
        /// </summary>
        private List<SelectListItem> MapCodeData_Name(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["CodeName"].ToString(),
                    Value = row["CodeId"].ToString()
                });
            }
            return result;
        }




        /// <summary>
        /// 取得Book_class的圖書類別(下拉式選單)
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetBookClassDropdownlist()
        {
            DataTable dt = new DataTable();
            string sql = @"Select  BOOK_CLASS_ID AS CodeId, BOOK_CLASS_NAME AS CodeName
                           From BOOK_CLASS
                           ORDER BY CodeId ASC";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData_Name(dt);

        }

        public List<SelectListItem> GetBorrowerDropdownlist()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT  DISTINCT 
                                DATA.BOOK_KEEPER AS CodeId ,MEM.USER_CNAME+'-'+MEM.USER_ENAME AS CodeName
                            FROM BOOK_DATA AS DATA
                            INNER JOIN MEMBER_M  AS MEM 
                                  ON DATA.BOOK_KEEPER=MEM.USER_ID
                           ORDER BY CodeId ASC";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData_Name(dt);

        }

        public List<SelectListItem> GetBookStatusDropdownlist()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT  CODE.CODE_ID AS CodeId,CODE.CODE_NAME AS CodeName
                           FROM BOOK_CODE AS CODE
                           WHERE CODE.CODE_TYPE='BOOK_STATUS' 
                           ORDER BY CodeId ASC";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapCodeData_Name(dt);

        }

    }
}