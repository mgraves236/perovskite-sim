using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra.Double;





namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

		Material GlobalM = new Material();

		public Form1()
        {
            InitializeComponent();

        }

		private void Form1_Load(object sender, EventArgs e) {}

		public Material CreateMaterial()
        {
			Material A = new Material(Convert.ToDouble(numericUpDown1.Value), Convert.ToDouble(numericUpDown2.Value),
				Convert.ToDouble(numericUpDown3.Value), Convert.ToDouble(numericUpDown4.Value),
				Convert.ToDouble(numericUpDown5.Value), Convert.ToDouble(numericUpDown10.Value),
				Convert.ToDouble(numericUpDown11.Value), Convert.ToDouble(numericUpDown12.Value),
				Convert.ToDouble(numericUpDown6.Value), Convert.ToDouble(numericUpDown7.Value),
				Convert.ToDouble(numericUpDown8.Value), Convert.ToDouble(numericUpDown9.Value),
				Convert.ToDouble(numericUpDown13.Value), Convert.ToDouble(numericUpDown14.Value));

			Material B = new Material(Convert.ToDouble(numericUpDown28.Value), Convert.ToDouble(numericUpDown27.Value),
				Convert.ToDouble(numericUpDown26.Value), Convert.ToDouble(numericUpDown25.Value),
				Convert.ToDouble(numericUpDown24.Value), Convert.ToDouble(numericUpDown19.Value),
				Convert.ToDouble(numericUpDown18.Value), Convert.ToDouble(numericUpDown17.Value),
				Convert.ToDouble(numericUpDown23.Value), Convert.ToDouble(numericUpDown22.Value),
				Convert.ToDouble(numericUpDown21.Value), Convert.ToDouble(numericUpDown20.Value),
				Convert.ToDouble(numericUpDown16.Value), Convert.ToDouble(numericUpDown15.Value));

			Material AB = new Material(A, B, Convert.ToDouble(numericUpDown29.Value),
				Convert.ToDouble(numericUpDown30.Value));

			return AB;
		}


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
			saveFileDialog1.Filter = "Text files (*.txt) | *.txt; | Bin files (*.dat) |*.dat";

			/*
             * txt 
             */
			if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (saveFileDialog1.FilterIndex == 1)
				{
					StreamWriter ptr = new StreamWriter(saveFileDialog1.FileName);
					//ptr.WriteLine("k \t VB \t CH \t CL \t CS");
					for (int i = 0; i < chart1.Series["VB"].Points.Count; i++)
					{
						ptr.WriteLine(chart1.Series["VB"].Points[i].XValue + "\t" + chart1.Series["VB"].Points[i].YValues[0]
							+ "\t" + chart1.Series["CH"].Points[i].YValues[0] + "\t" + chart1.Series["CL"].Points[i].YValues[0]
							+ "\t" + chart1.Series["CS"].Points[i].YValues[0]);

					}
				
					ptr.Close();
				}
			}
		}


        private void button1_Click(object sender, EventArgs e)
        {
			GlobalM = CreateMaterial();
			GlobalM.CreateBands();
			GlobalM.SetTemperature(Convert.ToDouble(numericUpDown31.Value));
			//richTextBox1.Text = Convert.ToString(GlobalM.Ep);
			double aS = GlobalM.AddStrain(Convert.ToDouble(numericUpDown32.Value));
			textBox1.Text = Convert.ToString(aS);
		}

        private void button4_Click(object sender, EventArgs e)
        {
			GlobalM = null;
			GlobalM = CreateMaterial();
			//richTextBox1.Text = Convert.ToString(GlobalM.Eg);
		}

        private void button2_Click(object sender, EventArgs e)
        {
            var hamiltonian1 = new Hamiltonian(GlobalM, 5, 3, 4);
            var matrix1 = hamiltonian1.CreateMatrix();
            //richTextBox1.Text = Convert.ToString(matrix1);
            //richTextBox1.Text = Convert.ToString(hamiltonian1.gamma1) + "\t" + Convert.ToString(GlobalM.gamma1)
             //   + "\t" + Convert.ToString(GlobalM.gamma1 - GlobalM.Ep / GlobalM.Eg / 3);

            String print = null;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    print = print + "\t" + "(" + Math.Round(matrix1[i, j].Real) + ", " + Math.Round(matrix1[i, j].Imaginary) + ")";
                }
                print = print + "\n";
            }
            //richTextBox1.Text = print;

            // routes
            double MR = Math.Sqrt((0 - 0) * (0 - 0) + (0 - 0) * (0 - 0) + (0 - Math.PI / GlobalM.aL) * (0 - Math.PI / GlobalM.aL));
			double GR = Math.Sqrt((0 - Math.PI / GlobalM.aL) * (0 - Math.PI / GlobalM.aL) + (0 - Math.PI / GlobalM.aL) * (0 - Math.PI / GlobalM.aL) 
				+ (0 - Math.PI / GlobalM.aL) * (0 + Math.PI / GlobalM.aL));

			MR = 0.15 * MR;
			GR = 0.15 * GR;

			double kx = 0;
			double ky = 0;
			double kz = 0;

			double point = Math.Sqrt(kx * kx + ky * ky + kz * kz);

			//var hamiltonian = new Hamiltonian(GlobalM, kx, ky, kz);
			//         var matrix = hamiltonian.CreateMatrix();
			      

			chart1.Series["VB"].Points.Clear();
			chart1.Series["CH"].Points.Clear();
			chart1.Series["CL"].Points.Clear();
			chart1.Series["CS"].Points.Clear();

			double iter = 0.001;
			int N = Convert.ToInt32(MR / iter);

			for (int i = 0; i <= N; i++)
			{				
                var hamiltonian = new Hamiltonian(GlobalM, kx, ky, kz);
                var matrix = hamiltonian.CreateMatrix();
				MathNet.Numerics.LinearAlgebra.Complex.Vector DiagonalMatrix = hamiltonian.SolveMatrix(matrix);

                chart1.Series["VB"].Points.AddXY(point, -DiagonalMatrix[0].Real / Globals.eVtoH);
                chart1.Series["VB"].Points.AddXY(point, -DiagonalMatrix[1].Real / Globals.eVtoH);
                chart1.Series["CS"].Points.AddXY(point, DiagonalMatrix[2].Real / Globals.eVtoH);
                chart1.Series["CS"].Points.AddXY(point, DiagonalMatrix[3].Real / Globals.eVtoH);
                chart1.Series["CH"].Points.AddXY(point, DiagonalMatrix[4].Real / Globals.eVtoH);
                chart1.Series["CH"].Points.AddXY(point, DiagonalMatrix[5].Real / Globals.eVtoH);
                chart1.Series["CL"].Points.AddXY(point, DiagonalMatrix[6].Real / Globals.eVtoH);
                chart1.Series["CL"].Points.AddXY(point, DiagonalMatrix[7].Real / Globals.eVtoH);
               
                kz = kz + iter;
				point = Math.Sqrt(kx * kx + ky * ky + kz * kz);
				//richTextBox1.Text = Convert.ToString(hamiltonian.SolveMatrix(matrix));

			}

			N = Convert.ToInt32(GR / iter);

			kx = 0.000001;
			ky = 0.000001;
			kz = 0.000001;

			point = Math.Sqrt(kx * kx + ky * ky + kz * kz);

            for (int i = 0; i <= N; i++)
            {
                var hamiltonian = new Hamiltonian(GlobalM, kx, ky, kz);
                var matrix = hamiltonian.CreateMatrix();
                MathNet.Numerics.LinearAlgebra.Complex.Vector DiagonalMatrix = hamiltonian.SolveMatrix(matrix);

				chart1.Series["VB"].Points.AddXY(point, -DiagonalMatrix[0].Real / Globals.eVtoH);
				chart1.Series["VB"].Points.AddXY(point, -DiagonalMatrix[1].Real / Globals.eVtoH);
				chart1.Series["CS"].Points.AddXY(point, DiagonalMatrix[2].Real / Globals.eVtoH);
				chart1.Series["CS"].Points.AddXY(point, DiagonalMatrix[3].Real / Globals.eVtoH);
				chart1.Series["CH"].Points.AddXY(point, DiagonalMatrix[4].Real / Globals.eVtoH);
				chart1.Series["CH"].Points.AddXY(point, DiagonalMatrix[5].Real / Globals.eVtoH);
				chart1.Series["CL"].Points.AddXY(point, DiagonalMatrix[6].Real / Globals.eVtoH);
				chart1.Series["CL"].Points.AddXY(point, DiagonalMatrix[7].Real / Globals.eVtoH);

				kx = kx + iter;
                ky = ky + iter;
                kz = kz + iter;
                point = -Math.Sqrt(kx * kx + ky * ky + kz * kz);
            }


        }
	}
}

