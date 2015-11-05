using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class EditDistributionValues : FormBase
    {
        PlotModel distModel = new PlotModel();

        private string _distributionName;
        private HealthImpact _healthImpactDistribution;
        public HealthImpact HealthImpactDistribution
        {
            get { return _healthImpactDistribution; }
            set { _healthImpactDistribution = value; }
        }
        public EditDistributionValues(string distributionName, HealthImpact healthImpact)
        {
            InitializeComponent();
            _distributionName = distributionName;
            _healthImpactDistribution = healthImpact;
        }

        private void EditDistributionValues_Load(object sender, EventArgs e)
        {
            try
            {
                double mean = Convert.ToDouble(_healthImpactDistribution.Beta);
                txtMeanValue.Text = _healthImpactDistribution.Beta;
                txtParameter1.Text = _healthImpactDistribution.BetaParameter1;

                distModel.PlotAreaBackground = OxyColors.White;
                distModel.Padding = new OxyThickness(9);

                if (_distributionName == "Normal")
                {
                    double sd = Convert.ToDouble(_healthImpactDistribution.BetaParameter1);
                    double x0 = mean - (sd * 4); 
                    double x1 = mean + (sd * 4); 

                    lblPDF.Text = _distributionName +" Dist.:";
                    lblNotesContext.Text = "The Normal distribution has two parameters - the mean,\nmu, and the standard deviation, sigma.";
                    lblParameter2.Visible = false;
                    txtParameter2.Visible = false;
                    lblParameter1.Text = "Sigma:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    Image normal = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Normal PDF.png");
                    pictureBox1.Image = normal;

                    distModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Left,
                        MinimumPadding = 0.1,
                        MaximumPadding = 0.1,
                        // MajorStep = 0.2,
                        // MinorStep = 0.05,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Title = "Standard Deviation",
                        AxisTitleDistance = 15,

                    });

                    distModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Bottom,
                        // MajorStep = 1,
                        // MinorStep = 0.25,
                        MaximumPadding = 0,
                        MinimumPadding = 0,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Title = "Mean",
                        AxisTitleDistance = 10,
                    });

                    LineSeries norm = (CreateNormalSeries(x0, x1, mean, (sd*sd)));
                    distModel.Series.Add(norm);
                    this.plot1.Model = distModel;
                }
                if (_distributionName == "Triangular")
                {
                    double min = Convert.ToDouble(_healthImpactDistribution.BetaParameter1);
                    double max = Convert.ToDouble(_healthImpactDistribution.BetaParameter2);
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Triangular distribution has three parameters-the \nminimum value(a), the maximum value(b), and the most \nlikely value(c).BenMAP uses the mean value, the minimum, \nand the maximum to calculate the most likely value.";
                    lblParameter1.Text = "a:";
                    lblParameter2.Text = "b:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Triangular = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Triangular PDF.png");
                    pictureBox1.Image = Triangular;
                    Triangular = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\800px-Triangular_distribution_PMF.png");
                    pictureBox2.Image = Triangular;

                    distModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Left,
                        MinimumPadding = 0,
                        MaximumPadding = 0.1,
                        // MajorStep = 0.2,
                        // MinorStep = 0.05,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Title = "2 / (b - a)",
                        AxisTitleDistance = 15,

                    });

                    distModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Bottom,
                        // MajorStep = 1,
                        // MinorStep = 0.25,
                        MaximumPadding = 0.1,
                        MinimumPadding = 0.1,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Title = "x",
                        AxisTitleDistance = 10,
                        TextColor = OxyColors.Transparent,
                    });

                    double c = CreateTriangularSeries(min, max, mean);
                    LineSeries tri = new LineSeries() { Color = OxyColors.ForestGreen, StrokeThickness = 3 };

                    tri.Points.Add(new DataPoint(min, 0));
                    tri.Points.Add(new DataPoint(c, (2.0 / Math.Abs(max - min))));
                    tri.Points.Add(new DataPoint(max, 0));

                    distModel.Series.Add(tri);
                    this.plot1.Model = distModel;
                }

                if (_distributionName == "Poisson")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Poisson distribution has a single parameter, lambda.";
                    lblParameter1.Text = "lambda:";
                    lblParameter2.Visible = false;
                    txtParameter2.Visible = false;
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    Image Poisson = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Poisson PDF.png");
                    pictureBox1.Image = Poisson;
                    Poisson = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\360px-Poisson_pmf_svg.png");
                    pictureBox2.Image = Poisson;
                }
                if (_distributionName == "Binomial")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Binomial distribution has two parameters, n and p.";
                    lblParameter1.Text = "n:";
                    lblParameter2.Text = "p:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Binomial = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Binomial PDF.png");
                    pictureBox1.Image = Binomial;
                    Binomial = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\640px-Binomial_distribution_cdf_svg.png");
                    pictureBox2.Image = Binomial;
                }
                if (_distributionName == "LogNormal")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The LogNormal distribution has two parameters-the mean of \nthe corresponding Normal distribution, mu, and the standard \ndeviation of the corresponding Normal distribution,sigma.\nNote that the given PDF is for the corresponding Normal.";
                    lblParameter1.Text = "mu:";
                    lblParameter2.Text = "sigma:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image LogNormal = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\LogNormal PDF.png");
                    pictureBox1.Image = LogNormal;
                    LogNormal = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\593px-Some_log-normal_distributions_svg.png");
                    pictureBox2.Image = LogNormal;
                }
                if (_distributionName == "Uniform")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Uniform distribution has two parameters, A and B, \nWhich define the interval on which the distribution is \ndefined.";
                    lblParameter1.Text = "A:";
                    lblParameter2.Text = "B:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Uniform = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Uniform PDF.png");
                    pictureBox1.Image = Uniform;
                    Uniform = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\800px-Uniform_distribution_PDF.png");
                    pictureBox2.Image = Uniform;
                }
                if (_distributionName == "Exponential")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Exponential distribution has one parameter, mu.";
                    lblParameter1.Text = "mu:";
                    lblParameter2.Visible = false;
                    txtParameter2.Visible = false;
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    Image Exponential = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Exponential PDF.png");
                    pictureBox1.Image = Exponential;
                    Exponential = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\2000px-Exponential_pdf_svg.png");
                    pictureBox2.Image = Exponential;
                }
                if (_distributionName == "Geometric")
                {
                    lblPDF.Text = "   " + _distributionName + " PDF:";
                    lblNotesContext.Text = "The Geometric distribution has one parameter, p.";
                    lblParameter1.Text = "p:";
                    lblParameter2.Visible = false;
                    txtParameter2.Visible = false;
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    Image Geometric = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Geometric PDF.png");
                    pictureBox1.Image = Geometric;
                    Geometric = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\675px-Geometric_pmf_svg.png");
                    pictureBox2.Image = Geometric;
                }
                if (_distributionName == "Weibull")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Weibull distribution has two parameters, alpha and beta.";
                    lblParameter1.Text = "alpha:";
                    lblParameter2.Text = "beta:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Weibull = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Weibull PDF.png");
                    pictureBox1.Image = Weibull;
                    Weibull = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\540px-Weibull_PDF_svg.png");
                    pictureBox2.Image = Weibull;
                }
                if (_distributionName == "Gamma")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Gamma distribution has two parameters, a and b.";
                    lblParameter1.Text = "a:";
                    lblParameter2.Text = "b:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Gamma = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Gamma PDF.png");
                    pictureBox1.Image = Gamma;
                    Gamma = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\800px-Gamma_distribution_pdf_svg.png");
                    pictureBox2.Image = Gamma;
                }
                if (_distributionName == "Logistic")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Logistic distribution has two parameters, m and b.";
                    lblParameter1.Text = "m:";
                    lblParameter2.Text = "b:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Logistic = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Logistic PDF.png");
                    pictureBox1.Image = Logistic;
                    Logistic = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\480px-Logisticpdfunction_svg.png");
                    pictureBox2.Image = Logistic;
                }
                if (_distributionName == "Beta")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Logistic distribution has two parameters, a and b.";
                    lblParameter1.Text = "a:";
                    lblParameter2.Text = "b:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Beta = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Beta PDF.png");
                    pictureBox1.Image = Beta;
                    Beta = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\639px-Beta_distribution_pdf_svg.png");
                    pictureBox2.Image = Beta;
                }
                if (_distributionName == "Pareto")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Pareto distribution has two parameters, a and b.";
                    lblParameter1.Text = "a:";
                    lblParameter2.Text = "b:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Pareto = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Pareto PDF.png");
                    pictureBox1.Image = Pareto;
                    Pareto = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\800px-Pareto_distributionPDF.png");
                    pictureBox2.Image = Pareto;
                }
                if (_distributionName == "Cauchy")
                {
                    lblPDF.Text = _distributionName + " PDF:";
                    lblNotesContext.Text = "The Cauchy distribution has two parameters, b and m.";
                    lblParameter1.Text = "b:";
                    lblParameter2.Text = "m:";
                    txtParameter1.Text = _healthImpactDistribution.BetaParameter1;
                    txtParameter2.Text = _healthImpactDistribution.BetaParameter2;
                    Image Cauchy = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\Cauchy PDF.png");
                    pictureBox1.Image = Cauchy;
                    Cauchy = Image.FromFile(Application.StartupPath + @"\Resources\DistributionFormula\360px-Cauchy_pdf_svg.png");
                    pictureBox2.Image = Cauchy;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private static LineSeries CreateNormalSeries(double x0, double x1, double mean, double variance, int n = 500000)
        {
            var ls = new LineSeries
            {
                Color = OxyColors.ForestGreen,
                StrokeThickness = 3
            };

            for (int i = 0; i < n; i++)
            {
                double x = x0 + (x1 - x0) * i / (n - 1.0);
                double f = 1.0 / Math.Sqrt(2.0 * Math.PI * variance) * Math.Exp(- (x - mean) * (x - mean) / 2.0 / variance);
                ls.Points.Add(new DataPoint(x, f));
            }
            return ls;
        }

        private static double CreateTriangularSeries(double a, double b, double c)
        {
            Random r = new Random();
            double rand = r.NextDouble();
            double F = (c - a) / (b - a);

            if (rand < F) { return (a + Math.Sqrt(rand * (b - a) * (c - a))); }

            else { return (b - Math.Sqrt((1 - rand) * (b - a) * (b - c))); }
        }

        private string _meanValue;
        public string MeanValue
        {
            get { return _meanValue; }
            set { _meanValue = value; }
        }

        private string _parameter1;
        public string Parameter1
        {
            get { return _parameter1; }
            set { _parameter1 = value; }
        }

        private string _parameter2;
        public string Parameter2
        {
            get { return _parameter2; }
            set { _parameter2 = value; }
        }

        public Image image { get; set; }

        private void lblMeanValue_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _meanValue = txtMeanValue.Text;
            _parameter1 = txtParameter1.Text;
            _parameter2 = txtParameter2.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void lblParameter2_Click(object sender, EventArgs e)
        {

        }
    }
}
