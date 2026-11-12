using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Management;

namespace eCafeClient
{
    public partial class frmLogin : Form
    {
        string txtTarget = "";
        bool strCaps = true;
        

        public frmLogin()
        {
            InitializeComponent();

            tssLbl1.Text = Program.tssLbl1;
            tssLbl2.Text = Program.tssLbl2;
            tssLbl3.Text = Program.tssLbl3;
            tssLbl4.Text = Program.tssLbl4;

            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Height = Screen.PrimaryScreen.Bounds.Height;
            //groupBox3.Location = new Point(10, (Screen.PrimaryScreen.Bounds.Height - groupBox1.Height) * 3 / 4);
            //groupBox2.Location = new Point(10 + groupBox3.Width + 10, (Screen.PrimaryScreen.Bounds.Height - groupBox1.Height) * 3 / 4);
            //groupBox1.Location = new Point(groupBox2.Location.X + groupBox2.Width + 20, (Screen.PrimaryScreen.Bounds.Height - groupBox1.Height) * 3 / 4);

            ////groupBox2.Location = new Point((Screen.PrimaryScreen.Bounds.Width - 30 - groupBox2.Width) /2 , (Screen.PrimaryScreen.Bounds.Height - groupBox1.Height) * 3 / 4);
            ////groupBox1.Location = new Point(groupBox2.Location.X + groupBox2.Width + 10, (Screen.PrimaryScreen.Bounds.Height - groupBox1.Height) * 3 / 4);

            panel2.Width = this.Width - groupBox2.Width - 30;
            

            this.BackgroundImageLayout = ImageLayout.Stretch;
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

            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.groupBox4.Location = new Point((this.Width / 2) - (groupBox4.Width / 2), (this.Height / 2) - (groupBox4.Height / 2) - panel1.Height);
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
        private bool accessPermission()
        {
            try
            {
                //{ "Cafe Manager", "Enterprise Manager","Card Manager", "Vendor Manager", "Item Category Planner", "Item Planner", "Access Right", "Operator Master", "Card Top UP", "Bill Cancellation","Reports", "Client", "Reception" }
                string cmdText = @"select case accessRight when 1 then '1' else '0' end 'accessRight' from oprRightMaster 
                    where oprType='"+ Program.oprType + "' and cafeCode='"+ Program.cafeCode + "' and frmIndex=11";
                int i = Program.executeQuery(cmdText, true, CommandType.Text, null, null, null, null, null);
                if (i == -1)
                return false;
                if (Program.ds.Tables[0].Rows.Count > 0)
                {
                    return Program.ds.Tables[0].Rows[0]["accessRight"].ToString() == "1" ? true : false;
                }
                else
                    return false;

            }
            catch (SqlException se)
            {
                MessageBox.Show(se.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                return false;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            long dt = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            if (dt >= 20270526)
            {
                Application.Exit();
                this.Close();
            }
            int i;
            if (checkData())
            {
                try
                {
                    string cmdText = @"select a.oprType,'OK' msg,b.cafeName from oprMaster a,cafeMaster b where a.cafeCode=b.cafeCode and a.oprCode COLLATE Latin1_General_CS_AS='"+ txtOpr.Text + "' and a.pwd COLLATE Latin1_General_CS_AS='"+ txtPwd.Text + "' and b.cafeCode='"+ Program.cafeCode + "'";
                     i = Program.executeQuery(cmdText, true, CommandType.Text, null, null, null, null, null);
                    if (i == -1)
                        return;
                    if (Program.ds.Tables[0].Rows.Count > 0)
                    {
                        if (Program.ds.Tables[0].Rows[0]["msg"].ToString() == "OK")
                        {
                            Program.cafeName = Program.ds.Tables[0].Rows[0]["cafeName"].ToString();
                            Program.oprType = Program.ds.Tables[0].Rows[0]["oprType"].ToString();
                            Program.oprCode = txtOpr.Text;
                            Program.gVersion = lblversion.Text;
                            if (accessPermission())
                            {
                                this.Hide();
                                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadProc));
                                t.Start();
                                this.Close();
                            }
                            else
                                MessageBox.Show("You don't have access permission to client module.", "Caf-e-Lite - Warning", MessageBoxButtons.OK);
                        }
                    }
                    else
                        MessageBox.Show("Wrong credentials.", "Caf-e-Lite - Warning", MessageBoxButtons.OK);

                }
                catch (SqlException se)
                {
                    MessageBox.Show(se.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {

            lblCafeName.Text = Program.cafeName;
            Program.gStrVersion = lblversion.Text.Replace(".", "");
            Program.machineName = System.Environment.MachineName;
            Program.version = "V-1.0";
            
            int i;
            string msg = "";

            Program.machineName = Environment.MachineName;
            Program.KioskCode = "Client001";
            Program.version = "V-1.0";

            //if (checkLic())
            //{
                try
                {
                    string cmdText = "select cafeCode,cafeName from cafeMaster";
                    i = Program.executeQuery(cmdText, true, CommandType.Text, null, null, null, null, null);
                    if (i == -1)
                        return;
                    if (Program.ds.Tables[0].Rows.Count > 0)
                    {
                        for (int row = 0; row < Program.ds.Tables[0].Rows.Count; row++)
                        {
                            cmbCafeName.Items.Add(Program.ds.Tables[0].Rows[row]["cafeName"].ToString());
                        }
                    }
                }
                catch (SqlException se)
                {
                    MessageBox.Show(se.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                }
            //}
            //else
            //{
            //    MessageBox.Show("System Configuration Error.", "Caf-e-Lite - Error", MessageBoxButtons.OK);
            //    this.Dispose();
            //}
        }

        private bool checkData()
        {
            //if (cmbCafeName.Text == "")
            //    MessageBox.Show("Please select Cafe.", "Caf-e-Lite - Warning", MessageBoxButtons.OK);
            //else 
            if (txtOpr.Text == "")
                MessageBox.Show("Please specify User.", "Caf-e-Lite - Warning", MessageBoxButtons.OK);
            else if (txtPwd.Text == "")
                MessageBox.Show("Please specify Password.", "Caf-e-Lite - Warning", MessageBoxButtons.OK);
            else
                return true;
            return false;
        }
        private bool checkLic()
        {
            try
            {
                string cmdText = "select value from master.sys.extended_properties where name=@name";
                Program.paramName = new string[] { "@name" };
                Program.paramValue = new string[] { "AeCC.1" };
                Program.paramDirection = new string[] { "I" };
                Program.paramDbType = new SqlDbType[] { SqlDbType.VarChar };
                Program.paramDbSize = new int[] { 0 };

                int i = Program.executeQuery(cmdText, true, CommandType.Text, Program.paramName, Program.paramValue, Program.paramDirection, Program.paramDbType, Program.paramDbSize);
                if (i == -1)
                    return false;
                if (Program.ds.Tables[0].Rows.Count > 0)
                {
                    if (Program.ds.Tables[0].Rows[0]["value"].ToString() == "1")
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            catch (SqlException se)
            {
                //MessageBox.Show("System Configuration Error", "Caf-e-Lite - Error", MessageBoxButtons.OK);
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "Caf-e-Lite - Error", MessageBoxButtons.OK);
                return false;
            }
        }

        private void txtOpr_Leave(object sender, EventArgs e)
        {
            txtTarget = "txtOpr";
        }

        private void txtPwd_Leave(object sender, EventArgs e)
        {
            txtTarget = "txtPwd";
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string strTarget=btn.Text;
           

        }

        private void txtOpr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) == false && e.KeyChar != (char)8 && e.KeyChar != (char)32 && e.KeyChar != (char)45 && e.KeyChar != (char)47)
                e.Handled = true;   //            32 space,45 -,47 /
        }

        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) == false && e.KeyChar != (char)8 && e.KeyChar != (char)32 && e.KeyChar != (char)45 && e.KeyChar != (char)47)
                e.Handled = true;   //            32 space,45 -,47 /
        }

        public static void ThreadProc()
        {
            Application.Run(new frmWelcome());
        }
        private void txtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, e);
        }
        
    }
}