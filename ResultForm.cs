using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KsushaMatStat
{
    public partial class ResultForm : Form
    {
        private const int lblTop = 70;

        const int lblPadY = 15;
        const int lblPadX = 15;

        private const int lblH = 20;
        private const int lblW = 100;


        public ResultForm(List<StrategyCalculationResult> results, int bestI = -1)
        {
            InitializeComponent();

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];

                Label lblStrat = new Label();
                lblStrat.Top = lblTop + lblPadY + i * lblH;
                lblStrat.Left = lblPadX;
                lblStrat.Width = lblW;
                lblStrat.Height = lblH;
                lblStrat.Text = $"({result.s.s}, {result.s.S})";

                this.Controls.Add(lblStrat);

                Label lblSrOb = new Label();
                lblSrOb.Top = lblTop + lblPadY + i * lblH;
                lblSrOb.Left = lblPadX + (lblPadX + lblW);
                lblSrOb.Width = lblW;
                lblSrOb.Height = lblH;
                lblSrOb.Text = result.Sob.ToString();

                this.Controls.Add(lblSrOb);


                Label lblSrp = new Label();
                lblSrp.Top = lblTop + lblPadY + i * lblH;
                lblSrp.Left = lblPadX + 2 * (lblPadX + lblW);
                lblSrp.Width = lblW;
                lblSrp.Height = lblH;
                lblSrp.Text = result.Sp.ToString();

                this.Controls.Add(lblSrp);


                Label lblShr = new Label();
                lblShr.Top = lblTop + lblPadY + i * lblH;
                lblShr.Left = lblPadX + 3 * (lblPadX + lblW);
                lblShr.Width = lblW;
                lblShr.Height = lblH;
                lblShr.Text = result.Sh.ToString();

                this.Controls.Add(lblShr);

                Label lblSneh = new Label();
                lblSneh.Top = lblTop + lblPadY + i * lblH;
                lblSneh.Left = lblPadX + 4 * (lblPadX + lblW);
                lblSneh.Width = lblW;
                lblSneh.Height = lblH;
                lblSneh.Text = result.Sd.ToString();

                this.Controls.Add(lblSneh);

                if (i == bestI)
                {
                    lblStrat.BackColor = Color.Chartreuse;
                    lblSrOb.BackColor = Color.Chartreuse;
                    lblSrp.BackColor = Color.Chartreuse;
                    lblShr.BackColor = Color.Chartreuse;
                    lblSneh.BackColor = Color.Chartreuse;
                }
            }
        }

        private void ResultForm_Load(object sender, EventArgs e)
        {
        }
    }
}