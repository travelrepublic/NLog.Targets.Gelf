using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonLog_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            switch (btn.Name)
            {
                case "buttonTrace":
                    _logger.Trace("This is a sample trace message");
                    break;
                case "buttonDebug":
                    _logger.Debug("This is a sample debug message");
                    break;
                case "buttonInfo":
                    _logger.Info("This is a sample info message");
                    break;
                case "buttonWarn":
                    _logger.Warn("This is a sample warn message");
                    break;
                case "buttonError":
                    _logger.Error("This is a sample error message");
                    break;
                case "buttonFatal":
                    _logger.Fatal("This is a sample fatal message");
                    break;
                case "buttonRandomMessage":
                    // 32 is a space, the rest are the upper case alphabet.
                    var byteValues = new[] { 32, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
                    var bytesToSend = new List<byte>();
                    var random = new Random();
                    int numberOfBytesToSend = int.Parse(tbxRandomMessageSize.Text);
                    for (int i = 1; i <= numberOfBytesToSend; i++) // 7700 succeeds, 7800 fails
                    {
                        bytesToSend.Add(Convert.ToByte(byteValues[random.Next(0, 26)]));
                    }
                    _logger.Fatal(new System.Text.ASCIIEncoding().GetString(bytesToSend.ToArray()));
                    break;
                case "buttonCustomMessage":
                    _logger.Fatal(tbxCustomMessage.Text);
                    break;
            }
        }
    }
}
