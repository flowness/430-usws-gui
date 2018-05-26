using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        double[][] Data = new double[2][];


        public Form1()
        {
            InitializeComponent();
            chart1.ChartAreas[0].Axes[1].Minimum = 0;
            chart1.ChartAreas[0].Axes[1].Maximum = 100;
            comboBox1.DataSource= System.IO.Ports.SerialPort.GetPortNames();
            serialPort1.NewLine = "\r\n";
            Data[0] = new double[160];
            Data[1] = new double[160];
       }


        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string ReciveString = serialPort1.ReadLine();// .Read(ReciveBuffer, offset, serialPort1.BytesToRead);
                var suff = Newtonsoft.Json.Linq.JObject.Parse(ReciveString);
                suff["UDS"].Values<double>().ToArray<double>().CopyTo(Data[1], 0);
                suff["UPS"].Values<double>().ToArray<double>().CopyTo(Data[0], 0);
                
                chart1.BeginInvoke(new Action(() =>
                {
                    //                    if (chart1.BorderSkin.BackHatchStyle < ChartHatchStyle.ZigZag) chart1.BorderSkin.BackHatchStyle += 1;
                    //                    else chart1.BorderSkin.BackHatchStyle = ChartHatchStyle.BackwardDiagonal;

                    for (int i = 0; i < 1; i++)
                    {
                        if (chart1.ChartAreas[0].Axes[1].Maximum < Data[i].Max())
                        {
                            chart1.ChartAreas[0].Axes[1].Minimum = -1 * Data[i].Max();
                            chart1.ChartAreas[0].Axes[1].Maximum = Data[i].Max();

                        }
                    }
                    for (int i = 0; i < 1; i++)
                    {
                        if (chart1.ChartAreas[0].Axes[1].Minimum > Data[i].Min())
                        {
                            chart1.ChartAreas[0].Axes[1].Minimum = Data[i].Min();
                            chart1.ChartAreas[0].Axes[1].Maximum = -1 * Data[i].Min();

                        }
                    }

                    chart1.Series[0].Points.DataBindY(Data[0]);
                    chart1.Series[1].Points.DataBindY(Data[1]);


                   // chart1.Invalidate();
                }));
                radioButton1.BeginInvoke(new Action(() =>
                {
                    if (radioButton1.BackColor.ToKnownColor() != KnownColor.Green) radioButton1.BackColor = Color.FromKnownColor(KnownColor.Green);
                    else radioButton1.BackColor = Color.FromKnownColor(KnownColor.Red);

                }));
                return;
            }
            catch (Exception error)
            {
                Console.WriteLine("{0} Exception caught.", error);
                return;
            }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.Close();
                button1.Text = "&Connect";
                radioButton1.BeginInvoke(new Action(() => radioButton1.BackColor = Color.FromKnownColor(KnownColor.White)));
/*
                chart1.BeginInvoke(new Action(() =>
                {
                    chart1.BorderSkin.BackHatchStyle = 0;
                    chart1.Series[0].Points.Clear();
                    chart1.Series[1].Points.Clear();
                }));
*/
            }
            else
            {
                serialPort1.Open();
                serialPort1.DiscardInBuffer();
                button1.Text = "&Disconnect";


                

            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.DataSource = System.IO.Ports.SerialPort.GetPortNames();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.SelectedValue.ToString();
        }
    }
}
