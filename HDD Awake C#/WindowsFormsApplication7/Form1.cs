using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.AccessControl;

namespace HDDAwakeCSharp
{
    public partial class HDDAwakeCS : Form
    {
        public HDDAwakeCS()
        {
            InitializeComponent();

            string appname = System.AppDomain.CurrentDomain.FriendlyName;

            #region Checking if the process opened before
            if (IsProcessOpen(appname.Remove(appname.Length - 5))) //Checking if the application is already opened
            {
                Environment.Exit(0);
            }
            #endregion
        }

        private string SELECTEDPATH;
        private string wrtPath;
        private string strTextFile;
        protected int writeState = 0;
        FileStream fs;
        int timeRemaining = 4;
        private void CreateLayers()
        {
            SELECTEDPATH = dirTextBox.Text.ToString(); // Get the path
            wrtPath = SELECTEDPATH + "\\Layer1\\Layer2\\Layer3\\Layer4\\Layer5";

            Directory.CreateDirectory(@wrtPath);   // Create folders

            strTextFile = "\\HDD Awake " + DateTime.Now.Date.Day.ToString() + "-" + DateTime.Now.Date.Month.ToString() + "-" + DateTime.Now.Date.Year.ToString() + ".txt";
            //Set text file name

            fs = File.Create(wrtPath + strTextFile);   //Create text file. I need to use FileStream to close the stream after creating the file. Else, it'll throw exception.

            fs.Dispose();
        }

        private void DeleteLayers()
        {
            if (writeState > 0) // If layers are created.
            {
                Directory.Delete(SELECTEDPATH + "\\Layer1", true);
            }
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            CreateLayers();

            backgroundWorker1.RunWorkerAsync();

            writeState++;  //increment write state with each click

            writeButton.Enabled = false;

            label2.Text = "Close the program to stop.";

            timer1.Start();
        }

        private void dirButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            dirTextBox.Text = folderBrowserDialog1.SelectedPath.ToString();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string sleepSec = numericUpDown1.Value.ToString();  //Get string from num-updown
            double sleepMin = Convert.ToDouble(sleepSec);
            int slepsec = Convert.ToInt32(sleepMin * 60);
            TimeSpan timeout = new TimeSpan(0, 0, slepsec);

            while (writeState % 2 != 0) // Work on 1. 3. 5. 7.... clicks 
            {
                string phrase = "Written in: " + DateTime.Now.Hour.ToString() + ":" +
                                                 DateTime.Now.Minute.ToString() + ":" +
                                                 DateTime.Now.Second.ToString() + " || " +
                                                 DateTime.Now.Day.ToString() + "-" +
                                                 DateTime.Now.Month.ToString() + "-" +
                                                 DateTime.Now.Year.ToString() + "\n";

                File.WriteAllText(wrtPath + strTextFile, phrase); // wrtPath is selected path. strTextFile is file name with date on it.

                System.Threading.Thread.Sleep(timeout); // Wait for the specied time.


            }
        }

        private void HDDAwakeCS_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        void HDDAwakeCS_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Prevent interrupting shut down
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                //Stopping infinite loop
                backgroundWorker1.CancelAsync();
            }
        }

        public bool IsProcessOpen(string appProcessName)
        {
            int appState = 0;

            foreach (System.Diagnostics.Process clsProcess in System.Diagnostics.Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(appProcessName))
                    appState = appState + 1;
            }

            if (appState > 1)
            {
                MessageBox.Show("The application is already running!");
                return true;
            }

            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timeRemaining > 0)
            {
                timeRemaining = timeRemaining - 1;
                label3.Text = "Minmizing in: " + timeRemaining;
            }
            else
            { 
                timer1.Stop();
                label3.Text = "Minimizing in: -";
                WindowState = FormWindowState.Minimized;
            }
        }

    }
}
