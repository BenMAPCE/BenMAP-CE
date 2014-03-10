using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BenMAP
{
    class MontoCarlo
    {
        const int N = 100;
        const int MAX = 50;
        const double MIN = 0.1;
        const int MIU = 40;
        const int SIGMA = 1;
        static Random aa = new Random((int)(DateTime.Now.Ticks / 10000));
        public double AverageRandom(double min, double max)
        {
            int MINnteger = (int)(min * 10000);
            int MAXnteger = (int)(max * 10000);
            int resultInteger = aa.Next(MINnteger, MAXnteger);
            return resultInteger / 10000.0;
        }
        public double Normal(double x, double miu, double sigma)
        {
            return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
        }
        public double Random_Normal(double miu, double sigma, double min, double max)
        {
            double x;
            double dScope;
            double y;
            do
            {
                x = AverageRandom(min, max);
                y = Normal(x, miu, sigma);
                dScope = AverageRandom(0, Normal(miu, miu, sigma));
            } while (dScope > y);
            return x;
        }

        public List<int> Random_NormalArray(double miu, double sigma, double min, double max, int count)
        {
            try
            {
                List<int> lstReturn = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    lstReturn.Add(Convert.ToInt32(Random_Normal(miu, sigma, min, max)));

                }
                return lstReturn;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public double RandExp(double const_a)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double p;
            double temp;
            if (const_a != 0)
                temp = 1 / const_a;
            else
                throw new System.InvalidOperationException("除数不能为零！不能产生参数为零的指数分布！");
            double randres;
            while (true)
            {
                p = rand.NextDouble();
                if (p < const_a)
                    break;
            }
            randres = -temp * Math.Log(temp * p, Math.E);
            return randres;
        }


        Random ran;

        public MontoCarlo()
        {
            ran = new Random();
        }
        public double ngtIndex(double lam)
        {
            double dec = ran.NextDouble();
            while (dec == 0)
                dec = ran.NextDouble();
            return -Math.Log(dec) / lam;
        }


        public double poisson(double lam, double time)
        {
            int count = 0;

            while (true)
            {
                time -= ngtIndex(lam);
                if (time > 0)
                    count++;
                else
                    break;
            }
            return count;
        }
        public List<int> poissonList(double lam, double time, int count)
        {
            List<int> lstReturn = new List<int>();
            for (int i = 0; i < count; i++)
            {
                lstReturn.Add(Convert.ToInt32(poisson(lam, time)));

            }
            return lstReturn;
        }

    }

}
