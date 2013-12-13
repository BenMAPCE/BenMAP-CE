using System;
using System.Collections.Generic;
using System.Text;

namespace ESIL.Kriging
{
  public static  class Pdist
    {
        #region C:\Program Files\MATLAB\R2009a\toolbox\stats
        //     %PDIST Pairwise distance between observations.
        // Y = PDIST(X) returns a vector Y containing the Euclidean distances
        // between each pair of observations in the N-by-P data matrix X.  Rows of
        // X correspond to observations, columns correspond to variables.  Y is a
        // 1-by-(N*(N-1)/2) row vector, corresponding to the N*(N-1)/2 pairs of
        //observations in X.

        // Y = PDIST(X, DISTANCE) computes Y using DISTANCE.  Choices are:

        //    'euclidean'   - Euclidean distance
        //    'seuclidean'  - Standardized Euclidean distance, each coordinate
        //                    in the sum of squares is inverse weighted by the
        //                    sample variance of that coordinate
        //    'cityblock'   - City Block distance
        //    'mahalanobis' - Mahalanobis distance
        //    'minkowski'   - Minkowski distance with exponent 2
        //    'cosine'      - One minus the cosine of the included angle
        //                    between observations (treated as vectors)
        //    'correlation' - One minus the sample linear correlation between
        //                    observations (treated as sequences of values).
        //    'spearman'    - One minus the sample Spearman's rank correlation
        //                    between observations (treated as sequences of values).
        //    'hamming'     - Hamming distance, percentage of coordinates
        //                    that differ
        //    'jaccard'     - One minus the Jaccard coefficient, the
        //                    percentage of nonzero coordinates that differ
        //    'chebychev'   - Chebychev distance (maximum coordinate difference)
        //    function      - A distance function specified using @, for
        //                    example @DISTFUN

        //A distance function must be of the form

        //      function D = DISTFUN(XI, XJ, P1, P2, ...),

        // taking as arguments a 1-by-P vector XI containing a single row of X, an
        // M-by-P matrix XJ containing multiple rows of X, and zero or more
        // additional problem-dependent arguments P1, P2, ..., and returning an
        // M-by-1 vector of distances D, whose Kth element is the distance between
        //the observations XI and XJ(K,:).

        // Y = PDIST(X, DISTFUN, P1, P2, ...) passes the arguments P1, P2, ...
        // directly to the function DISTFUN.

        // Y = PDIST(X, 'minkowski', P) computes Minkowski distance using the
        // positive scalar exponent P.

        // The output Y is arranged in the order of ((2,1),(3,1),..., (N,1),
        //(3,2),...(N,2),.....(N,N-1)), i.e. the lower left triangle of the full
        // N-by-N distance matrix in column order.  To get the distance between
        // the Ith and Jth observations (I < J), either use the formula
        // Y((I-1)*(N-I/2)+J-I), or use the helper function Z = SQUAREFORM(Y),
        // which returns an N-by-N square symmetric matrix, with the (I,J) entry
        // equal to distance between observation I and observation J.

        //Example:

        //   X = randn(100, 5);                 % some random points
        //   Y = pdist(X, 'euclidean');         % unweighted distance
        //   Wgts = [.1 .3 .3 .2 .1];           % coordinate weights
        //   Ywgt = pdist(X, @weucldist, Wgts); % weighted distance

        //   function d = weucldist(XI, XJ, W) % weighted euclidean distance
        //   d = sqrt((repmat(XI,size(XJ,1),1)-XJ).^2 * W');

        //See also SQUAREFORM, LINKAGE, SILHOUETTE.

        //An example of distance for data with missing elements:

        //   X = randn(100, 5);     % some random points
        //   X(unidrnd(prod(size(X)),1,20)) = NaN; % scatter in some NaNs
        //   D = pdist(X, @naneucdist);

        //   function d = naneucdist(XI, XJ) % euclidean distance, ignoring NaNs
        //   [m,p] = size(XJ);
        //   sqdx = (repmat(XI,m,1) - XJ) .^ 2;
        //   pstar = sum(~isnan(sqdx),2); % correction for missing coords
        //   pstar(pstar == 0) = NaN;
        //   d = sqrt(nansum(sqdx,2) .* p ./ pstar);


        // For a large number of observations, it is sometimes faster to compute
        // the distances by looping over coordinates of the data (though the code
        // is more complicated):

        //   function d = nanhamdist(XI, XJ) % hamming distance, ignoring NaNs
        //   [m,p] = size(XJ);
        //   nesum = zeros(m,1);
        //   pstar = zeros(m,1);
        //   for q = 1:p
        //       notnan = ~(isnan((XI(q)) | isnan(XJ(:,q)));
        //       nesum = nesum + (XI(q) ~= XJ(:,q)) & notnan;
        //       pstar = pstar + notnan;
        //   end
        //   nesum(any() | nans((i+1):n)) = NaN;
        //   Y(k:(k+n-i-1)) = nesum ./ pstar;

        //$Revision: 1.15.4.15 $ $Date: 2008/10/08 18:23:12 $
        #endregion

        public static double[] GetPDist(double[,] X)
        {
            double[] y = new double[1];
            try
            {
                int nargin = 0;
                string dist = "";
                if (nargin < 2)
                {
                    dist = "euc";
                }
                //% Integer/logical/char/anything data may be handled by a caller-defined
                //% distance function, otherwise it is converted to double.  Complex floating
                //% point data must also be handled by a caller-defined distance function.
                int[] tmpValues = Common.Get2DimensionsSize(X);
                int n = tmpValues[0];//行数
                int p = tmpValues[1];//列数
                double[,] tranpose = new double[p, n];
                MatrixCompute.Transpose(X, ref tranpose);
                switch (dist)
                {

                    case "seu"://% Standardized Euclidean weights by coordinate variance
                        break;
                    case "mah"://% Mahalanobis
                        break;
                    case "min"://% Minkowski distance needs a third argument
                        break;
                    case "cos":// % Cosine
                        break;
                    case "cor":// % Correlation
                        break;
                    case "spe":
                        break;
                    default:
                        break;
                }
                // % Call a mex file to compute distances for the standard distance measures
                //% and full real double or single data.
                //Y = pdistmex(X',dist,additionalArg); 用欧氏距离实现X
                tmpValues=Common.Get2DimensionsSize(tranpose);
                n=tmpValues[0];
                p=tmpValues[1];
                y=new double[p*(p-1)/2];
                double tmp=0.0;
                int count=0;
                for (int i = 0; i < p-1; i++)
                {
                    for (int j = i+1; j < p; j++)
                    {
                        tmp = 0.0;
                        for (int row = 0; row < n; row++)
                        {
                            tmp=tmp+Math.Pow((tranpose[row,i]-tranpose[row,j]),2.0);
                        }
                        y[count] = Math.Sqrt(tmp);
                        count++;
                    }
                }
                return y;
            }
            catch (Exception ex)
            {
                Common.LogError(ex);
                return y;
            }
        }



    }//class
}
