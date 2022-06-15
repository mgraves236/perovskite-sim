using System;

public class Function2D
{
	public Function2D(double start, double end, double iter)
	{
		size = (int)abs(start - end) / iter;
		x = new double[size + 2];
		y = new double[size + 2];

		for (int i = 0; i <= size + 1; i++)
		{
			if (x == 0)
			{

			}
			else
			{
				x[i] = start + iter * i;
			}
			y[i] = calcFunction(x[i]);
		}
	}


	public double calcFunction(double x)
	{
		return (exp(x) + 1) / x;
	}


}
