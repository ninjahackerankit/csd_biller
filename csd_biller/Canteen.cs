using PrinterUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESC_POS_USB_NET.Printer;
using ESC_POS_USB_NET.Enums;

namespace csd_biller
{
    public partial class Canteen : Form
    {
        public static DataTable dt;
        public static DataTable bill;
        public static int count;
        public Canteen()
        {
            InitializeComponent();

            bill = new DataTable();
            bill.Columns.Add("Name");
            bill.Columns.Add("Price");
            bill.Columns.Add("Quantity");
            bill.Columns.Add("Total");

            dt = new DataTable();
            // create table which have article and price
            dt.Columns.Add("Name");
            dt.Columns.Add("Price");

            dt = Read();

            List<string> l = new List<string>();
            foreach(DataRow row in dt.Rows)
            {
                l.Add(row["Name"].ToString());
            }
            comboBox1.DataSource = l;

            //comboBox1.DataSource = Articles();
            comboBox1.SelectedIndex = -1;
            Read();

            count = 1;
        }

        /// <summary>
        /// READ THE PRICE FILE
        /// </summary>
        /// <returns></returns>
        public static DataTable Read()
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\project\price _files\csd_main.txt");
            
            //create table with data
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Name");
            dt1.Columns.Add("Price");
            foreach (string line in lines)
            {
                string[] ar = line.Split('|');
                object[] o1 = { ar[0].ToString(), Convert.ToDecimal(ar[1].ToString()) };
                dt1.Rows.Add(o1);

            }
            return dt1;
        }

        /// <summary>
        /// return articles names for combo box list, but now not needed
        /// </summary>
        /// <returns></returns>
        public List<string> Articles()
        {
            List<string> articles = new List<string>();
            articles.Add("Lux Toilet Soap    ");
            articles.Add("Clinic Plus Shampoo");

            return articles;
        }

        /// <summary>
        /// WHEN ARTICLE IS SELECTED FROM DROP DOWN, THEN AUTOMATICALLY FILL THE UNIT PRICE OF ITEM 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string article = comboBox1.SelectedItem == null ? "" : comboBox1.SelectedItem.ToString();
            string price = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString() == article)
                {
                    price = dr[1].ToString();
                    continue;
                }
            }
            txtRate.Text = price;
            txtQuantity.Text = "";
            txttotalprice.Text = "";
        }

        /// <summary>
        /// WHEN WUANTITY OF ITEM IS ENTERED, AUTOMATICALLY CALCULATE TOTAL PRICE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            int qty = txtQuantity.Text == "" ? 0 : Convert.ToInt32(txtQuantity.Text.ToString());
            //txtprice = qty *

            // Presuming the DataTable has a column named Date.
            string article = comboBox1.SelectedItem.ToString();
            string price = txtRate.Text.ToString();

            txttotalprice.Text = (qty * (Convert.ToDecimal(price))).ToString();
        }

        /// <summary>
        /// ADD ITEM TO BILL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtQuantity.Text.ToString() == "")
            {
                MessageBox.Show("Pls Enter Quantity of Item !!");
            }
            else
            {
                string article = comboBox1.SelectedItem.ToString();
                object[] o = { article, txtRate.Text.ToString(), txtQuantity.Text.ToString(), txttotalprice.Text.ToString() };
                bill.Rows.Add(o);
                dataGridView1.DataSource = bill;
                dataGridView1.AutoResizeColumns();

                decimal ttl = 0;
                foreach(DataRow dr in bill.Rows)
                {
                    ttl = ttl + Convert.ToDecimal(dr["Total"]);
                }

                txtTotalBill.Text = ttl.ToString();
            }
        }

        /// <summary>
        /// PRINT THE BILL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime localDate = DateTime.Now;
            // print bill
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            System.Text.StringBuilder f = new System.Text.StringBuilder();
            double total = 0;
            foreach (System.Data.DataRow r in bill.Rows)
            {
                System.Text.StringBuilder temp = new StringBuilder();
                System.Text.StringBuilder temp2 = new StringBuilder();

                if (r["Name"].ToString().Length>22)
                {
                    temp.Append(r["Name"].ToString().Substring(0, 22) + "    ");
                    temp.Append(r["Quantity"].ToString() + "    ");
                    temp.Append(r["Price"].ToString() + "    ");
                    temp.Append(r["Total"].ToString() + "    ");

                    while(temp.ToString().Length<=48)
                    {
                        temp.Append(" ");
                    }

                    // so rest of string comes below and not in same line (CHECK BELOW COMMENTS)
                    /// VLCC SOME PRODUCT NAME  2 190.5  281
                    /// REMAINING NAME
                    temp.Append(r["Name"].ToString().Substring(22) + "    ");

                    // to save the data in file
                    temp2.Append(r["Name"].ToString() + "    ");
                    temp2.Append(r["Quantity"].ToString() + "    ");
                    temp2.Append(r["Price"].ToString() + "    ");
                    temp2.Append(r["Total"].ToString() + "    ");
                }
                else
                {
                    string name = r["Name"].ToString();

                    // Add extra spaces,if str length is less than 22 
                    for (int i= 0; i<22- r["Name"].ToString().Length; i++)
                    {
                        name = name +  i.ToString();
                    }
                    
                    temp.Append(name + "    ");
                    temp.Append(r["Quantity"].ToString() + "    ");
                    temp.Append(r["Price"].ToString() + "    ");
                    temp.Append(r["Total"].ToString() + "    ");

                    // save data in file
                    temp2.Append(name + "    ");
                    temp2.Append(r["Quantity"].ToString() + "    ");
                    temp2.Append(r["Price"].ToString() + "    ");
                    temp2.Append(r["Total"].ToString() + "    ");
                }

                total = total + Convert.ToDouble(r["Total"].ToString());
                b.Append(temp.ToString());
                b.Append("\n");

                f.Append(temp2.ToString());
                f.Append("\n");
            }

            //rounding off the decimal number
            int totalbill = RoundOff(total);
            string totalstr = " Total bill = " + totalbill.ToString();

            /// bill printer start
            Printer printer = new Printer("EPSON TM-T82II Receipt5");
            printer.Separator('*');
            printer.AlignCenter();
            printer.BoldMode("SAINIK CANTEEN ENTERPRISES");
            printer.Append("CP COLONY, ROAD NO 1, GAYA, BIHAR 823001");
            printer.Separator();
            printer.AlignLeft();
            printer.Font("Name                    Quantity Price Total", Fonts.FontA);
            printer.Separator();
            printer.Font(b.ToString(), Fonts.FontA);
            printer.Separator();
            printer.InitializePrint();
            printer.AlignRight();
            printer.Font(totalstr, Fonts.FontA);
            printer.Separator();
            printer.AlignCenter();
            printer.Append("Thank You!!! ");
            printer.Separator('*');

            printer.FullPaperCut();
            printer.PrintDocument();
            /// bill printer end
            MessageBox.Show(b.ToString());        


            bill.Clear();
            dataGridView1.DataSource = null;
            txtTotalBill.Text = "";
            txttotalprice.Text = "";
            //txtQuantity.Text = "";

            DateTime today = DateTime.Today;
            string time = DateTime.Now.ToString("h:mm:sec");
            string[] tar = time.Split(':');
            string cur_time = tar[0].ToString() + "_" + tar[1].ToString() + "_" + tar[2].ToString() + "_";
            string path = @"C:\project\bills\bill_" + today.ToString("dd-MM-yyyy") + "_"  + cur_time+ "_" +count.ToString() + ".txt";
            count = count + 1;
            //save the data in text file.
            System.IO.File.WriteAllText(path, f.ToString());

        }

        /// <summary>
        /// close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public int RoundOff(double num)
        {
            if(num.ToString().IndexOf('.')<0)
            {
                return Convert.ToInt32(num);
            }
            else
            {
                double roundnum = num;
                for (int i = 0; i < num.ToString().IndexOf('.'); i++)
                {
                    roundnum = roundnum / 10;
                }

                if (roundnum > 0)
                {
                    int n = Convert.ToInt32(num.ToString().Substring(0, Convert.ToInt32(num.ToString().IndexOf('.')))) + 1;
                    return n;
                }
                else
                {
                    return Convert.ToInt32(num);
                }
            }                     
        }
    }
}
