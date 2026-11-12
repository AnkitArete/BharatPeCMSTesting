using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace eCafeClient
{
    static class Program
    {
        public static string strCon = "";
        public static SqlConnection con;
        public static SqlCommand cmd;
        public static SqlDataAdapter da;
        public static DataSet ds;
        public static string[] paramName;
        public static string[] paramValue;
        public static string[] paramDirection;
        public static SqlDbType[] paramDbType;
        public static int[] paramDbSize;

        public static string cafeCode = "";
        public static string cafeName = "";
        public static string oprCode = "";
        public static string oprType = "";
        public static string balAvailable = "";
        public static string gStrVersion = "";
        public static string gVersion = "";
        public static string GPN = "";
        public static string balAmtSal = "";
        public static string balAmtSudexo = "";
        public static string balAmtPaytm = "";
        public static string isGuestAllowed = "";
        public static string isSudexoAvailed = "";
        public static string isPaytmAllowed = "";

        public static string _couponType = "";
        public static string _paymentOption = "";

        public static string _Starttime = "";
        public static string _Endtime = "";

        // frmDialog frmd2 = new frmDialog();

        public static string msgResult = "";
        public static string ctgType = "";
        public static string vendorName = "";
        public static string gl_itemName = "";
        public static string empCtg = "";
        public static string cardNo = "";
        public static string cardID = "";
        public static string empID = "";
        public static string cardCat = "";
        public static string cryptKey = "Aret-info-@12345";

        public static string empName = "";//for welcome message
        public static string cardMode = "Proximity";
        public static string itemCtgNameSub = "";
        public static string clientType = "C";//client
        public static string glob_VendorName = "";

        public static string comPort = "";
        public static string version = "";
        public static string machineName = "";
        public static string KioskCode = "";

        public static string vndNameFB = "";
        public static string itemNameFB = "";
        public static string itemCtgNameFB = "";
        public static string empNameFB = "";
        public static string transDateFB = "";

        public static int frmCount;
        public static bool[] frmViewPer;
        public static bool[] frmAddPer;
        public static bool[] frmModPer;
        public static bool[] frmDelPer;

        public static string tssLbl1 = "";
        public static string tssLbl2 = "";
        public static string tssLbl3 = "";
        public static string tssLbl4 = "";

        public static System.Drawing.SizeF PageSize1 = new System.Drawing.SizeF(280, 420);

        public static System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(244, 247, 222);
        public static System.Drawing.Color btnBgColor = System.Drawing.Color.FromArgb(132, 0, 55);

        /////////////////////////
        ///
        public static string HexKey = "7fb70c709a5eed32d37fd5771f09b0fa";
        public static string HexIV = "E1E592E87225847C11D948684F3B070D";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            strCon = checkFile();
            if (strCon == null)
                return;
            Application.Run(new frmLogin());//frmLogin
        }

        public static string checkFile()
        {
            StreamReader sr=null;
            string stn = "";
            string[] stn1 = new string[3];
            string[] temp = new string[3];

            try
            {
                strCon = null;
                string strCon1 = null, strCon2 = null;
                sr = new StreamReader("config.txt");
                strCon1 = sr.ReadLine();
                //strCon1 = cls_Encryption.Decrypt(strCon1, Program.cryptKey);
                strCon = ClsAESEncryptDecrypt.Decrypt(strCon1, ClsAESEncryptDecrypt.HexToByte(HexKey), ClsAESEncryptDecrypt.HexToByte(HexIV));
                string[] strConComp;

                if (strCon1 == null)
                {
                    MessageBox.Show("Configuration File has no details.", "Caf-e-Lite - Error", MessageBoxButtons.OK);
                    return null; ;
                }
                else
                    strCon2 = sr.ReadLine().ToUpper();

                if (strCon2 == null)
                    comPort = "";
                else
                    comPort = strCon2;

                ////    CafeCode
                cafeCode = sr.ReadLine().Trim();
                temp = cafeCode.Split(new string[] { "=" }, StringSplitOptions.None);
                cafeCode = temp[1];

                ////    station code
                stn = sr.ReadLine();
                stn1 = stn.Split(new string[] { "=" }, StringSplitOptions.None);
                KioskCode = stn1[1];

                strConComp=strCon1.Split(new string[] { ";" }, StringSplitOptions.None);

                switch (strConComp.Length)
                {
                    case 3:
                        if (strConComp[0].ToLower().Contains("initial catalog"))
                            if (strConComp[1].ToLower().Contains("data source"))
                                if (strConComp[2].ToLower().Contains("integrated security"))
                                    strCon = strCon1;
                        break;
                    case 4:
                        if (strConComp[0].ToLower().Contains("initial catalog"))
                            if (strConComp[1].ToLower().Contains("data source"))
                                if (strConComp[2].ToLower().Contains("uid"))
                                    if (strConComp[3].ToLower().Contains("pwd"))
                                        strCon = strCon1;
                        break;

                    default:
                        break;

                }
                return strCon;
            }
            catch (FileNotFoundException fe)
            {
                MessageBox.Show("Configuration File is missing.", "Caf-e-Lite - Missing", MessageBoxButtons.OK);
                return strCon;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                return strCon;
            }
        }
        public class Output
        {
            public bool isSuccess = new bool();
            public DataSet ds = new DataSet();
            public string[] OutParam = new string[30];
        }
        public static Output ExecuteProc_value(string ProcName, object[] ParamValue)
        {
            Output outt = new Output();
            checkFile();
            string[] OutParam = new string[15];
            try
            {
                int count = 0;
                int i = 0;


                SqlConnection con = new SqlConnection(strCon);
                SqlCommand cmd = new SqlCommand(ProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1900;
                if (ParamValue != null)
                {
                    SqlParameter[] param = getParameters(ProcName, ParamValue);
                    foreach (var p in param)
                    {
                        cmd.Parameters.Add(p);
                    }

                }
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                //count = cmd.ExecuteNonQuery();
                if (cmd.Parameters != null)
                {
                    i = 0;
                    int t = 0;
                    while (i < cmd.Parameters.Count)
                    {
                        if (cmd.Parameters[i].Direction == ParameterDirection.Output)
                        {
                            cmd.Parameters[i].SqlDbType = SqlDbType.VarChar;
                            OutParam[t] = cmd.Parameters[i].Value.ToString();
                            t++;
                        }
                        i++;
                    }
                }
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                con.Close();
                outt.ds = ds;
                outt.OutParam = OutParam;
                return outt;
            }


            catch (SqlException se)
            {
                throw se;

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public static SqlParameter[] getParameters(string ProcName, object[] ParamValues)
        {
            string qry = @"select O.name [ProcName],P.name [ParamName],parameter_id,T.name [Type],P.max_length [Length],is_output from sys.all_parameters P
                            join sys.all_objects O on P.object_id=O.object_id and O.type='P'
                            join sys.types T on T.system_type_id=P.system_type_id
                            where O.name=@name
                            Order  by parameter_id";
            DataTable dt = ExecuteQuery(qry, new string[] { "@name" }, new object[] { ProcName });
            SqlParameter[] param = new SqlParameter[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (dt.Rows[i]["is_output"].ToString() == "True")
                {
                    param[i] = new SqlParameter(row["ParamName"].ToString(), (SqlDbType)Enum.Parse(typeof(SqlDbType), row["Type"].ToString(), true), Convert.ToInt16(row["Length"].ToString()));
                    param[i].Direction = ParameterDirection.Output;
                    param[i].Value = ParamValues[i];
                }
                else
                {
                    param[i] = new SqlParameter(row["ParamName"].ToString(), ParamValues[i]);
                    param[i].SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), row["Type"].ToString(), true);
                    param[i].Direction = ParameterDirection.Input;
                }

            }
            return param;
        }
        public static DataTable ExecuteQuery(string qry, string[] ParamName, object[] ParamValue)
        {
            DataTable table = new DataTable();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(qry, strCon);
                if (ParamName != null)
                {
                    SqlParameter[] param = getParameters(ParamName, ParamValue, null, null, null);
                    foreach (var p in param)
                    {
                        adapter.SelectCommand.Parameters.Add(p);
                    }

                }
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                // App.Current.MainPage.DisplayAlert("", ex.ToString(), "OK");
                throw ex;
            }
            return table;
        }
        public static SqlParameter[] getParameters(string[] ParamName, object[] ParamValues, string[] ParamDirection, SqlDbType[] ParamDbType, int[] ParamSize)
        {

            SqlParameter[] param = new SqlParameter[ParamName.Length];

            for (int i = 0; i < ParamName.Length; i++)
            {
                if (ParamDirection != null)
                {
                    if (ParamDirection[i] == "O")
                    {
                        param[i] = new SqlParameter(ParamName[i], ParamDbType[i], ParamSize[i]);
                        param[i].Direction = ParameterDirection.Output;
                    }
                    else
                    {
                        param[i] = new SqlParameter(ParamName[i], ParamValues[i]);
                        param[i].SqlDbType = ParamDbType[i];
                        param[i].Direction = ParameterDirection.Input;
                    }
                }
                else
                {
                    param[i] = new SqlParameter(ParamName[i], ParamValues[i]);
                }
            }
            return param;
        }

        public static int executeQuery(string cmdText, bool resultReturn, CommandType cmdType, string[] parameterName, string[] parameterValue, string[] parameterDirection, SqlDbType[] parameterDbType, int[] parameterDbSize)
        {
            try
            {
                //strCon = checkFile();
                int count = 0;
                if (strCon != null || strCon != "")
                {
                    SqlConnection con = new SqlConnection(strCon);
                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.CommandType = cmdType;
                    int i = 0;
                    if (parameterName != null)
                        while (i < parameterName.Length)
                        {
                            if (parameterDirection != null)
                            {
                                if (parameterDirection[i] == "O")
                                    cmd.Parameters.Add(parameterName[i], parameterDbType[i], parameterDbSize[i]);
                                else
                                    cmd.Parameters.AddWithValue(parameterName[i], parameterValue[i]);

                                cmd.Parameters[i].Direction = parameterDirection[i] == "O" ? (ParameterDirection.Output) : (ParameterDirection.Input);
                            }
                            else
                                cmd.Parameters.AddWithValue(parameterName[i], parameterValue[i]);

                            i++;
                        }


                    if (resultReturn)
                    {
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        count = da.Fill(ds);
                    }
                    else
                    {
                        con.Open();
                        count = cmd.ExecuteNonQuery();// 
                        i = 0;
                        if (parameterDirection != null)
                            while (i < parameterName.Length)
                            {
                                if (parameterDirection[i] == "O")
                                {
                                    cmd.Parameters[i].SqlDbType = SqlDbType.VarChar;
                                    paramValue[i] = (string)cmd.Parameters[i].Value;
                                }
                                //paramValue[i] = parameterDirection[i] == "O" ? cmd.Parameters[i].SqlValue.ToString() : "";
                                i++;
                            }
                        con.Close();
                    }
                    return 0;
                }
                return count;
            }
            catch (SqlException sq)
            {
                MessageBox.Show(sq.ToString(), "Error!", MessageBoxButtons.OK);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK);
                return -1;
            }
        }

    }
}