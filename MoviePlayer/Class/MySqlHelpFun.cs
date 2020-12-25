using MoviePlayer.Protocol;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePlayer.Class
{
    public class MySqlHelpFun
    {

        public static string sqlcon = "server=192.168.1.191;port=3306;user=SQ8007;password=123456; database=sq_download;";
        public static MySqlConnection conn = null;
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetConn()
        {
            conn = new MySqlConnection(sqlcon);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public static void CloseConn()
        {
            if (conn.State == ConnectionState.Open)   //判断是否打开与数据库的连接
            {
                conn.Close();     //关闭数据库的连接
                conn.Dispose();   //释放My_con变量的所有空间
            }
        }

        /// <summary>
        /// 执行SqlCommand
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        public static void GetSqlCom(string sqlStr)
        {
            try
            {
                GetConn();                  //打开与数据库的连接
                MySqlCommand sqlCom = new MySqlCommand(sqlStr, conn); //创建一个MySqlCommand对象，用于执行SQL语句
                sqlCom.ExecuteNonQuery();   //执行SQL语句            
                sqlCom.Dispose();           //释放所有空间
                CloseConn();                //调用con_close()方法，关闭与数据库的连接
            }
            catch( Exception e)
            {
                Module.WriteLogFile(e.Message);
            }
        }

        public static bool GetSqlRead(string sqlStr)
        {
            try
            {

                GetConn();                                            //打开与数据库的连接
                MySqlCommand sqlCom = new MySqlCommand(sqlStr, conn); //创建一个MySqlCommand对象，用于执行SQL语句
                MySqlDataReader rdr = sqlCom.ExecuteReader();         //执行SQL语句            
                bool isDataExist = rdr.Read();
                sqlCom.Dispose();                                     //释放所有空间
                CloseConn();                                          //调用con_close()方法，关闭与数据库的连接
                return isDataExist;
            }
            catch
            {
                return false;
            }

        }

        public static void SendErr(string ErrData)
        {
            int intNow = Module.ConvertDateTimeInt(DateTime.Now);
            string checkSql = string.Format("select * from hallstatus_table where McuId='{0}';", UdpConnect.uuid);
            if (GetSqlRead(checkSql))
            {
                string sql = string.Format("update hallstatus_table set ErrLogCode='{0}',ErrDesc='{1}',UpdateTime={2},DelFlag={3} where McuId='{4}';", "E S1", ErrData, intNow, 0, UdpConnect.uuid);
                GetSqlCom(sql);
            }
            else
            {
                GetSqlCom("insert into hallstatus_table(McuId,ErrLogCode,ErrDesc,UpdateTime,DelFlag) values" + "('" + UdpConnect.uuid + "','" + "E S1','" + ErrData + "'," + intNow + ",0)");
            }
        }
    }
}

