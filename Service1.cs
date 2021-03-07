using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Configuration;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SampleWindowsService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
       // public static string path = ConfigurationManager.AppSettings["FilePath"];
       // string[] files = System.IO.Directory.GetFiles(path, ConfigurationManager.AppSettings["FilePattern"], System.IO.SearchOption.TopDirectoryOnly);
        GetData gd = new GetData();
        

        public Service1()
        {
            InitializeComponent();
        }
        public void InitTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer(Convert.ToDouble(ConfigurationManager.AppSettings["Interval"].ToString()));
            timer.AutoReset = true; // the key is here so it repeats
            timer.Elapsed += OnElapsedTime;
            timer.Start();
        }
        public void displayLog()
        {
           
            try
            {

                gd.getEmpData();
                WriteToFile("\n");
                WriteToFile("/***************************************Single Row Output***************************************/" + "\n");
                WriteToFile("Executed Query: " + gd.selectquery);
                WriteToFile("Query Result: ");
                WriteToFile(gd.data);
                WriteToFile("Query Executed Successfully!!!");
                WriteToFile("\n");


                WriteToFile("\n");
                WriteToFile("/***************************************Whole Table Output***************************************/" + "\n");
                WriteToFile("Executed Query: " + gd.selectqueryfull);
                WriteToFile("Query Result: ");
                WriteToFile(gd.getWholeData());
                WriteToFile("Query Executed Successfully!!!");
                WriteToFile("\n");


                WriteToFile("\n");
                WriteToFile("/***************************************DB Report***************************************/" + "\n");
                WriteToFile("Executed Query: " + gd.dbreport);
                WriteToFile("Query Result: ");
                WriteToFile(gd.getDbReport());
                WriteToFile("Query Executed Successfully!!!");
                WriteToFile("\n");

                

            }
            catch (Exception ex)
            {

                WriteToFile("Error:  " + "\n" + ex.InnerException);
            }
        }
        public void callWebService()
        {
            try
            {
                ClientSection clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");

                string addr = clientSection.Endpoints[0].Address.ToString();
                //string addr = ep.Address.ToString();
                WriteToFile("\n");
                WriteToFile("/***************************************Data From WCF Service***************************************/" + "\n");
                WriteToFile("Calling WCF Service.. " + addr);
                WriteToFile(gd.callWCFService());
                WriteToFile("WCF Service " + addr + "  " + " called at " + DateTime.Now);
                WriteToFile("\n");
            }
            catch (Exception ex)
            {

                WriteToFile("Error:  " + "\n" + ex.Message);
            }
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service started at " + DateTime.Now);
            InitTimer();
            displayLog();
            callWebService();
        }
        protected override void OnStop()
        {
            timer.Stop();
            WriteToFile("Timer Stopped!!!"+"\n");
            WriteToFile("Service stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service recalled at " + DateTime.Now);
            displayLog();
            callWebService();
        }
        public void WriteToFile(string Message)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            string changeext;
            FileInfo Fi;
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath1 = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_')+ DateTime.Now.TimeOfDay.TotalSeconds.ToString() + ".txt";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog.txt";
            Fi = new FileInfo(filepath);
            
                       
            if (!File.Exists(filepath))
            {           
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
                
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            if (File.Exists(filepath))
            {
                long length = Fi.Length;
                if (length > 100000000)
                {
                    System.IO.File.Move(filepath, filepath1);
                }
            }

        }
    }
}
