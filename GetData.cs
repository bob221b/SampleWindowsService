using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SampleWindowsService
{
    public class GetData
    {
        string constr = ConfigurationManager.ConnectionStrings["EmployeeConnection"].ConnectionString;
        SqlConnection con;
        SqlDataReader r;
        SqlCommand cmd;
        SqlDataAdapter dap;
        DataTable Table = new DataTable("EmployeeTable");
        DataTable Table1 = new DataTable("DBReport");
        public string data;
        public string selectquery;
        public string selectqueryfull;
        public string dbreport;
        ServiceReference1.EmployeeServiceClient emp = new ServiceReference1.EmployeeServiceClient();
        public void connection()
        {
            con = new SqlConnection(constr);
            con.Open();
        }
        public void getEmpData()
        {
            connection();
            selectquery = ConfigurationManager.AppSettings["SelectQuery"].ToString();
            cmd = new SqlCommand(selectquery, con);
            r = cmd.ExecuteReader();

            while (r.Read())
            {
                data = r[0].ToString() + "    " + r[1].ToString();
            }
            con.Close();
        }
        public string getWholeData()
        {
            connection();
            selectqueryfull = ConfigurationManager.AppSettings["SelectQuery"].ToString();
            cmd = new SqlCommand(selectqueryfull, con);
            dap = new SqlDataAdapter(cmd);
            Table.Clear();
            dap.Fill(Table);
            con.Close();
            return dumpTableData(Table);

        }
        public string getDbReport()
        {
            connection();
            dbreport = ConfigurationManager.AppSettings["DBReportSP"].ToString();
            cmd = new SqlCommand(dbreport, con);
            dap = new SqlDataAdapter(cmd);
            Table1.Clear();
            dap.Fill(Table1);
            con.Close();
            return dumpTableData(Table1);

        }
        public string callWCFService()
        {
           return emp.GetData();

        } 

        public  string dumpTableData(DataTable table)
        {
            
            string data = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (null != table && null != table.Rows)
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        sb.Append(item);
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }

                data = sb.ToString();
            }
            return data;
        }
        
        

    }
}
