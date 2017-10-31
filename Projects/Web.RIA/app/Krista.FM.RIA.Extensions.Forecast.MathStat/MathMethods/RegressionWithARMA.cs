using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class RegressionWithARMA
    {
        private double[] x;
        private double[] rem;
        private Regression reg;
        private ARMA arma;
        private int numYear;

        public RegressionWithARMA(double[] x, int a, int b, int group, MathGroups loadedMathGroups)
        {
            this.X = x;
            numYear = x.Length;
            reg = new Regression(x, loadedMathGroups);
            reg.GetOptimal(group);
            
            double[] pred;
            rem = reg.Predict(0, out pred);
            /*for (int i = 0; i < x.Length; i++)
            {
                rem[i] = x[i] - pred[i];
            }*/
            arma = new ARMA(rem, a, b);
            ////arma.SolveARMA();
        }

        public double[] X
        {
            get { return x; }
            set { x = value; }
        }

        public double[] Predict(int numPredY)
        {
            double[] predict = new double[numYear + numPredY];
            reg.Predict(numPredY, out predict);

            double[] predRem = arma.PredictARMA(numPredY);

            for (int i = 0; i < numYear + numPredY; i++)
            {
                predict[i] += predRem[i];
            }

            return predict;
        }
    }
}
