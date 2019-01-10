using QLCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace QLCafe.DAO
{
    public class ThongKeFood
    {
        private int idFood;

        public int IdFood
        {
            get { return idFood; }
            set { idFood = value; }
        }

        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public class ThongKeBill
    {
        private string dateCheckout;

        public string DateCheckout
        {
            get { return dateCheckout; }
            set { dateCheckout = value; }
        }
       

        private double totalPrice;

        public double TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
    }
    public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance
        {
            get
            {
                if (instance == null) instance = new BillDAO();
                return instance;
            }

            private set
            {
                BillDAO.instance = value;
            }
        }

        private BillDAO() { }

        /// <summary>
        /// Thành công: bill ID
        /// thất bại: -1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetUncheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Bill WHERE idTable = " + id + " AND status = 0");

            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }

            return -1;
        }
        public void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("exec USP_InsertBill @idTable", new object[] { id });
        }

        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }
        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("SELECT MAX(id) FROM dbo.Bill");
            }
            catch
            {
                return 1;
            }
        }

        public void CheckOut(int id, int discount, float totalPrice)
        {
            string query = "UPDATE dbo.Bill SET dateCheckOut = GETDATE(), status = 1, " + "discount = " + discount + ", totalPrice = " + totalPrice + " WHERE id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }


        public List<ThongKeFood> ThongKeFoodByDate(string fromDate , string toDate) 
        {
            List<ThongKeFood> thongke = new List<ThongKeFood>();
            string query = "SELECT   idFood,F.name, sum(count) as count " +
                           " FROM BillInfo as B, Food as F, Bill " +
                           " Where B.idFood = F.id and B.idBill = Bill.id and Bill.DateCheckOut between '"+ fromDate +"' and '"+ toDate +"' " +
                           " GROUP BY idFood, F.name ";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                ThongKeFood temp = new ThongKeFood();
                temp.IdFood = (int)item[0];
                temp.Name = (string)item[1];
                temp.Count = (int)item[2];
                thongke.Add(temp);
            }
            return thongke;
        }


        public List<ThongKeBill> ThongKeDoanhThuByDate(string fromDate, string toDate, string typeDatePicker)
        {
            List<ThongKeBill> thongke = new List<ThongKeBill>();
            string query = "";

            switch (typeDatePicker)
            {
                case "Ngày":
                    query = string.Format("Select DateCheckout as dateCheckOut, sum(totalPrice ) as totalPrice " +
                            "from bill " +
                            "where DateCheckOut between '{0}' and '{1}' " +
                            "Group by DateCheckOut " +
                            "Order by dateCheckOut ",fromDate,toDate);
                    
                    break;
                case "Năm":
                    query = string.Format("Select DatePart(YEAR,DateCheckOut) as year, sum(totalPrice ) as totalPrice " +
                            "from bill " +
                            "where DateCheckOut between '{0}' and '{1}' " +
                            "Group by DatePart(YEAR,DateCheckOut) " +
                            "Order by DatePart(YEAR,DateCheckOut) ",fromDate,toDate);
                    break;

                case "Tháng":
                    query = string.Format(
                        "SELECT CAST(MONTH(DateCheckOut) AS VARCHAR(2)) + '-' + CAST(YEAR(DateCheckOut) AS VARCHAR(4)) as month, sum(totalPrice ) as totalPrice " +
                        "FROM bill " +
                        "WHERE DateCheckOut BETWEEN  '{0}' and '{1}' " +
                        "GROUP BY CAST(MONTH(DateCheckOut) AS VARCHAR(2)) + '-' + CAST(YEAR(DateCheckOut) AS VARCHAR(4)) " +
                        "Order by CAST(MONTH(DateCheckOut) AS VARCHAR(2)) + '-' + CAST(YEAR(DateCheckOut) AS VARCHAR(4)) desc", fromDate, toDate
                );
                    break;

                default:
                    break;
            }


            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                ThongKeBill temp = new ThongKeBill();
                temp.DateCheckout = item[0].ToString();
                temp.TotalPrice = (double)item[1];

                thongke.Add(temp);
            }
            return thongke;
        }
    }

}
