using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace KsushaMatStat
{
    /// <summary>
    /// Класс для задания значения случайной переменной распределенной дискретно
    /// </summary>
    public class DiskretRandomValue
    {
        /// <summary>
        /// Возможное значение
        /// </summary>
        public double value;
        /// <summary>
        /// Вероятность значения
        /// </summary>
        public double p;
    }

    /// <summary>
    /// Класс для хранения результатов имитации стратегии
    /// </summary>
    public class StrategyImitationResult
    {
        /// <summary>
        /// I(t) – уровень запасов в момент времени t;
        /// </summary>
        public SortedDictionary<double, double> It;
        /// <summary>
        /// Общее кол-во заказаных товаров
        /// </summary>
        public double q;
        /// <summary>
        /// Общее кол-во заказов товара
        /// </summary>
        public double k;
        /// <summary>
        /// Стратегия для которого проводилась имитация
        /// </summary>
        public StrategyData s;
    }
/// <summary>
/// Класс для результатов оценки стартегии
/// </summary>
    public class StrategyCalculationResult
    {
        /// <summary>
        /// средние затраты на осуществление заказа для стратегий
        /// </summary>
        public double Sp;
        /// <summary>
        /// средние затраты для хранения продукции для стратеги
        /// </summary>
        public double Sh;
        /// <summary>
        /// средние издержки в месяц, связанные с нехваткой продукции 
        /// </summary>
        public double Sd;
/// <summary>
/// средние общие затраты в месяц
/// </summary>
        public double Sob;
/// <summary>
/// Стратегия
/// </summary>
        public StrategyData s;

        public StrategyCalculationResult(StrategyImitationResult result, InputData data, StrategyData s)
        {
            Sp = 0;
            Sh = 0;
// в конструкторе считаем среднее по месяцам
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

/// <summary>
/// Класс для вычислений
/// </summary>
    public class Calculator
    {
        /// <summary>
        /// генератор случайных чисел
        /// </summary>
        Random random = new Random(0);
        /// <summary>
        /// тут будут возможные значения для случайной переменной D(объем спроса на запас)
        /// </summary>
        private List<DiskretRandomValue> possValues;
        /// <summary>
        /// Лямбда - параметр экспоненциального распределения - Td - промежутки между возникновением спроса
        /// </summary>
        private double lambda;
        /// <summary>
        /// минимальное и максимальное значение для тау - время доставки продукции
        /// </summary>
        private double t_min, t_max;


        /// <summary>
        /// В конструкторе задаются значения для всех этих параметров
        /// </summary>
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

        /// <summary>
        /// Генерация случайного значения распределенного экспоненциально
        /// </summary>
        /// <param name="lyambda">параметр распределния лямбда</param>
        /// <returns>случю значение</returns>
        public double RandExp(double lyambda)
        {
            double xk = random.NextDouble();
            return ((-1.0 / lyambda) * Math.Log(xk));
        }
/// <summary>
/// Генерация Td
/// </summary>
/// <returns></returns>
        double getNextTd()
        {
            return this.RandExp(this.lambda);
        }
/// <summary>
/// Равномерно распределенное случ. значение
/// </summary>
/// <param name="a">минимальное возм значение</param>
/// <param name="b">макс возможное значение</param>
/// <returns></returns>

        double getUniform(double a, double b)
        {
            return a + (b - a) * random.NextDouble();
        }
/// <summary>
/// Генерация тау
/// </summary>
/// <returns></returns>
        double getNextTau()
        {
            return this.getUniform(this.t_min, this.t_max);
        }
/// <summary>
/// Генерация случайного значения распределенного дискретно
/// </summary>
/// <param name="values">Возможные значения</param>
/// <returns></returns>
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
/// <summary>
/// Генерация параметра D
/// </summary>
/// <returns></returns>
        double getD()
        {
            return this.getNextDiskret(this.possValues);
        }
/// <summary>
/// Возможные возникающие события
/// Spros - возникновение спроса
/// Ozenka - оценка зпасов
/// ZakazCome - поступление заказа
/// </summary>
        enum EventType
        {
            Spros,
            Ozenka,
            ZakazCome
        }
//Константа - оценка раз в месяц - через 28 дней
        const int ozenkaDelta = 28;

/// <summary>
/// Имитация применения стратегии
/// </summary>
/// <param name="initial">Начальное кол-во запасов</param>
/// <param name="minS">Минимальное кол-во запасов</param>
/// <param name="maxS">Максимальное кол-во запасов</param>
/// <param name="N">Длительность имитации</param>
/// <returns>Результатыимитации</returns>
        public StrategyImitationResult ImitateStrategy(double initial, double minS, double maxS, int N)
        {
            SortedDictionary<double, double> tovarByTime = new SortedDictionary<double, double>();//кол-во товар в момент времени - I(t)

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
//пока есть события - смотрим какое событие происходит в обрабатываем в зависимости от его типа
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


/// <summary>
/// Имитация стратегий и их оценка
/// </summary>
/// <param name="data">Исъодные данные</param>
/// <returns></returns>
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