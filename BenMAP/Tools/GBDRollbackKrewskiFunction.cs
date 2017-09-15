using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BenMAP
{
    class GBDRollbackFunction
    {
        //This function was initially only for Krewski function and it's updated to be shared by all functions in 2017.
        //first argument is expected to be differential (change) of concentration when rollback is applied
        //example:  Baseline 50, rollback to 35 standard, delta=15
        public GBDRollbackResult GBD_math(int functionId, double[] concDelta, double[] population, double[] incRate, 
            double[] betaMean, double[] betaSe, double[] q0, double[] q1, double[] paraA, double[] paraB, double[] paraC, 
            double[] probDeath, double[] lifeExp)
        {
            try
            {
                double Sum_97_5 = 0;
                double SumResult = 0;
                double Sum_2_5 = 0;
                double SumYll = 0;

                //double beta = 0.005826891;
                //double se = 0.000962763;
                //double IncrateIn = 0.0081120633;
                if (functionId == 1) //Krewski function
                {
                    for (int idx = 0; idx < population.Length; idx++)
                    {

                        double ConcDeltaIn = concDelta[idx];
                        double PopIn = population[idx];
                        double incrate = incRate[idx];
                        double beta = betaMean[idx];
                        double se = betaSe[idx];
                        double probDeathIn = probDeath[idx];
                        double lifeExpIn = lifeExp[idx];

                        double krewski = (1 - (1 / Math.Exp(beta * ConcDeltaIn))) * PopIn * incrate;
                        double krewski_2_5pct = (1 - (1 / Math.Exp(qnorm5(.025, beta, se, true, false) * ConcDeltaIn))) * PopIn * incrate;
                        double krewski_97_5pct = (1 - (1 / Math.Exp(qnorm5(.975, beta, se, true, false) * ConcDeltaIn))) * PopIn * incrate;
                        double yll = krewski * probDeathIn * lifeExpIn;

                        SumResult += krewski;
                        Sum_2_5 += krewski_2_5pct;
                        Sum_97_5 += krewski_97_5pct;
                        SumYll += yll;

                    }
                }
                else if (functionId == 2)//Burnett SCHIF
                {
                    for (int idx = 0; idx < population.Length; idx++)
                    {

                        double PopIn = population[idx];
                        double incrate = incRate[idx];
                        double q0In = q0[idx];
                        double q1In = q1[idx];
                        double beta = betaMean[idx];
                        double se = betaSe[idx];
                        double probDeathIn = probDeath[idx];
                        double lifeExpIn = lifeExp[idx];
                        double A = paraA[idx];
                        double B = paraB[idx];
                        double C = paraC[idx];

                        double schif = 
                            (1 - (1 / Math.Exp(beta * (Math.Log((q1In + A) / A) / (1 + Math.Exp(-(q1In - B) / C)) - (Math.Log((q0In + A) / A) / (1 + Math.Exp(-(q0In - B) / C))))))) * incrate * PopIn;
                        double schif_2_5pct = 
                            (1 - (1 / Math.Exp(qnorm5(0.025, beta, se, true, false) * (Math.Log((q1In + A) / A) / (1 + Math.Exp(-(q1In - B) / C)) - (Math.Log((q0In + A) / A) / (1 + Math.Exp(-(q0In - B) / C))))))) * incrate * PopIn;
                        double schif_97_5pct =
                            (1 - (1 / Math.Exp(qnorm5(0.975, beta, se, true, false) * (Math.Log((q1In + A) / A) / (1 + Math.Exp(-(q1In - B) / C)) - (Math.Log((q0In + A) / A) / (1 + Math.Exp(-(q0In - B) / C))))))) * incrate * PopIn;
                        double yll = schif * probDeathIn * lifeExpIn;

                        SumResult += schif;
                        Sum_2_5 += schif_2_5pct;
                        Sum_97_5 += schif_97_5pct;
                        SumYll += yll;
                    }
                }
                else if (functionId == 3)//Burnett IER
                {
                    for (int idx = 0; idx < population.Length; idx++)
                    {
                        double PopIn = population[idx];
                        double incrate = incRate[idx];
                        double q0In = q0[idx];
                        double q1In = q1[idx];
                        double beta = betaMean[idx];
                        double se = betaSe[idx];
                        double probDeathIn = probDeath[idx];
                        double lifeExpIn = lifeExp[idx];
                        double A = paraA[idx];
                        double B = paraB[idx];
                        double C = paraC[idx];

                        double ier = 0;
                        double ier_2_5pct = 0;
                        double ier_97_5pct = 0;
                        double yll = 0;

                        if (q1In > q0In)
                        {
                            if(q1In <=A || q0In >B)
                            {
                                //ier = 0;
                                //ier_2_5pct = 0;
                                //ier_97_5pct = 0;
                                //yll = 0;
                            }
                            else
                            {
                                ier = (1 - (1 / Math.Exp(beta * (Math.Min(B, q1In) - Math.Max(A, q0In))))) * incrate * PopIn;
                                ier_2_5pct = (1 - (1 / Math.Exp(getIerBeta(beta, true) * (Math.Min(B, q1In) - Math.Max(A, q0In))))) * incrate * PopIn;
                                ier_97_5pct = (1 - (1 / Math.Exp(getIerBeta(beta, false) * (Math.Min(B, q1In) - Math.Max(A, q0In))))) * incrate * PopIn;
                                yll = ier * probDeathIn * lifeExpIn;
                            }
                        }
                        else
                        {
                            if (q0In <= A || q1In > B)
                            {
                                //ier = 0;
                                //ier_2_5pct = 0;
                                //ier_97_5pct = 0;
                                //yll = 0;
                            }
                            else
                            {
                                ier = (1 - (1 / Math.Exp(beta * (Math.Min(B, q0In) - Math.Max(A, q1In))))) * incrate * PopIn;
                                ier_2_5pct = (1 - (1 / Math.Exp(getIerBeta(beta,true) * (Math.Min(B, q0In) - Math.Max(A, q1In))))) * incrate * PopIn;
                                ier_97_5pct = (1 - (1 / Math.Exp(getIerBeta(beta, false) * (Math.Min(B, q0In) - Math.Max(A, q1In))))) * incrate * PopIn;
                                yll = ier * probDeathIn * lifeExpIn;
                            }
                        }

                        SumResult += ier;
                        Sum_2_5 += ier_2_5pct;
                        Sum_97_5 += ier_97_5pct;
                        SumYll += yll;
                    }
                }

                return new GBDRollbackResult(SumResult, Sum_2_5, Sum_97_5, SumYll, 0,0);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
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

        private double qCustom(double beta, bool type)
        {
            double qBeta = 0;

            return qBeta;
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
        
        //hash table for IER beta distribution at 2.5 and 97.5 quantile
        private double getIerBeta(double beta, bool type2_5)
        {
            Hashtable hashBeta = new Hashtable();
            if (type2_5 == true) //2.5
            {
                hashBeta.Add(0.0001746989720168, 0);
                hashBeta.Add(0.0099628574627724, 0.0046905173272351);
                hashBeta.Add(0.002999299363265, 0.0026153072325057);
                hashBeta.Add(0.0013727765917787, 0.0008563325774125);
                hashBeta.Add(0.0007271604882085, 0.0004935294576456);
                hashBeta.Add(0.0004001653924183, 0.000301897811847);
                hashBeta.Add(0.0002315737394352, 0.0002061232982695);
                hashBeta.Add(0.0001407450961642, 0.0001393388111603);
                hashBeta.Add(0.0000895171712709, 0.0000994955293123);
                hashBeta.Add(0.0000593795845358, 0.0000733765587482);
                hashBeta.Add(0.0000408993572129, 0.0000564499159983);
                hashBeta.Add(0.0003420320214482, 0);
                hashBeta.Add(0.0056867606527375, 0.0028436371201648);
                hashBeta.Add(0.0069035425564865, 0.001662755891308);
                hashBeta.Add(0.0043576644814887, 0.0008894791536009);
                hashBeta.Add(0.002640232356909, 0.0007165335278238);
                hashBeta.Add(0.0015299230035909, 0.0005491635632585);
                hashBeta.Add(0.000858293933866, 0.0005532154689951);
                hashBeta.Add(0.000475038145023, 0.0004025595580254);
                hashBeta.Add(0.0002717388716256, 0.0003482782559873);
                hashBeta.Add(0.0001595160667227, 0.0003205547089791);
                hashBeta.Add(0.0000947476876356, 0.0002381738307094);
                hashBeta.Add(0.0000598560906361, 0.0001983088899252);
                hashBeta.Add(0.0000411315331219, 0.0001822940828541);
                hashBeta.Add(0.0000302961420543, 0.0001453414211295);
                hashBeta.Add(0.0000237516927386, 0.0001138170659155);
                hashBeta.Add(0.0000193697763826, 0.0000907315892491);
                hashBeta.Add(0.0000163136561295, 0.0000770659315567);
                hashBeta.Add(0.000014182232965, 0.0000909552925612);
                hashBeta.Add(0.0000125042927383, 0.0000576301820394);
                hashBeta.Add(0.0000111242962128, 0.0000563219373926);
                hashBeta.Add(0.0000224047084452, 0);
                hashBeta.Add(0.0028712182291598, 0.0009649919085427);
                hashBeta.Add(0.0015030339864426, 0.0007639115988628);
                hashBeta.Add(0.0009690068201675, 0.0006129668270759);
                hashBeta.Add(0.0006882013156499, 0.0005116735023408);
                hashBeta.Add(0.0005191310416605, 0.0004213018638617);
                hashBeta.Add(0.0004067215945336, 0.0003659488069493);
                hashBeta.Add(0.0003279716708464, 0.000306841412206);
                hashBeta.Add(0.0002705578529302, 0.0002614257006504);
                hashBeta.Add(0.0002269193486721, 0.0002257980084814);
                hashBeta.Add(0.0001932649020114, 0.0001993164679086);
                hashBeta.Add(0.0000180539988384, 0);
                hashBeta.Add(0.0034815543314637, 0.0006860732548813);
                hashBeta.Add(0.0019599114625712, 0.0007296404538281);
                hashBeta.Add(0.0012901405754414, 0.0006814740502646);
                hashBeta.Add(0.0009235421233335, 0.000616690345175);
                hashBeta.Add(0.0006976793250837, 0.000549454633747);
                hashBeta.Add(0.0005462040182535, 0.0004849026534669);
                hashBeta.Add(0.0004389884093573, 0.000425905230879);
                hashBeta.Add(0.0003614089879854, 0.0003743544519777);
                hashBeta.Add(0.0003036714490889, 0.0003302387451006);
                hashBeta.Add(0.0002593847805792, 0.0002926190961858);
            }
            else //97.5
            {
                hashBeta.Add(0.0001746989720168, 0.0031303227603919);
                hashBeta.Add(0.0099628574627724, 0.0127150671559427);
                hashBeta.Add(0.002999299363265, 0.0042905564703401);
                hashBeta.Add(0.0013727765917787, 0.0033385409928493);
                hashBeta.Add(0.0007271604882085, 0.0014309639311474);
                hashBeta.Add(0.0004001653924183, 0.0004586463171905);
                hashBeta.Add(0.0002315737394352, 0.0001032042852028);
                hashBeta.Add(0.0001407450961642, 0.0000470729051585);
                hashBeta.Add(0.0000895171712709, 0.0000211480444596);
                hashBeta.Add(0.0000593795845358, 0.0000065638344244);
                hashBeta.Add(0.0000408993572129, 0.000005491342452);
                hashBeta.Add(0.0003420320214482, 0.0034592181869957);
                hashBeta.Add(0.0056867606527375, 0.006926336122836);
                hashBeta.Add(0.0069035425564865, 0.0109879418424677);
                hashBeta.Add(0.0043576644814887, 0.0043493487649124);
                hashBeta.Add(0.002640232356909, 0.0029943155792618);
                hashBeta.Add(0.0015299230035909, 0.0011451581896103);
                hashBeta.Add(0.000858293933866, 0.0005037400312794);
                hashBeta.Add(0.000475038145023, 0.000252039470254);
                hashBeta.Add(0.0002717388716256, 0.0001295369002692);
                hashBeta.Add(0.0001595160667227, 0.0000947658180712);
                hashBeta.Add(0.0000947476876356, 0.0000354796765569);
                hashBeta.Add(0.0000598560906361, 0.0000075876895315);
                hashBeta.Add(0.0000411315331219, 0.000000683714681);
                hashBeta.Add(0.0000302961420543, 0.000000233199978);
                hashBeta.Add(0.0000237516927386, 0.0000000013827694);
                hashBeta.Add(0.0000193697763826, 0.0000000000238694);
                hashBeta.Add(0.0000163136561295, 0.0000010386763785);
                hashBeta.Add(0.000014182232965, 0.0000000000000129);
                hashBeta.Add(0.0000125042927383, 0.0000008895898014);
                hashBeta.Add(0.0000111242962128, 0.0000003542999533);
                hashBeta.Add(0.0000224047084452, 0.0003204795900078);
                hashBeta.Add(0.0028712182291598, 0.004994233706741);
                hashBeta.Add(0.0015030339864426, 0.0021423827245021);
                hashBeta.Add(0.0009690068201675, 0.0012000729094458);
                hashBeta.Add(0.0006882013156499, 0.0008505323519191);
                hashBeta.Add(0.0005191310416605, 0.0006389280590936);
                hashBeta.Add(0.0004067215945336, 0.0004309262112891);
                hashBeta.Add(0.0003279716708464, 0.0003718030807187);
                hashBeta.Add(0.0002705578529302, 0.0003071763368561);
                hashBeta.Add(0.0002269193486721, 0.0002504786864135);
                hashBeta.Add(0.0001932649020114, 0.0001916502215028);
                hashBeta.Add(0.0000180539988384, 0.000305626555173);
                hashBeta.Add(0.0034815543314637, 0.005802311180419);
                hashBeta.Add(0.0019599114625712, 0.0025627860854979);
                hashBeta.Add(0.0012901405754414, 0.0015419485080003);
                hashBeta.Add(0.0009235421233335, 0.0010164488392931);
                hashBeta.Add(0.0006976793250837, 0.000722175496919);
                hashBeta.Add(0.0005462040182535, 0.000554344532705);
                hashBeta.Add(0.0004389884093573, 0.0004324796524323);
                hashBeta.Add(0.0003614089879854, 0.0003515169611627);
                hashBeta.Add(0.0003036714490889, 0.0002944901455835);
                hashBeta.Add(0.0002593847805792, 0.0002431654993949);



            }
            return Convert.ToDouble(hashBeta[beta]);
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
