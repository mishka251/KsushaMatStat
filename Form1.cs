using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KsushaMatStat
{
    public class StrategyData
    {
        public double s; //критический уровень запасов
        public double S; //максимальный уровень запасов

        public double CriticalStocksCount
        {
            get => this.s;
            set => this.s = value;
        }

        public double MaxStocksCount
        {
            get => this.S;
            set => this.S = value;
        }
        
        public StrategyData(double s, double S)
        {
            this.s = s;
            this.S = S;
        }
    }

    public class InputData
    {
        public int N; //длительность моделируемого периода (мес.)
        public int i0; //начальный уровень запасов (шт.)
        public double i; // затраты на заказ единицы продукции (тыс. руб.)
        public double К; // затраты на заказ (тыс. руб.)
        public double h; // затраты на хранение единицы продукции в день (тыс. руб.)
        public double w; // издержки, связанные с отложенными поставками (тыс. руб.)
        public List<StrategyData> s; //стратегии управления запасами
        public int ModelingPeriod
        {
            get => N;
            set => N = value;
        }

        
        public int InitialStocksCount
        {
            get => i0;
            set => i0 = value;
        }

        public double OrderStockItemPrice
        {
            get => i;
            set => i = value;
        }

        public double OrderPrice
        {
            get => К;
            set => К = value;
        }

        public double StoreStockItemPrice
        {
            get => h;
            set => h = value;
        }

        public double DeferredDeliveryPrice
        {
            get => w;
            set => w = value;
        }

        public List<StrategyData> Strategies
        {
            get => s;
            set => s = value;
        }

        public InputData(int n, int i0, double i, double к, double h, double w, List<StrategyData> s)
        {
            N = n;
            this.i0 = i0;
            this.i = i;
            К = к;
            this.h = h;
            this.w = w;
            this.s = s;
        }
    }

    public partial class Form1 : Form
    {
        /// <summary>
        /// Исходные данные
        /// </summary>
        private InputData inputData;

        /// <summary>
        /// Список ткстбоксов для ввода стратегий
        /// </summary>
        private List<TextBox> StrategyTBs;
        private List<TextBox> StrategyTBS;
        private List<Button> StrategyRmBtns;

        //параметры для генераии текстбоксов
        private const int tbH = 30;
        private const int tbW = 60;

        private const int tbPad = 20;
        private const int tbTopPad = 30;

        private int n = 0;
        
        public Form1()
        {
            InitializeComponent();
            //исходдные данные "по умолчанию"
            this.inputData = new InputData(36, 15, 1, 67, 1, 4, new List<StrategyData>()
            {
                new StrategyData(15, 50),
                new StrategyData(15, 60),

                new StrategyData(20, 40),
                new StrategyData(20, 60),
                new StrategyData(20, 80),
                new StrategyData(20, 100),

                new StrategyData(40, 50),
                new StrategyData(40, 60),
                new StrategyData(40, 80),
                new StrategyData(40, 100),

                new StrategyData(60, 70),
                new StrategyData(60, 80),
                new StrategyData(60, 100),
            });

            //записываем их на форму
            this.tbN.Text = this.inputData.N.ToString();
            this.TBi0.Text = this.inputData.i0.ToString();
            this.tbI.Text = this.inputData.i.ToString();
            this.tbK.Text = this.inputData.К.ToString();
            this.tbh.Text = this.inputData.h.ToString();
            this.tbw.Text = this.inputData.w.ToString();

            
            StrategyTBs= new List<TextBox>();
            StrategyTBS= new List<TextBox>();
            StrategyRmBtns = new List<Button>();
            
            
            for (int i = 0; i < inputData.Strategies.Count; i++)
            {
                TextBox tbs = new TextBox();
                tbs.Top = tbTopPad+ tbPad + (tbH+tbPad) * i;
                tbs.Height = tbH;
                tbs.Width = tbW;
                tbs.Left = tbPad;
                tbs.Text = inputData.Strategies[i].s.ToString();
                this.StrategyTBs.Add(tbs);
                
                TextBox tbS = new TextBox();
                tbS.Top = tbTopPad+tbPad + (tbH+tbPad) * i;
                tbS.Height = tbH;
                tbS.Width = tbW;
                tbS.Left = tbPad + tbW + tbPad;
                tbS.Text = inputData.Strategies[i].S.ToString();
                this.StrategyTBS.Add(tbS);
                
                Button btn = new Button();
                btn.Text = "-";
                btn.Top = tbTopPad+tbPad + (tbH+tbPad) * i;
                btn.Height = tbH;
                btn.Width = 20;
                btn.Left = tbPad + tbW + tbPad+tbW+tbPad;

                btn.Click += (sender, args) =>
                {
                    int index = this.StrategyRmBtns.IndexOf(sender as Button);
                    
                    this.panel1.Controls.Remove(this.StrategyRmBtns[index]);
                    this.panel1.Controls.Remove(this.StrategyTBs[index]);
                    this.panel1.Controls.Remove(this.StrategyTBS[index]);
                    
                    this.StrategyRmBtns.RemoveAt(index);
                    this.inputData.Strategies.RemoveAt(index);
                    this.StrategyTBs.RemoveAt(index);
                    this.StrategyTBS.RemoveAt(index);
                };

                n = inputData.Strategies.Count;
                
                this.StrategyRmBtns.Add(btn);
                
                
                this.panel1.Controls.Add(tbS);
                this.panel1.Controls.Add(tbs);
                this.panel1.Controls.Add(btn);
            }
            
            
        }

        /// <summary>
        /// Считываем введенные данные в исходные
        /// </summary>
        void ParseData()
        {
            this.inputData.N = int.Parse(this.tbN.Text);
            this.inputData.i0 = int.Parse(this.TBi0.Text);
            this.inputData.i = int.Parse(this.tbI.Text);
            this.inputData.К = int.Parse(this.tbK.Text);
            this.inputData.h = int.Parse(this.tbh.Text);
            this.inputData.w = int.Parse(this.tbw.Text);

            for (int i = 0; i < inputData.Strategies.Count; i++)
            {
                this.inputData.Strategies[i].s = int.Parse(this.StrategyTBs[i].Text); 
                this.inputData.Strategies[i].S = int.Parse(this.StrategyTBS[i].Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ввод, расчёты, вывож
            this.ParseData();
            Calculator calc = new Calculator();
            var res =  calc.Calculate(this.inputData);
            //MessageBox.Show("ok");

            int bestInd = 0;
            for(int i =1; i<res.Count; i++)
            {
                if (res[i].Sob < res[bestInd].Sob)
                {
                    bestInd = i;
                }
            }

            ResultForm rf = new ResultForm(res, bestInd);
            rf.Show();
        }

        //создание текстбоксов для добавления стратегии
        private void button2_Click(object sender, EventArgs e)
        {
            this.inputData.Strategies.Add(new StrategyData(0, 0));
            int _n = this.inputData.Strategies.Count - 1;
            TextBox tbs = new TextBox();
            tbs.Top = tbTopPad+ tbPad + (tbH+tbPad) * n;
            tbs.Height = tbH;
            tbs.Width = tbW;
            tbs.Left = tbPad;
            tbs.Text = inputData.Strategies[_n].s.ToString();
            this.StrategyTBs.Add(tbs);
                
            TextBox tbS = new TextBox();
            tbS.Top = tbTopPad+tbPad + (tbH+tbPad) * n;
            tbS.Height = tbH;
            tbS.Width = tbW;
            tbS.Left = tbPad + tbW + tbPad;
            tbS.Text = inputData.Strategies[_n].S.ToString();
            this.StrategyTBS.Add(tbS);
            
            Button btn = new Button();
            btn.Text = "-";
            btn.Top = tbTopPad+tbPad + (tbH+tbPad) * n;
            btn.Height = tbH;
            btn.Width = 20;
            btn.Left = tbPad + tbW + tbPad+tbW+tbPad;

            //удаление стратегии с формы
            btn.Click += (sender, args) =>
            {
                int index = this.StrategyRmBtns.IndexOf(sender as Button);
                
                this.panel1.Controls.Remove(this.StrategyRmBtns[index]);
                this.panel1.Controls.Remove(this.StrategyTBs[index]);
                this.panel1.Controls.Remove(this.StrategyTBS[index]);

                this.StrategyRmBtns.RemoveAt(index);
                this.inputData.Strategies.RemoveAt(index);
                this.StrategyTBs.RemoveAt(index);
                this.StrategyTBS.RemoveAt(index);
            };
                
            this.StrategyRmBtns.Add(btn);
                
                
            this.panel1.Controls.Add(tbS);
            this.panel1.Controls.Add(tbs);
            this.panel1.Controls.Add(btn);

            n++;

        }
    }
}