using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace KsushaMatStat
{
    public class DiskretRandomValue
    {
        public double value;
        public double p;
    }

    public class StrategyImitationResult
    {
        public SortedDictionary<double, double> It;
        public double q;
        public double k;
        public StrategyData s;
    }

    public class StrategyCalculationResult
    {
        public double Sp;
        public double Sh;
        public double Sd;

        public double Sob;
        public StrategyData s;

        public StrategyCalculationResult(StrategyImitationResult result, InputData data, StrategyData s)
        {
            Sp = 0;
            Sh = 0;

            for (int i = 0; i < result.It.Count - 1; i++)
            {
                double tim1 = result.It.ElementAt(i).Key;
                double tim2 = result.It.ElementAt(i + 1).Key;
                double item = result.It.ElementAt(i).Value;

                if (item > 0)
                {
                    Sh += item * (tim2 - tim1);
                    Sp += 0;
                }
                else
                {
                    Sh += 0;
                    Sp += -item * (tim2 - tim1);
                }
            }

            Sh /= data.N;
            Sp /= data.N;

            Sh *= data.h;
            Sp *= data.w;

            Sd = result.k * data.К + result.q * data.i;

            Sob = Sh + Sd + Sp;
            this.s = s;
        }
    }

    public class Calculator
    {
        Random random = new Random(0);
        private List<DiskretRandomValue> possValues;
        private double lambda;
        private double t_min, t_max;


        public Calculator()
        {
            this.possValues = new List<DiskretRandomValue>()
            {
                new DiskretRandomValue() {value = 10, p = 0.4},
                new DiskretRandomValue() {value = 15, p = 0.6},
            };

            this.lambda = 2;
            this.t_min = 0.5;
            this.t_max = 2;
        }

        public double RandExp(double lyambda)
        {
            double xk = random.NextDouble();
            return ((-1.0 / lyambda) * Math.Log(xk));
        }

        double getNextTd()
        {
            return this.RandExp(this.lambda);
        }

        double getUniform(double a, double b)
        {
            return a + (b - a) * random.NextDouble();
        }

        double getNextTau()
        {
            return this.getUniform(this.t_min, this.t_max);
        }

        double getNextDiskret(List<DiskretRandomValue> values)
        {
            double p = random.NextDouble();
            double p_sum = 0;
            foreach (var value in values)
            {
                if (p_sum <= p && p < p_sum + value.p)
                {
                    return value.value;
                }
            }

            return values.Last().value;
        }

        double getD()
        {
            return this.getNextDiskret(this.possValues);
        }

        enum EventType
        {
            Spros,
            Ozenka,
            ZakazCome
        }

        const int ozenkaDelta = 28;


        public StrategyImitationResult ImitateStrategy(double initial, double minS, double maxS, int N)
        {
            SortedDictionary<double, double> tovarByTime = new SortedDictionary<double, double>();

            double q = 0;
            double k = 0;

            double i_t = initial;

            tovarByTime.Add(0, initial);


            SortedDictionary<double, EventType> events = new SortedDictionary<double, EventType>();


            double td = this.getNextTd();
            events.Add(td, EventType.Spros);


            events.Add(ozenkaDelta, EventType.Ozenka);
            double s = minS;
            double S = maxS;

            Queue<double> zakzay = new Queue<double>();

            while (events.Count > 0)
            {
                double time = events.Keys.First();
                EventType eventType = events[time];

                events.Remove(time);

                switch (eventType)
                {
                    case EventType.Spros:
                        double D = this.getD();
                        i_t -= D;
                        td = this.getNextTd();
                        if (td + time <= N)
                        {
                            events.Add(td + time, EventType.Spros);
                        }

                        break;
                    case EventType.Ozenka:
                        if (i_t < s)
                        {
                            double count = S - i_t;
                            q += count;
                            k++;
                            //zatraty
                            zakzay.Enqueue(count);

                            double zakaz_come = this.getNextTau();
                            if (time + zakaz_come <= N)
                            {
                                events.Add(time + zakaz_come, EventType.ZakazCome);
                            }
                        }

                        if (time + ozenkaDelta <= N)
                        {
                            events.Add(time + ozenkaDelta, EventType.Ozenka);
                        }

                        break;
                    case EventType.ZakazCome:
                        i_t += zakzay.Dequeue();
                        break;
                }

                tovarByTime.Add(time, i_t);
            }

            return new StrategyImitationResult() {It = tovarByTime, k = k, q = q};
        }


        public List<StrategyCalculationResult> Calculate(InputData data)
        {
            List<StrategyCalculationResult> results = new List<StrategyCalculationResult>();
            foreach (var str in data.Strategies)
            {
                StrategyImitationResult res = ImitateStrategy(data.i0, str.s, str.S, data.N);
                StrategyCalculationResult result = new StrategyCalculationResult(res, data, str);
                results.Add(result);
            }

            return results;
        }
    }
}