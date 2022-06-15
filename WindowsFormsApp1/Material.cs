using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
	public class Material
	{
		public double Eg;
		public double mh;
		public double gamma1;
		public double gamma2;
		public double gamma3;
		public double C11;
		public double C12;
		public double aV;
		public double aC;
		public double aL;
		public double delta;
		public double Ep;
		public double b;
		public double alpha;

		public double VB = 0;
		public double CS;
		public double CH;
		public double CL;

		public Material()
		{

		}

		public Material(double Eg, double mh, double gamma1, double gamma2, double gamma3,
			double aL, double delta, double Ep,
			double C11, double C12, double aV, double aC, double b, double alpha)
		{
			this.Eg = Eg;
			this.mh = mh;
			this.gamma1 = gamma1;
			this.gamma2 = gamma2;
			this.gamma3 = gamma3;
			this.C11 = C11;
			this.C12 = C12;
			this.aV = aV;
			this.aC = aC;
			this.aL = aL;
			this.delta = delta;
			this.Ep = Ep / 2;
			this.b = b;
			this.alpha = alpha;
		}

		public Material(Material A, Material B, double x, double C)
		{
			this.Eg = Interpolation(A.Eg, B.Eg, x, C) * Globals.eVtoH;
			this.mh = Interpolation(A.mh, B.mh, x, C);
			this.gamma1 = Interpolation(A.gamma1, B.gamma1, x, C);
			this.gamma2 = Interpolation(A.gamma2, B.gamma2, x, C);
			this.gamma3 = Interpolation(A.gamma3, B.gamma3, x, C);
			this.C11 = Interpolation(A.C11, B.C11, x, C);
			this.C12 = Interpolation(A.C12, B.C12, x, C);
			this.aV = Interpolation(A.aV, B.aV, x, C) * Globals.eVtoH; 
			this.aC = Interpolation(A.aC, B.aC, x, C) * Globals.eVtoH;
			this.aL = Interpolation(A.aL, B.aL, x, C) * Globals.AToB;
			this.delta = Interpolation(A.delta, B.delta, x, C) * Globals.eVtoH;
			this.Ep = Interpolation(A.Ep, B.Ep, x, C) * Globals.eVtoH;
			this.b = Interpolation(A.b, B.b, x, C) * Globals.eVtoH;
			this.alpha = Interpolation(A.alpha, B.alpha, x, C) * Globals.eVtoH;
		}


		public double Interpolation(double P_A, double P_B, double x, double C)
		{
			return x * P_A + (1 - x) * P_B + x * (1 - x) * C;
		}

		public void SetTemperature(double T)
		{
			this.Eg += this.alpha * T;
			this.CS += this.alpha * T;
			this.CH += this.alpha * T;
			this.CL += this.alpha * T;
		}

		public void CreateBands()
		{
			this.CS = this.VB + this.Eg;
			this.CH = this.VB + this.Eg + this.delta;
			this.CL = this.VB + this.Eg + this.delta;
		}

		public double AddStrain(double eps_xx)
		{
			double aS = (eps_xx * this.aL + this.aL);

			double deltaECH = 2 * this.aC * (1 - this.C12 / this.C11) * eps_xx;
			double deltaEVH = 2 * this.aV * (1 - this.C12 / this.C11) * eps_xx;
			double deltaES = this.b * (1 + 2 * this.C12 / this.C11) * eps_xx;

			this.VB = this.VB + deltaEVH;
			this.CS = this.CS + deltaECH;
			this.CH = this.CH + deltaECH + deltaES;
			this.CL = this.CL + deltaECH - deltaES;

			/*String text = Convert.ToString(deltaECH) + " " + Convert.ToString(deltaEVH) + " " + Convert.ToString(deltaES) + " " +
				Convert.ToString(this.VB) + " " + Convert.ToString(this.CS) + " " + Convert.ToString(this.CH) + " " + Convert.ToString(this.CL)
				+ " " + Convert.ToString(this.alpha);

			return text;*/
			return aS;
		}

		public void Copy(Material m)
		{
			this.Eg = m.Eg;
			this.mh = m.mh;
			this.gamma1 = m.gamma1;
			this.gamma2 = m.gamma2;
			this.gamma3 = m.gamma3;
			this.C11 = m.C11;
			this.C12 = m.C12;
			this.aV = m.aV;
			this.aC = m.aC;
			this.aL = m.aL;
			this.delta = m.delta;
			this.Ep = m.Ep;
			this.b = m.b;
			this.alpha = m.alpha;
		}


	}
}
