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
                if (functionId == 1) //YY: Krewski function
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
                hashBeta.Add(7.13613816576479E-06, 0);
                hashBeta.Add(0.0036732089091998, 0);
                hashBeta.Add(0.00445680692020188, 0.00141010752630656);
                hashBeta.Add(0.00490614415212154, 0.00216329625033075);
                hashBeta.Add(0.00513078457688121, 0.00267355689078526);
                hashBeta.Add(0.00542655575787463, 0.0031651718314315);
                hashBeta.Add(0.00546694384237022, 0.00346327402913653);
                hashBeta.Add(0.00552951369267584, 0.003731723062801);
                hashBeta.Add(0.00556627826775864, 0.00394675441423374);
                hashBeta.Add(0.00555465166497886, 0.00409327284961263);
                hashBeta.Add(0.00554233509918154, 0.00421452086875392);
                hashBeta.Add(5.43742314304533E-05, 0);
                hashBeta.Add(0.0123802435238816, 0);
                hashBeta.Add(0.0108210486134889, 0.0054294734828181);
                hashBeta.Add(0.0094237284728386, 0.00598752502941192);
                hashBeta.Add(0.00816478237486962, 0.00532220297348843);
                hashBeta.Add(0.00674688827914245, 0.00450411971932308);
                hashBeta.Add(0.00541381792529853, 0.00372255867693596);
                hashBeta.Add(0.00421834468434003, 0.00299169801430789);
                hashBeta.Add(0.00340322296639642, 0.00249384362513289);
                hashBeta.Add(0.00274867331790437, 0.0020774115146839);
                hashBeta.Add(0.00227740522255022, 0.00177158436777911);
                hashBeta.Add(0.000790363071197945, 0);
                hashBeta.Add(0.0094123414522043, 0.00107132515154446);
                hashBeta.Add(0.0393637850490632, 0.01151292303727);
                hashBeta.Add(0.0465500411347408, 0.0133300996590178);
                hashBeta.Add(0.0469786411087351, 0.0136421139415226);
                hashBeta.Add(0.0428718371979271, 0.0128782833195303);
                hashBeta.Add(0.0348947014832796, 0.0109848591032032);
                hashBeta.Add(0.030922712846204, 0.0103028936725404);
                hashBeta.Add(0.0255963105649377, 0.0090708225889078);
                hashBeta.Add(0.0211964854197365, 0.00800488123179096);
                hashBeta.Add(0.0164031706762413, 0.00662840624585554);
                hashBeta.Add(0.0134239981190811, 0.00580715411069759);
                hashBeta.Add(0.0112047100454802, 0.00518122348064701);
                hashBeta.Add(0.00902577558288467, 0.00444591405367241);
                hashBeta.Add(0.00826969172914026, 0.00431691693841111);
                hashBeta.Add(0.00747568966060573, 0.00411012769444771);
                hashBeta.Add(0.00745007384163233, 0.00428102156749706);
                hashBeta.Add(0.00727124694612462, 0.00433575946581352);
                hashBeta.Add(0.00745586035271641, 0.00458407946876416);
                hashBeta.Add(0.00712946369227998, 0.00450192724687201);
                hashBeta.Add(5.7586918558646E-06, 0);
                hashBeta.Add(0.0043517369975323, 0);
                hashBeta.Add(0.00582952711501686, 0.00120253789065484);
                hashBeta.Add(0.00667183065623108, 0.0021430162067018);
                hashBeta.Add(0.00715650648314438, 0.0029387140064955);
                hashBeta.Add(0.00730183723720923, 0.00355130828368165);
                hashBeta.Add(0.00744894479432709, 0.00411579947723721);
                hashBeta.Add(0.0075860924363396, 0.00461509676942216);
                hashBeta.Add(0.00777204556694381, 0.00509299098161267);
                hashBeta.Add(0.00766809551937113, 0.00532727208671322);
                hashBeta.Add(0.00762892262191379, 0.00555593970012269);


            }
            else //97.5
            {
                hashBeta.Add(7.13613816576479E-06, 0);
                hashBeta.Add(0.0036732089091998, 0.00982108241191249);
                hashBeta.Add(0.00445680692020188, 0.00821776863251851);
                hashBeta.Add(0.00490614415212154, 0.0078765453782247);
                hashBeta.Add(0.00513078457688121, 0.00766662396492881);
                hashBeta.Add(0.00542655575787463, 0.00773336687331194);
                hashBeta.Add(0.00546694384237022, 0.00753274315193353);
                hashBeta.Add(0.00552951369267584, 0.00740630312652637);
                hashBeta.Add(0.00556627826775864, 0.00728064846271638);
                hashBeta.Add(0.00555465166497886, 0.00713196229962045);
                hashBeta.Add(0.00554233509918154, 0.00700547484744641);
                hashBeta.Add(5.43742314304533E-05, 0);
                hashBeta.Add(0.0123802435238816, 0.0322686217992467);
                hashBeta.Add(0.0108210486134889, 0.0173688387610989);
                hashBeta.Add(0.0094237284728386, 0.016717725828903);
                hashBeta.Add(0.00816478237486962, 0.0151691165860646);
                hashBeta.Add(0.00674688827914245, 0.0121094640323429);
                hashBeta.Add(0.00541381792529853, 0.00908041938953055);
                hashBeta.Add(0.00421834468434003, 0.00663004237151571);
                hashBeta.Add(0.00340322296639642, 0.00506324432575931);
                hashBeta.Add(0.00274867331790437, 0.00388809567823601);
                hashBeta.Add(0.00227740522255022, 0.00308900334468149);
                hashBeta.Add(0.000790363071197945, 0.00672123314866158);
                hashBeta.Add(0.0094123414522043, 0.022971118683451);
                hashBeta.Add(0.0393637850490632, 0.0624042407881356);
                hashBeta.Add(0.0465500411347408, 0.0699607900636253);
                hashBeta.Add(0.0469786411087351, 0.0678167268968709);
                hashBeta.Add(0.0428718371979271, 0.0599138436557351);
                hashBeta.Add(0.0348947014832796, 0.0474016265363874);
                hashBeta.Add(0.030922712846204, 0.0411082986282463);
                hashBeta.Add(0.0255963105649377, 0.0335014516081885);
                hashBeta.Add(0.0211964854197365, 0.0274287271232458);
                hashBeta.Add(0.0164031706762413, 0.0209896971456108);
                hashBeta.Add(0.0134239981190811, 0.0169868468464245);
                hashBeta.Add(0.0112047100454802, 0.0140274169022716);
                hashBeta.Add(0.00902577558288467, 0.0111868967210222);
                hashBeta.Add(0.00826969172914026, 0.0101582838914336);
                hashBeta.Add(0.00747568966060573, 0.00911103817794254);
                hashBeta.Add(0.00745007384163233, 0.00902004631997956);
                hashBeta.Add(0.00727124694612462, 0.00875263124862092);
                hashBeta.Add(0.00745586035271641, 0.00893140944854513);
                hashBeta.Add(0.00712946369227998, 0.00850280566129078);
                hashBeta.Add(5.7586918558646E-06, 0);
                hashBeta.Add(0.0043517369975323, 0.0112444111977684);
                hashBeta.Add(0.00582952711501686, 0.0103277554704577);
                hashBeta.Add(0.00667183065623108, 0.0101580110773699);
                hashBeta.Add(0.00715650648314438, 0.00999251028454225);
                hashBeta.Add(0.00730183723720923, 0.00962781676975382);
                hashBeta.Add(0.00744894479432709, 0.00941547764659079);
                hashBeta.Add(0.0075860924363396, 0.0092882993258514);
                hashBeta.Add(0.00777204556694381, 0.00928242486739316);
                hashBeta.Add(0.00766809551937113, 0.0089804721710377);
                hashBeta.Add(0.00762892262191379, 0.00878928721384936);


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
