using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eCafeClient
{
    public partial class frmWelcome : Form
    {
        Program.Output ott = new Program.Output();
        string ShowDialogueStatus = "NI";
        string checkAlacarteMenuVal = "";
        string checkBuffetMenuVal = "";
        string formOpenType = "";
        string[] VendorList = new string[10];
        Button[] btnItemArray;
        int TotalVendor = 0;
        int btnVndWidth = 0;
        int btnVndHeight = 0;
        int btnVndGap = 5;
        Point[] ptV = new Point[6];
        string selVndName = "";
        Button[] btnVendor;
        Point[] ptV2 = new Point[6];
        Color _btnBackColor = Color.FromArgb(163, 41, 118);
        int autoExit = -1;
        public frmWelcome()
        {
            InitializeComponent();
            this.btn0.Click += new System.EventHandler(this.btn_Click);
            this.btn1.Click += new System.EventHandler(this.btn_Click);
            this.btn2.Click += new System.EventHandler(this.btn_Click);
            this.btn3.Click += new System.EventHandler(this.btn_Click);
            this.btn4.Click += new System.EventHandler(this.btn_Click);
            this.btn5.Click += new System.EventHandler(this.btn_Click);
            this.btn6.Click += new System.EventHandler(this.btn_Click);
            this.btn7.Click += new System.EventHandler(this.btn_Click);
            this.btn8.Click += new System.EventHandler(this.btn_Click);
            this.btn9.Click += new System.EventHandler(this.btn_Click);
            this.btnClear.Click += new System.EventHandler(this.btn_Click);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string strTarget = btn.Text;
            switch (strTarget)
            {
                case "CAPS":
                    break;

                case "CLEAR":
                    txtPwd.Text = "";
                    break;

                case "0":
                    if (txtPwd.Text != "")
                        txtPwd.Text = txtPwd.Text + "0";
                    break;

                default:
                    txtPwd.Text = txtPwd.Text + strTarget;
                    break;
            }

        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }
        public void clear()
        {
            lblVendorName.Text = "";
            lblName.Text = "";
            txtCardNo.Text = "";
            txtEmpID.Text = "";
            txtPwd.Text = "";
            txt_cardtype.Text = "";
        }
        public void gettime()
        {
            try
            {
                int i = Program.executeQuery(string.Format("SELECT CONVERT(VARCHAR(20),convert(time,startFrom),100),CONVERT(VARCHAR(20),convert(time,startTo),100) from itemCtgMaster"), true, CommandType.Text, null, null, null, null, null);
                if (i == -1)
                    return;
                if (Program.ds.Tables[0] != null && Program.ds.Tables[0].Rows.Count > 0)
                {
                    Program._Starttime = Program.ds.Tables[0].Rows[0][0].ToString();
                    Program._Endtime = Program.ds.Tables[0].Rows[0][1].ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void frmWelcome_Load(object sender, EventArgs e)
        {
            clear();
            gettime();
            lbl_Welcomemsg.Text = "";
            tb_All.SelectedTab = tb_Welcome;
            lblCafeName.Text = Program.cafeName;
            txtcarduid.Focus();
            gpExit.Visible = false;
            this.Text = "Welcome [" + Program.gVersion + "]";
            DynamicMsg();
            getvendor();
            GetMealCount();
        }
        /// <summary>
        /// //////////////////////////////////////  Welcome Form  ////////////////////////////////////////////
        /// </summary>
        public void DynamicMsg()
        {
            string cmdText = @"select msg1,msg2 from msgClt where cafeCode='"+ Program.cafeCode + "'";
            Program.paramName = new string[] { "@cafeCode" };
            Program.paramValue = new string[] { Program.cafeCode };
            Program.paramDirection = new string[] { "I" };
            Program.paramDbType = new SqlDbType[] { SqlDbType.VarChar };
            Program.paramDbSize = new int[] { 0 };

            int i = Program.executeQuery(cmdText, true, CommandType.Text, Program.paramName, Program.paramValue, Program.paramDirection, Program.paramDbType, Program.paramDbSize);
            if (i == -1)
                return;
            if (Program.ds.Tables[0].Rows.Count > 0)
            {
                lbl_DynamicMsg.Text = Program.ds.Tables[0].Rows[0]["msg1"].ToString();
                lbl_DynamicMsg2.Text = Program.ds.Tables[0].Rows[0]["msg2"].ToString();
            }
            lbl_DynamicMsg2.TextAlign = ContentAlignment.MiddleCenter;
            lbl_DynamicMsg.TextAlign = ContentAlignment.MiddleCenter;
            lbl_DynamicMsg.Location = new Point((this.Width / 2) - 200, (this.Height / 2) - (lbl_DynamicMsg.Height / 2));
            lbl_DynamicMsg2.Location = new Point((this.Width / 2) - 200, (this.Height / 2) - (lbl_DynamicMsg2.Height / 2) + lbl_DynamicMsg.Height + 20);
        }
        public void getvendor()
        {
            try
            {
                string cmdText = @"select vendorName from vendorMaster where CafeCode= @CafeCode";
                Program.paramName = new string[] { "@CafeCode" };
                Program.paramValue = new string[] {  Program.cafeCode };
                Program.paramDirection = new string[] { "I" };
                Program.paramDbType = new SqlDbType[] { SqlDbType.VarChar };
                Program.paramDbSize = new int[] { 0 };

                int i = Program.executeQuery(cmdText, true, CommandType.Text, Program.paramName, Program.paramValue, Program.paramDirection, Program.paramDbType, Program.paramDbSize);
                if (i == -1)
                    return;
                if (Program.ds.Tables[0].Rows.Count > 0)
                {
                    Program.vendorName= lbl_vendorName.Text = Program.ds.Tables[0].Rows[0]["vendorName"].ToString();
                    getctg();
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.ToString(), "Error", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }
        public void getctg()
        {
            string cmdText = @"select itemCtgName from itemCtgMaster where vendorName=@vendorName and lower(ctgType)='fixed' and status='1' and cafeCode=@cafeCode and 
                ((datepart(hour,startFrom)<datepart(hour,getdate()) and datepart(hour,startTo)>datepart(hour,getdate()))
                or
                (datepart(hour,startFrom)=datepart(hour,getdate()) and datepart(hour,startTo)=datepart(hour,getdate()) and 
                datepart(minute,startFrom)<datepart(minute,getdate()) and datepart(minute,startTo)>datepart(minute,getdate()))
                or
                (datepart(hour,startFrom)=datepart(hour,getdate()) and datepart(hour,startTo)>datepart(hour,getdate()) and 
                datepart(minute,startFrom)<=datepart(minute,getdate())) 
                or
                (datepart(hour,startFrom)<datepart(hour,getdate()) and datepart(hour,startTo)=datepart(hour,getdate()) and 
                datepart(minute,startTo)>=datepart(minute,getdate()))
                 )";
            Program.paramName = new string[] { "@vendorName", "@cafeCode" };
            Program.paramValue = new string[] { Program.vendorName,Program.cafeCode };
            Program.paramDirection = new string[] { "I", "I" };
            Program.paramDbType = new SqlDbType[] { SqlDbType.VarChar, SqlDbType.VarChar };
            Program.paramDbSize = new int[] { 0, 0 };

            int i = Program.executeQuery(cmdText, true, CommandType.Text, Program.paramName, Program.paramValue, Program.paramDirection, Program.paramDbType, Program.paramDbSize);
            if (i == -1)
                return;
            if(Program.ds.Tables[0].Rows.Count > 0)
            {
                lbl_itemCtg.Text = Program.ds.Tables[0].Rows[0]["itemCtgName"].ToString();
            }
        }
        private void GetMealCount()
        {
            string cmdText = @"select count(1) [Count] from transDetail where CONVERT(varchar(10),transdate,120)= CONVERT(varchar(10),getdate(),120) and itemCtgName= @itmCtg;select top(1)empName from transDetail where CONVERT(varchar(10),transdate,120)= CONVERT(varchar(10),getdate(),120) and itemCtgName= @itmCtg order by transDate desc";
            Program.paramName = new string[] { "@itmCtg" };
            Program.paramValue = new string[] { lbl_itemCtg.Text };
            Program.paramDirection = new string[] { "I" };
            Program.paramDbType = new SqlDbType[] { SqlDbType.VarChar };
            Program.paramDbSize = new int[] { 0 };
            int i = Program.executeQuery(cmdText, true, CommandType.Text, Program.paramName, Program.paramValue, Program.paramDirection, Program.paramDbType, Program.paramDbSize);
            if (i == -1)
                return;
            if (Program.ds.Tables[0] != null && Program.ds.Tables[0].Rows.Count > 0)
            {
                lblSwapeCount.Text = Program.ds.Tables[0].Rows[0]["Count"].ToString();
            }
            if (Program.ds.Tables[1] != null && Program.ds.Tables[1].Rows.Count > 0)
            {
                lbl_lastSwipe.Text = "Last swiped by :- " + Program.ds.Tables[1].Rows[0][0].ToString();
                lbl_lastSwipe.Location = new Point(
                pnl_LastSwipe.ClientSize.Width / 2 - lbl_lastSwipe.Size.Width / 2,
                (pnl_LastSwipe.ClientSize.Height / 2) - (lbl_lastSwipe.Size.Height / 2));
                lbl_lastSwipe.Anchor = AnchorStyles.None;
            }
        }
        private void lblSwapeCount_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            LinearGradientBrush lgb = new LinearGradientBrush(lblSwapeCount.ClientRectangle, Color.Transparent, Color.FromArgb(200, Color.Black), LinearGradientMode.Vertical);
            e.Graphics.FillEllipse(lgb, lblSwapeCount.DisplayRectangle);
        }
        private void txtcarduid_TextChanged(object sender, EventArgs e)
        {
            tmrcarduid.Enabled = true;
        }
        private void tmrcarduid_Tick(object sender, EventArgs e)
        {
            tmrcarduid.Enabled = false;
            if (txtcarduid.Text != "")
            {
                checkCardNew(txtcarduid.Text);
            }
        }
        private void checkCardNew(string carduid)
        {
            Program.cardID = carduid;
            txtcarduid.Text = "";
            try
            {
                txtcarduid.Text = "";
                ott = Program.ExecuteProc_value("udp_cardStatus", new object[] { Program.cardID, Program.cafeCode, "", "",  "", "", "", "", "", "", "", "", "", "", "" });
                if (ott.OutParam[0].ToString() == "OK")
                {
                    Program.empName = ott.OutParam[1];
                    Program.GPN = ott.OutParam[2];
                    Program.empCtg = ott.OutParam[3];
                    Program.cardCat = ott.OutParam[4];
                    Program.empID = ott.OutParam[5];
                    Program.balAmtSal = ott.OutParam[6];
                    Program.cardNo = ott.OutParam[8];
                    Program.isGuestAllowed = ott.OutParam[9];
                    if(Program.isGuestAllowed.ToLower()=="true")
                    {
                        Program.isGuestAllowed = "1";
                    }
                    else
                    {
                        Program.isGuestAllowed = "0";
                    }
                    checkSodexoGuest();
                    mealTypeStatus();
                }
                else
                {
                    lbl_Welcomemsg.Text = ott.OutParam[0].ToString();
                    setting();
                    tmrMsgclear.Enabled = true;
                }
            }
            catch (Exception se)
            {
                
            }
        }
        public void setting()
        {
            lbl_Welcomemsg.Location = new Point(
               pnl_header.ClientSize.Width / 2 - lbl_Welcomemsg.Size.Width / 2,
               pnl_header.ClientSize.Height / 2 - lbl_Welcomemsg.Size.Height / 2);
            lbl_Welcomemsg.Anchor = AnchorStyles.None;
            gpExit.Location = new Point(((Screen.PrimaryScreen.Bounds.Width - gpExit.Width) / 2), (Screen.PrimaryScreen.Bounds.Height - gpExit.Height) / 2);
        }
        private void tmrMsgclear_Tick(object sender, EventArgs e)
        {
            tmrMsgclear.Enabled = false;
            lbl_Welcomemsg.Text = "";
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            gpExit.Visible = true;
            txtPwd.Focus();
            setting();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPwd.Text = "";
            txtcarduid.Focus();
            gpExit.Visible = false;
            Program.cardID = "";
        }
        private void btnOK_Click(object sender, EventArgs e)

        {
            try
            {
                int i = Program.executeQuery(string.Format(@"select oprType, 'OK' msg from oprMaster where oprCode COLLATE Latin1_General_CS_AS ='"+ Program.oprCode + "' and pwd COLLATE Latin1_General_CS_AS = '"+ txtPwd.Text + "' and cafeCode = '"+ Program.cafeCode + "'"), true, CommandType.Text, null, null, null, null, null);
                if (i == -1)
                    return;
                if (Program.ds.Tables[0].Rows.Count > 0)
                {
                    if (Program.ds.Tables[0].Rows[0]["msg"].ToString() == "OK")
                        Application.Exit();
                }
                else
                {
                    tmrMsgclear.Enabled = true;
                    lbl_Welcomemsg.Text = ott.OutParam[0].ToString();
                    setting();
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.ToString(), "Error2", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error3", MessageBoxButtons.OK);
            }
        }
        /// <summary>
        /// //////////////////////////////////////  Payment Type  ////////////////////////////////////////////
        /// </summary>
        /// 
        private void prepareAutoExit()
        {
            tmrAutoExit.Enabled = false;
            autoExit = -1;
            tmrAutoExit.Enabled = true;
        }
        public void mealTypeStatus()
        {
            prepareAutoExit();
            try
            {
                pb_lunchInner_Click(null, null);
                int i = Program.executeQuery(string.Format("select mealtype,status from MealTypeStatus"), true, CommandType.Text, null, null, null, null, null);
                if (i == -1)
                    return;
                DataTable dt1 = Program.ds.Tables[0];
                foreach (DataRow r in dt1.Rows)
                {
                    if (r[0].ToString() == "Buffet")
                    {
                        checkBuffetMenuVal = r[1].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private void tmrAutoExit_Tick(object sender, EventArgs e)
        {
            try
            {
                checkBuffetMenuVal = "";
                lbl_Welcomemsg.Text = "";
                tb_All.SelectedTab = tb_Welcome;
                txtcarduid.Focus();
            }
            catch (Exception ex)
            {
                    
            }
        }
        private void pnl_backPic_Click(object sender, EventArgs e)
        {
            tmrAutoExit.Enabled = false;
            checkBuffetMenuVal = "";
            tb_All.SelectedIndex = 0;
        }
        private void pb_lunchInner_Click(object sender, EventArgs e)
        {
            formOpenType = "Fixed";
           // checkSodexoGuest();
        }
        private void checkSodexoGuest()
        {
            formOpenType = "Fixed";
            pnl_step2Pic.Visible = true;
            lbl_selPayOption.Visible = true;
            string _checkGuest = Program.isGuestAllowed;
            string _checkContract = Program.isPaytmAllowed == null ? "N" : Program.isPaytmAllowed.Trim() == "" ? "N" : Program.isPaytmAllowed.Trim();

          
            if (_checkGuest == "1")
            {
                tb_All.SelectedTab = tb_PaymentType;

                pnl_salary.Visible = true;
                pnl_guest.Visible = true;
                pnl_contractual.Visible = false;

                pnl_guest.Location = new Point((panel1.Width / 3) - (pnl_guest.Width / 2) + (pnl_salary.Width + 50), pnl_salary.Location.Y);
                pnl_salary.Location = new Point((panel1.Width / 3) - (pnl_salary.Width / 2) + (pnl_guest.Width + 50) + (pnl_guest.Width + 50), 60);               
            }
            else
            {
                if (_checkContract.Trim().ToLower() == "y")
                {
                    pnl_salary.Visible = false;
                    pnl_guest.Visible = false;
                    pnl_contractual.Visible = false;

                    pnl_contractual.Location = new Point((panel1.Width / 2) - (pnl_contractual.Width / 2), (panel1.Height / 5));
                }
                else
                {
                    pnl_salary.Visible = true;
                    pnl_guest.Visible = false;
                    pnl_contractual.Visible = false;
                    pnl_salary.Location = new Point((panel1.Width / 2), panel1.Height / 2);
                    imgBtnSalary_Click(null, null);
                }
            }

            lbl_selPayOption.Location = new Point(lbl_selMealType.Location.X, pnl_salary.Location.Y);
            pnl_step2Pic.Location = new Point(lbl_selPayOption.Location.X, lbl_selPayOption.Location.Y + lbl_selPayOption.Height + 20);
            
        }
        private void imgBtnSalary_Click(object sender, EventArgs e)
        {
            Program._couponType = "Employee";
            Program._paymentOption = "Salary";

            if (formOpenType == "Fixed")
            {
                tmrAutoExit.Enabled = false;
                pnl_confirm.Visible = false;
                tb_All.SelectedTab = tb_FixedMeal;
                FixedMenuManage();
            }
            else
                return;
        }
        private void guestCoupon_Click(object sender, EventArgs e)
        {
            guestCoupon.BorderStyle = BorderStyle.FixedSingle;
            guestCoupon.BackColor = Color.AliceBlue;

            Program._couponType = "Guest";
            Program._paymentOption = "Guest";

            if (formOpenType == "Fixed")
            {
                tb_All.SelectedTab = tb_FixedMeal;
                pnl_confirm.Visible = false;
                FixedMenuManage();
            }
            else
                return;
        }
        /// <summary>
        /// //////////////////////////////////////  Fixed Meal  ////////////////////////////////////////////
        /// </summary>
        /// 
        public void FixedMenuManage()
        {
            lblName.Text ="Hi "+ Program.empName;
            txtEmpID.Text = Program.empID;
            txtCardNo.Text = Program.cardNo;
            txt_cardtype.Text = Program.cardCat;
            lbl_fixederrormsg.Text = "";
            settingfixedmenu();
            showVendors();
        }
        public void settingfixedmenu()
        {
            lbl_fixederrormsg.Location = new Point(
               pnl_fixedmenuheader.ClientSize.Width / 2 - lbl_fixederrormsg.Size.Width / 2,
               pnl_fixedmenuheader.ClientSize.Height / 2 - lbl_fixederrormsg.Size.Height / 2);
            lbl_fixederrormsg.Anchor = AnchorStyles.None;
            pnl_confirm.BackgroundImage = null;
            lblName.Location = new Point(
                pnl_name.ClientSize.Width / 2 - lblName.Size.Width / 2,
                pnl_name.ClientSize.Height / 2 - lblName.Size.Height / 2);
            lblName.Anchor = AnchorStyles.None;
            pnl_info.Location = new Point(
               panel9.ClientSize.Width / 2 - pnl_info.Size.Width / 2,
               panel9.ClientSize.Height / 2 - pnl_info.Size.Height / 2);
            pnl_info.Anchor = AnchorStyles.None;
            lblVendorName.Location = new Point(
               pnl_VendorName.ClientSize.Width / 2 - lblVendorName.Size.Width / 2,
               pnl_VendorName.ClientSize.Height / 2 - lblVendorName.Size.Height / 2);
            lblVendorName.Anchor = AnchorStyles.None;
        }
        private void showVendors()
        {
            Program.ctgType = "Fixed";          
            try
            {                
               int i = Program.executeQuery(string.Format(@"select * from udf_vendor_avbl('" + Program.empCtg + "','" + Program.cafeCode + "');select slipStatus from slipFooter"), true, CommandType.Text, null, null, null, null, null);
                if (i == -1)
                    return;
                if (Program.ds.Tables[0] != null && Program.ds.Tables[0].Rows.Count > 0)
                {
                    Program.vendorName = Program.ds.Tables[0].Rows[0][0].ToString();
                    lblVendorName.Text = Program.vendorName;
                    settingfixedmenu();
                    displayItems();
                }
                else
                {
                    lbl_fixederrormsg.Text = "Lunch time exists between from "+Program._Starttime+ " to " + Program._Endtime + ".";
                    settingfixedmenu();
                    tmrAutoExit.Enabled = true;
                }
            }
            catch (SqlException se)
            {
                MessageBox.Show(se.ToString(), "Error", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }
        public void displayItems()
        {
            try
            {
                int i = Program.executeQuery(string.Format(@"select * from udf_subsidized_avbl('" + Program.empCtg + "','" + Program.vendorName + "','" + Program.cafeCode + "')"), true, CommandType.Text, null, null, null, null, null);
                if (i == -1)
                    return;
                if (Program.ds.Tables[0] != null && Program.ds.Tables[0].Rows.Count > 0)
                {
                    Program.itemCtgNameSub = Program.ds.Tables[0].Rows[0][0].ToString();
                    Program.gl_itemName = Program.ds.Tables[0].Rows[0][1].ToString();
                    Program.gl_itemName = Program.gl_itemName.Split(',')[0];
                    save();
                }
                else
                {
                    lbl_fixederrormsg.Text = "No Item found.";
                    tmrAutoExit.Enabled = true;
                }

            }
            catch (SqlException se)
            {
               
            }
            catch (Exception ex)
            {
                
            }
        }
        public void save()
        {
            try
            {
                ott = Program.ExecuteProc_value("udp_add_trans_sub", new object[] { Program.vendorName, Program.itemCtgNameSub, Program.gl_itemName, "1", Program.cardID, Program.cafeCode, Program.oprCode, Program.clientType, Program.version, Program.KioskCode, Program.machineName, Program._couponType, Program._paymentOption, "", "", "", "", "", "", "", "", "", "", "", "", "", "" });
                if (ott.OutParam[0].ToString() == "OK")
                { 
                    timerok.Enabled = true;
                }
                else
                {
                    lbl_fixederrormsg.Text = ott.OutParam[0].ToString();
                    settingfixedmenu();
                    tmrAutoExit.Enabled = true;
                }
            }
            catch (SqlException se)
            {
                
            }
            catch (Exception ex)
            {
               
            }
        }
        private void timerok_Tick(object sender, EventArgs e)
        {
            timerok.Enabled = false;            tmrAutoExit.Enabled = true;
            pnl_mealimage.BackgroundImage = Properties.Resources.OK;
            pnl_confirm.Visible = true;
            GetMealCount();
        }
    }
}
