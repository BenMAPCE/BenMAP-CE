using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenMAP
{
    class GBDRollbackKrewskiFunction
    {

        //first argument is expected to be differential (change) of concentration when rollback is applied
        //example:  Baseline 50, rollback to 35 standard, delta=15
        public GBDRollbackKrewskiResult GBD_math(double[] concDelta, double[] population, double incrate, double beta, double se)
        {
            double Sum_97_5 = 0;
            double Krewski = 0;
            double Sum_2_5 = 0;
            //double beta = 0.005826891;
            //double se = 0.000962763;
            //double IncrateIn = 0.0081120633;
            for (int idx = 0; idx < concDelta.Length; idx++)
            {

                double ConcIn = concDelta[idx];
                double PopIn = population[idx];
                
                double krewski = (1 - (1 / Math.Exp(beta * ConcIn))) * PopIn * incrate;
                double krewski_2_5pct = (1 - (1 / Math.Exp(qnorm5(.025, beta, se, true, false) * ConcIn))) * PopIn * incrate;
                double krewski_97_5pct = (1 - (1 / Math.Exp(qnorm5(.975, beta, se, true, false) * ConcIn))) * PopIn * incrate;
                Krewski += krewski;
                Sum_2_5 += krewski_2_5pct;
                Sum_97_5 += krewski_97_5pct;
            }
            Console.WriteLine("Krewski: "+Krewski);
            Console.WriteLine("2.5: "+Sum_2_5);
            Console.WriteLine("97.5: " +Sum_97_5);
            return new GBDRollbackKrewskiResult(Krewski, Sum_2_5, Sum_97_5);
        }

        //qnorm function
        //found at https://svn.r-project.org/R/trunk/src/nmath/qnorm.c
        // reference author is: Wichura, M. J. (1988) Algorithm AS 241: The percentage points of the normal distribution. Applied Statistics, 37, 477–484.
        private double qnorm5(double p, double mu, double sigma, bool lower_tail, bool log_p)
        {
            double p_, q, r, val;


            //#ifdef IEEE_754
            if (Double.IsNaN(p) || Double.IsNaN(mu) || Double.IsNaN(sigma))
                return p + mu + sigma;
            //#endif
            R_Q_P01_boundaries(p, Double.NegativeInfinity, Double.PositiveInfinity, log_p, lower_tail);

            if (sigma < 0) return double.NaN;
            if (sigma == 0) return mu;

            p_ = R_DT_qIv(p, log_p, lower_tail);/* real lower_tail prob. p */
            q = p_ - 0.5;

            //#ifdef DEBUG_qnorm
            //  REprintf("qnorm(p=%10.7g, m=%g, s=%g, l.t.= %d, log= %d): q = %g\n",
            //    p,mu,sigma, lower_tail, log_p, q);
            //#endif


            /*-- use AS 241 --- */
            /* double ppnd16_(double *p, long *ifault)*/
            /*      ALGORITHM AS241  APPL. STATIST. (1988) VOL. 37, NO. 3

                    Produces the normal deviate Z corresponding to a given lower
                    tail area of P; Z is accurate to about 1 part in 10**16.

                    (original fortran code used PARAMETER(..) for the coefficients
                     and provided hash codes for checking them...)
            */
            if (Math.Abs(q) <= .425)
            {/* 0.075 <= p <= 0.925 */
                r = .180625 - q * q;
                val =
                        q * (((((((r * 2509.0809287301226727 +
                                   33430.575583588128105) * r + 67265.770927008700853) * r +
                                 45921.953931549871457) * r + 13731.693765509461125) * r +
                               1971.5909503065514427) * r + 133.14166789178437745) * r +
                             3.387132872796366608)
                        / (((((((r * 5226.495278852854561 +
                                 28729.085735721942674) * r + 39307.89580009271061) * r +
                               21213.794301586595867) * r + 5394.1960214247511077) * r +
                             687.1870074920579083) * r + 42.313330701600911252) * r + 1.0);
            }
            else
            { /* closer than 0.075 from {0,1} boundary */

                /* r = min(p, 1-p) < 0.075 */
                if (q > 0)
                    r = R_DT_CIv(p, log_p, lower_tail);/* 1-p */
                else
                    r = p_;/* = R_DT_Iv(p) ^=  p */

                r = Math.Sqrt(-((log_p &&
                         ((lower_tail && q <= 0) || (!lower_tail && q > 0))) ?
                        p : /* else */ Math.Log(r)));
                /* r = sqrt(-log(r))  <==>  min(p, 1-p) = exp( - r^2 ) */
                //#ifdef DEBUG_qnorm
                //REprintf("\t close to 0 or 1: r = %7g\n", r);
                //#endif

                if (r <= 5.0)
                { /* <==> min(p,1-p) >= exp(-25) ~= 1.3888e-11 */
                    r += -1.6;
                    val = (((((((r * 7.7454501427834140764e-4 +
                               .0227238449892691845833) * r + .24178072517745061177) *
                             r + 1.27045825245236838258) * r +
                            3.64784832476320460504) * r + 5.7694972214606914055) *
                          r + 4.6303378461565452959) * r +
                         1.42343711074968357734)
                        / (((((((r *
                                 1.05075007164441684324e-9 + 5.475938084995344946e-4) *
                                r + .0151986665636164571966) * r +
                               .14810397642748007459) * r + .68976733498510000455) *
                             r + 1.6763848301838038494) * r +
                            2.05319162663775882187) * r + 1.0);
                }
                else
                { /* very close to  0 or 1 */
                    r += -5.0;
                    val = (((((((r * 2.01033439929228813265e-7 +
                               2.71155556874348757815e-5) * r +
                              .0012426609473880784386) * r + .026532189526576123093) *
                            r + .29656057182850489123) * r +
                           1.7848265399172913358) * r + 5.4637849111641143699) *
                         r + 6.6579046435011037772)
                        / (((((((r *
                                 2.04426310338993978564e-15 + 1.4215117583164458887e-7) *
                                r + 1.8463183175100546818e-5) * r +
                               7.868691311456132591e-4) * r + .0148753612908506148525)
                             * r + .13692988092273580531) * r +
                            .59983220655588793769) * r + 1.0);
                }

                if (q < 0.0)
                    val = -val;
                /* return (q >= 0.)? r : -r ;*/
            }
            return mu + sigma * val;
        }

        //functions below were found and copied from: https://svn.r-project.org/R/trunk/src/nmath/dpq.h
        //functions are used in the qnorm code above and need to be defined
        private double R_Q_P01_boundaries(double p, double _LEFT_, double _RIGHT_, bool log_p, bool lower_tail)
        {
            if (log_p)
            {
                if (p > 0)
                    return double.NaN;
                if (p == 0) /* upper bound*/
                    return lower_tail ? _RIGHT_ : _LEFT_;
                if (p == Double.NegativeInfinity)
                    return lower_tail ? _LEFT_ : _RIGHT_;
            }
            else
            { /* !log_p */
                if (p < 0 || p > 1)
                    return double.NaN;
                if (p == 0)
                    return lower_tail ? _LEFT_ : _RIGHT_;
                if (p == 1)
                    return lower_tail ? _RIGHT_ : _LEFT_;
            }
            return double.NaN;
        }
        private double R_DT_qIv(double p, bool log_p, bool lower_tail)
        {
            return log_p ? (lower_tail ? Math.Exp(p) : -(Math.Exp(p) - 1.0))
                           : R_D_Lval(p, lower_tail);
        }
        private double R_DT_CIv(double p, bool log_p, bool lower_tail)
        {
            return log_p ? (lower_tail ? -(Math.Exp(p) - 1.0) : Math.Exp(p))
                           : R_D_Cval(p, lower_tail);
        }
        private double R_D_Lval(double p, bool lower_tail)
        {
            return lower_tail ? (p) : (0.5 - (p) + 0.5);	/*  p  */
        }
        private double R_D_Cval(double p, bool lower_tail)
        {
            return lower_tail ? (0.5 - (p) + 0.5) : (p);	/*  1 - p */
        }
    }
}
