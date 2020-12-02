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
        private InputData inputData;
        public Form1()
        {
            InitializeComponent();
            this.inputData = new InputData(15, 67, 36, 1, 4, 1, new List<StrategyData>()
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
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Calculator calc = new Calculator();
            var res =  calc.Calculate(this.inputData);
            MessageBox.Show("ok");
        }
    }
}