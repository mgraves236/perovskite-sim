using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
//using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
//using MathNet.Numerics.LinearAlgebra.Double;

namespace WindowsFormsApp1
{

	public static class Globals
	{
		public const double h = 1;
		public const double m0 = 1;
		public const double mh = 1;
		public const double eVtoH = 0.0367502;
		public const double AToB = 1.889725989;
	}

	public class Hamiltonian
	{
		public Complex P;
		public Complex Pp;
		public Complex Pm;
		Complex Pz;
		Complex VB;
		Complex CH;
		Complex CL;
		Complex CS;
		Complex S;
		Complex R;
		public Complex D;
		Complex gammaV;
		public Complex gamma1;
		Complex gamma2;
		Complex gamma3;

		public Hamiltonian(Material Material, double kx, double ky, double kz)
		{

			this.gammaV = new Complex(1 / Globals.mh - Material.Ep / 3 * (2 / Material.Eg + 1 / (Material.Eg + Material.delta)), 0);

			this.gamma1 = new Complex(Material.gamma1- 1 / 3 * Material.Ep / Material.Eg, 0);

			this.gamma2 = new Complex(Material.gamma2 - 1 / 6 * Material.Ep / Material.Eg, 0);

			this.gamma3 = new Complex(Material.gamma3- 1 / 6 * Material.Ep / Material.Eg, 0);

			this.VB = new Complex(Material.VB - Globals.h * Globals.h / (2 * Globals.m0) * this.gammaV.Real * (kx * kx + ky * ky + kz * kz), 0);

			this.CH = new Complex(Material.CH + Globals.h * Globals.h / (2 * Globals.m0) * ((this.gamma1.Real + this.gamma2.Real)
				* (kx * kx + ky * ky) + (this.gamma1.Real - 2 * this.gamma2.Real) * kz * kz), 0);

			this.CL = new Complex(Material.CL + Globals.h * Globals.h / (2 * Globals.m0) * ((this.gamma1.Real - this.gamma2.Real)
				* (kx * kx + ky * ky) + (this.gamma1.Real + 2 * this.gamma2.Real) * kz * kz), 0);

			this.CS = new Complex(Material.CS + Globals.h * Globals.h / (2 * Globals.m0) * (this.gamma1.Real * (kx * kx + ky * ky +
				kz * kz)), 0);

			this.P = new Complex(Globals.h * Math.Sqrt(Material.Ep / (2 * Globals.m0)), 0);

			this.Pp = new Complex(this.P.Real * kx, this.P.Real * ky);

			this.Pm = new Complex(this.P.Real * kx, - this.P.Real * ky);

			this.Pz = new Complex(this.P.Real * kz, 0);

			this.S = new Complex(Globals.h * Globals.h / (2 * Globals.m0) * 2 * Math.Sqrt(3)
				* this.gamma3.Real * (-kx) * kz,
				Globals.h * Globals.h / (2 * Globals.m0) * 2 * Math.Sqrt(3)
				* this.gamma3.Real * (ky) * kz);

			this.R = new Complex(Globals.h * Globals.h / (2 * Globals.m0) * Math.Sqrt(3)
				* this.gamma2.Real * (kx * kx - ky * ky), -2 * this.gamma3.Real * kx * ky);

			this.D = new Complex(Globals.h * Globals.h / (2 * Globals.m0) * Math.Sqrt(2)
				* (kx * kx + ky * ky - 2 * kz * kz), 0);
		}

		public MathNet.Numerics.LinearAlgebra.Complex.Matrix CreateMatrix()
        {
			var Matrix = new MathNet.Numerics.LinearAlgebra.Complex.DenseMatrix(8);

			Matrix[0, 0] = this.VB;
			Matrix[1, 1] = this.VB;
			Matrix[2, 2] = this.CH;
			Matrix[3, 3] = this.CL;
			Matrix[4, 4] = this.CL;
			Matrix[5, 5] = this.CH;
			Matrix[6, 6] = this.CS;
			Matrix[7, 7] = this.CS;
			// down left part [row, column]
			Matrix[2, 0] = new Complex((double)1 / Math.Sqrt(2) * this.Pm.Real, (double)1 / Math.Sqrt(2) * this.Pm.Imaginary);
			Matrix[3, 0] = new Complex( - Math.Sqrt(2) / Math.Sqrt(3) * this.Pz.Real, -Math.Sqrt(2) / Math.Sqrt(3) * this.Pz.Imaginary);
			Matrix[4, 0] = new Complex(-(double)1 / Math.Sqrt(6) * this.Pp.Real, -(double)1 / Math.Sqrt(6) * this.Pp.Imaginary);
			Matrix[6, 0] = new Complex(-(double)1 / Math.Sqrt(3) * this.Pz.Real, -(double)1 / Math.Sqrt(3) * this.Pz.Imaginary);
			Matrix[7, 0] = new Complex(-(double)1 / Math.Sqrt(3) * this.Pp.Real, -(double)1 / Math.Sqrt(3) * this.Pp.Imaginary);

			Matrix[3, 1] = new Complex(1 / Math.Sqrt(6) * this.Pm.Real, 1 / Math.Sqrt(6) * this.Pm.Imaginary);
			Matrix[4, 1] = new Complex(-Math.Sqrt(2) / Math.Sqrt(3) * this.Pz.Real, -Math.Sqrt(2) / Math.Sqrt(3) * this.Pz.Imaginary);
			Matrix[5, 1] = new Complex(-1 / Math.Sqrt(2) * this.Pp.Real, -1 / Math.Sqrt(2) * this.Pp.Imaginary);
			Matrix[6, 1] = new Complex(-1 / Math.Sqrt(3) * this.Pm.Real, -1 / Math.Sqrt(3) * this.Pm.Imaginary);
			Matrix[7, 1] = new Complex(1 / Math.Sqrt(3) * this.Pz.Real, 1 / Math.Sqrt(3) * this.Pz.Imaginary);

			Matrix[3, 2] = Complex.Conjugate(this.S);
			Matrix[4, 2] = - Complex.Conjugate(this.R);
			Matrix[6, 2] = 1 / Math.Sqrt(2) * Complex.Conjugate(this.S);
			Matrix[7, 2] = - Math.Sqrt(2) * Complex.Conjugate(this.R);
			
			Matrix[5, 3] = - Complex.Conjugate(this.R);
			Matrix[6, 3] = - this.D;
			Matrix[7, 3] = - Math.Sqrt(3) / Math.Sqrt(2) * Complex.Conjugate(this.S);
			
			Matrix[5, 4] = - Complex.Conjugate(this.S);
			Matrix[6, 4] = new Complex(-Math.Sqrt(3) / Math.Sqrt(2) * this.S.Real, -Math.Sqrt(3) / Math.Sqrt(2) * this.S.Imaginary);
			Matrix[7, 4] = this.D;
			
			Matrix[6, 5] = Math.Sqrt(2) * this.R;
			Matrix[7, 5] = new Complex(1 / Math.Sqrt(2) * this.S.Real, 1 / Math.Sqrt(2) * this.S.Imaginary);

			for (int i = 1; i < 8; i++)
            {
				for (int j = 0; j < i; j++)
                {
					Matrix[j, i] = Complex.Conjugate(Matrix[i, j]);
                }
            }
			
			return Matrix;
        }

		public MathNet.Numerics.LinearAlgebra.Complex.Vector SolveMatrix(MathNet.Numerics.LinearAlgebra.Complex.Matrix Matrix)
        {
			var factorEvd = Matrix.Evd(symmetricity:MathNet.Numerics.LinearAlgebra.Symmetricity.Hermitian);
			MathNet.Numerics.LinearAlgebra.Complex.Vector values;
			values = (MathNet.Numerics.LinearAlgebra.Complex.Vector)factorEvd.EigenValues;

			return values;
		}
	}
}
