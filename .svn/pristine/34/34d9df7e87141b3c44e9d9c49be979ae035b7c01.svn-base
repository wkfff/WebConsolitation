using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class FDistr
    {
        public const double Machineepsilon = 5E-16;
        public const double Maxrealnumber = 1E300;
        public const double Minrealnumber = 1E-300;
        
        // Здесь мое понимание кончается
        // Все функции идут в порядке их использования 1 использует 2-ую, та 3-юю и т.д.
        // Результаты совпадают с таблицами википедии и твоего учебника.
        public static double Incompletebeta(double a, double b, double x)
        {
            double result = 0;
            double t = 0;
            double xc = 0;
            double w = 0;
            double y = 0;
            int flag = 0;
            double sg = 0;
            double big = 0;
            double biginv = 0;
            double maxgam = 0;
            double minlog = 0;
            double maxlog = 0;

            big = 4.503599627370496e15;
            biginv = 2.22044604925031308085e-16;
            maxgam = 171.624376956302725;
            minlog = Math.Log(Minrealnumber);
            maxlog = Math.Log(Maxrealnumber);
            
            if (x.CompareTo(0) == 0)
            {
                result = 0;
                return result;
            }

            if (x.CompareTo(1) == 0)
            {
                result = 1;
                return result;
            }

            flag = 0;
            
            if (((b * x).CompareTo(1.0) <= 0) && (x.CompareTo(0.95) <= 0))
            {
                result = Incompletebetaps(a, b, x, maxgam);
                return result;
            }

            w = 1.0 - x;
            
            if (x.CompareTo(a / (a + b)) > 0)
            {
                flag = 1;
                t = a;
                a = b;
                b = t;
                xc = x;
                x = w;
            }
            else
            {
                xc = w;
            }

            if (((flag == 1) && (b * x).CompareTo(1.0) <= 0) && (x.CompareTo(0.95) <= 0))
            {
                t = Incompletebetaps(a, b, x, maxgam);
                if (t.CompareTo(Machineepsilon) <= 0)
                {
                    result = 1.0 - Machineepsilon;
                }
                else
                {
                    result = 1.0 - t;
                }

                return result;
            }

            y = (x * (a + b - 2.0)) - (a - 1.0);
            
            if (y.CompareTo(0) < 0.0)
            {
                w = Incompletebetafe(a, b, x, big, biginv);
            }
            else
            {
                w = Incompletebetafe2(a, b, x, big, biginv) / xc;
            }

            y = a * Math.Log(x);
            t = b * Math.Log(xc);
            
            if (((a + b).CompareTo(maxgam) < 0) && (Math.Abs(y).CompareTo(maxlog) < 0) && (Math.Abs(t).CompareTo(maxlog) < 0))
            {
                t = Math.Pow(xc, b);
                t = t * Math.Pow(x, a);
                t = t / a;
                t = t * w;
                t = t * (Gammafunction(a + b) / (Gammafunction(a) * Gammafunction(b)));
                if (flag == 1)
                {
                    if (t.CompareTo(Machineepsilon) <= 0)
                    {
                        result = 1.0 - Machineepsilon;
                    }
                    else
                    {
                        result = 1.0 - t;
                    }
                }
                else
                {
                    result = t;
                }

                return result;
            }

            y = y + t + Lngamma(a + b, ref sg) - Lngamma(a, ref sg) - Lngamma(b, ref sg);
            y = y + Math.Log(w / a);
            
            if (y.CompareTo(minlog) < 0)
            {
                t = 0.0;
            }
            else
            {
                t = Math.Exp(y);
            }

            if (flag == 1)
            {
                if (t.CompareTo(Machineepsilon) <= 0)
                {
                    t = 1.0 - Machineepsilon;
                }
                else
                {
                    t = 1.0 - t;
                }
            }

            result = t;
            return result;
        }
        
        public static double Lngamma(double x, ref double sgngam)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double p = 0;
            double q = 0;
            double u = 0;
            double w = 0;
            double z = 0;
            int i = 0;
            double logpi = 0;
            double les2Pi = 0;
            double tmp = 0;

            sgngam = 0;
            sgngam = 1;
            logpi = 1.14472988584940017414;
            les2Pi = 0.91893853320467274178;
            if (x.CompareTo(-34.0) < 0)
            {
                q = -x;
                w = Lngamma(q, ref tmp);
                p = (int)Math.Floor(q);
                i = (int)Math.Round(p);
                if (i % 2 == 0)
                {
                    sgngam = -1;
                }
                else
                {
                    sgngam = 1;
                }

                z = q - p;
                if (z.CompareTo(0.5) > 0)
                {
                    p = p + 1;
                    z = p - q;
                }

                z = q * Math.Sin(Math.PI * z);
                result = logpi - Math.Log(z) - w;
                return result;
            }

            if (x.CompareTo(13) < 0)
            {
                z = 1;
                p = 0;
                u = x;
                while (u.CompareTo(3) >= 0)
                {
                    p = p - 1;
                    u = x + p;
                    z = z * u;
                }

                while (u.CompareTo(2) < 0)
                {
                    z = z / u;
                    p = p + 1;
                    u = x + p;
                }

                if (z.CompareTo(0) < 0)
                {
                    sgngam = -1;
                    z = -z;
                }
                else
                {
                    sgngam = 1;
                }

                if (u.CompareTo(2) == 0)
                {
                    result = Math.Log(z);
                    return result;
                }

                p = p - 2;
                x = x + p;
                b = -1378.25152569120859100;
                b = -38801.6315134637840924 + (x * b);
                b = -331612.992738871184744 + (x * b);
                b = -1162370.97492762307383 + (x * b);
                b = -1721737.00820839662146 + (x * b);
                b = -853555.664245765465627 + (x * b);
                c = 1;
                c = -351.815701436523470549 + (x * c);
                c = -17064.2106651881159223 + (x * c);
                c = -220528.590553854454839 + (x * c);
                c = -1139334.44367982507207 + (x * c);
                c = -2532523.07177582951285 + (x * c);
                c = -2018891.41433532773231 + (x * c);
                p = x * b / c;
                result = Math.Log(z) + p;
                return result;
            }

            q = ((x - 0.5) * Math.Log(x)) - x + les2Pi;
            if (x.CompareTo(100000000) > 0)
            {
                result = q;
                return result;
            }

            p = 1 / (x * x);

            if (x.CompareTo(1000.0) >= 0)
            {
                q = q + (((((7.9365079365079365079365 * 0.0001 * p) - (2.7777777777777777777778 * 0.001)) * p) + 0.0833333333333333333333) / x);
            }
            else
            {
                a = 8.11614167470508450300 * 0.0001;
                a = -(5.95061904284301438324 * 0.0001) + (p * a);
                a = (7.93650340457716943945 * 0.0001) + (p * a);
                a = -(2.77777777730099687205 * 0.001) + (p * a);
                a = (8.33333333333331927722 * 0.01) + (p * a);
                q = q + (a / x);
            }

            result = q;
            return result;
        }

        public static double Gammafunction(double x)
        {
            double result = 0;
            double p = 0;
            double pp = 0;
            double q = 0;
            double qq = 0;
            double z = 0;
            int i = 0;
            double sgngam = 0;

            sgngam = 1;
            q = Math.Abs(x);
            if (q.CompareTo(33.0) > 0)
            {
                if (x.CompareTo(0.0) < 0)
                {
                    p = (int)Math.Floor(q);
                    i = (int)Math.Round(p);
                    if (i % 2 == 0)
                    {
                        sgngam = -1;
                    }

                    z = q - p;
                    if (z.CompareTo(0.5) > 0)
                    {
                        p = p + 1;
                        z = q - p;
                    }

                    z = q * Math.Sin(Math.PI * z);
                    z = Math.Abs(z);
                    z = Math.PI / (z * Gammastirf(q));
                }
                else
                {
                    z = Gammastirf(x);
                }

                result = sgngam * z;
                return result;
            }

            z = 1;
            while (x.CompareTo(3) >= 0)
            {
                x = x - 1;
                z = z * x;
            }

            while (x.CompareTo(0) < 0)
            {
                if (x.CompareTo(-0.000000001) > 0)
                {
                    result = z / ((1 + (0.5772156649015329 * x)) * x);
                    return result;
                }

                z = z / x;
                x = x + 1;
            }

            while (x.CompareTo(2) < 0)
            {
                if (x.CompareTo(0.000000001) < 0)
                {
                    result = z / ((1 + (0.5772156649015329 * x)) * x);
                    return result;
                }

                z = z / x;
                x = x + 1.0;
            }

            if (x.CompareTo(2) == 0)
            {
                result = z;
                return result;
            }

            x = x - 2.0;
            pp = 1.60119522476751861407E-4;
            pp = 1.19135147006586384913E-3 + (x * pp);
            pp = 1.04213797561761569935E-2 + (x * pp);
            pp = 4.76367800457137231464E-2 + (x * pp);
            pp = 2.07448227648435975150E-1 + (x * pp);
            pp = 4.94214826801497100753E-1 + (x * pp);
            pp = 9.99999999999999996796E-1 + (x * pp);
            qq = -2.31581873324120129819E-5;
            qq = 5.39605580493303397842E-4 + (x * qq);
            qq = -4.45641913851797240494E-3 + (x * qq);
            qq = 1.18139785222060435552E-2 + (x * qq);
            qq = 3.58236398605498653373E-2 + (x * qq);
            qq = -2.34591795718243348568E-1 + (x * qq);
            qq = 7.14304917030273074085E-2 + (x * qq);
            qq = 1.00000000000000000320 + (x * qq);
            result = z * pp / qq;
            return result;
        }

        private static double Incompletebetafe(double a, double b, double x, double big, double biginv)
        {
            double result = 0;
            double xk = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;
            double k5 = 0;
            double k6 = 0;
            double k7 = 0;
            double k8 = 0;
            double r = 0;
            double t = 0;
            double ans = 0;
            double thresh = 0;
            int n = 0;

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;
            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * Machineepsilon;
            do
            {
                xk = -(x * k1 * k2 / (k3 * k4));
                pk = pkm1 + (pkm2 * xk);
                qk = qkm1 + (qkm2 * xk);
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = x * k5 * k6 / (k7 * k8);
                pk = pkm1 + (pkm2 * xk);
                qk = qkm1 + (qkm2 * xk);
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (qk.CompareTo(0) != 0)
                {
                    r = pk / qk;
                }

                if (r.CompareTo(0) != 0)
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }

                if (t.CompareTo(thresh) < 0)
                {
                    break;
                }

                k1 = k1 + 1.0;
                k2 = k2 + 1.0;
                k3 = k3 + 2.0;
                k4 = k4 + 2.0;
                k5 = k5 + 1.0;
                k6 = k6 - 1.0;
                k7 = k7 + 2.0;
                k8 = k8 + 2.0;

                if ((Math.Abs(qk) + Math.Abs(pk)).CompareTo(big) > 0)
                {
                    pkm2 = pkm2 * biginv;
                    pkm1 = pkm1 * biginv;
                    qkm2 = qkm2 * biginv;
                    qkm1 = qkm1 * biginv;
                }

                if ((Math.Abs(qk).CompareTo(biginv) < 0) || (Math.Abs(pk).CompareTo(biginv) < 0))
                {
                    pkm2 = pkm2 * big;
                    pkm1 = pkm1 * big;
                    qkm2 = qkm2 * big;
                    qkm1 = qkm1 * big;
                }

                n = n + 1;
            } 
            while (n != 300);

            result = ans;
            return result;
        }

        private static double Incompletebetafe2(double a, double b, double x, double big, double biginv)
        {
            double result = 0;
            double xk = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;
            double k5 = 0;
            double k6 = 0;
            double k7 = 0;
            double k8 = 0;
            double r = 0;
            double t = 0;
            double ans = 0;
            double z = 0;
            double thresh = 0;
            int n = 0;

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0;
            k8 = a + 2.0;
            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0 - x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * Machineepsilon;
            do
            {
                xk = -(z * k1 * k2 / (k3 * k4));
                pk = pkm1 + (pkm2 * xk);
                qk = qkm1 + (qkm2 * xk);
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = z * k5 * k6 / (k7 * k8);
                pk = pkm1 + (pkm2 * xk);
                qk = qkm1 + (qkm2 * xk);
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (qk.CompareTo(0) != 0)
                {
                    r = pk / qk;
                }

                if (r.CompareTo(0) != 0)
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }

                if (t.CompareTo(thresh) < 0)
                {
                    break;
                }

                k1 = k1 + 1.0;
                k2 = k2 - 1.0;
                k3 = k3 + 2.0;
                k4 = k4 + 2.0;
                k5 = k5 + 1.0;
                k6 = k6 + 1.0;
                k7 = k7 + 2.0;
                k8 = k8 + 2.0;

                if ((Math.Abs(qk) + Math.Abs(pk)).CompareTo(big) > 0)
                {
                    pkm2 = pkm2 * biginv;
                    pkm1 = pkm1 * biginv;
                    qkm2 = qkm2 * biginv;
                    qkm1 = qkm1 * biginv;
                }

                if ((Math.Abs(qk).CompareTo(biginv) < 0) || (Math.Abs(pk).CompareTo(biginv) < 0))
                {
                    pkm2 = pkm2 * big;
                    pkm1 = pkm1 * big;
                    qkm2 = qkm2 * big;
                    qkm1 = qkm1 * big;
                }

                n = n + 1;
            }
            while (n != 300);

            result = ans;
            return result;
        }

        private static double Incompletebetaps(double a, double b, double x, double maxgam)
        {
            double result = 0;
            double s = 0;
            double t = 0;
            double u = 0;
            double v = 0;
            double n = 0;
            double t1 = 0;
            double z = 0;
            double ai = 0;
            double sg = 0;

            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = Machineepsilon * ai;
            while (Math.Abs(v).CompareTo(z) > 0)
            {
                u = (n - b) * x / n;
                t = t * u;
                v = t / (a + n);
                s = s + v;
                n = n + 1.0;
            }

            s = s + t1;
            s = s + ai;
            u = a * Math.Log(x);

            if (((a + b).CompareTo(maxgam) < 0) && (Math.Abs(u).CompareTo(Math.Log(Maxrealnumber)) < 0))
            {
                t = Gammafunction(a + b) / (Gammafunction(a) * Gammafunction(b));
                s = s * t * Math.Pow(x, a);
            }
            else
            {
                t = Lngamma(a + b, ref sg) - Lngamma(a, ref sg) - Lngamma(b, ref sg) + u + Math.Log(s);
                if (t.CompareTo(Math.Log(Minrealnumber)) < 0)
                {
                    s = 0.0;
                }
                else
                {
                    s = Math.Exp(t);
                }
            }

            result = s;
            return result;
        }

        private static double Gammastirf(double x)
        {
            double result = 0;
            double y = 0;
            double w = 0;
            double v = 0;
            double stir = 0;

            w = 1 / x;
            stir = 7.87311395793093628397E-4;
            stir = -2.29549961613378126380E-4 + (w * stir);
            stir = -2.68132617805781232825E-3 + (w * stir);
            stir = 3.47222221605458667310E-3 + (w * stir);
            stir = 8.33333333333482257126E-2 + (w * stir);
            w = 1 + (w * stir);
            y = Math.Exp(x);

            if (x.CompareTo(143.01608) > 0)
            {
                v = Math.Pow(x, (0.5 * x) - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = Math.Pow(x, x - 0.5) / y;
            }

            result = 2.50662827463100050242 * y * w;
            return result;
        }
    }
}
