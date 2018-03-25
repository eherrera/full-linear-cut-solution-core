using System;
using System.Collections.Generic;
using System.Linq;
using LinealCutOptimizer.Core.Model;

namespace FullLinearCutSolution.Core.Model
{
    public class CutPattern
    {
        public List<decimal> Measurements { get; set; } = new List<decimal>();
        public List<int> Units { get; set; } = new List<int>();
        public bool TrySatisfy(List<OrderItem> items, bool forceBestCase = false)
        {
            var min = int.MaxValue;

            foreach (var m in Measurements)
            {
                var item = items.FirstOrDefault(e => e.Measurement == m);
                if (!items.Any(i => i.Measurement == m))
                {
                    return false;
                }
                if (item != null)
                {
                    var unappliedUnits = item.Units - item.AppliedUnits;                    
                    if (unappliedUnits < min)
                    {
                        min = unappliedUnits;
                    }
                }
                if (forceBestCase)
                {
                    if (items.Any(i => i.Measurement == m && i.Units - i.AppliedUnits < min))
                    {
                        return false;
                    }
                }
            }
            min = FixMinimalUnitAmount(min, items);
            if (min == 0)
            {
                return false;
            }

            foreach (decimal m in Measurements)
            {
                var item = items.FirstOrDefault(e => e.Measurement == m);
                if (item != null)
                {
                    var unappliedUnits = item.Units - item.AppliedUnits;
                    if (unappliedUnits < min)
                    {                    
                        min = unappliedUnits;
                    }
                }
                if (item != null) item.AppliedUnits += min;
                Units.Add(min);
            }
            return true;
        }

        private int FixMinimalUnitAmount(int min, List<OrderItem> items)
        {
            var uniqueMeasurements = new Dictionary<decimal, int>();
            foreach (var m in Measurements)
            {
                if (!uniqueMeasurements.ContainsKey(m))
                {
                    uniqueMeasurements.Add(m, 1);
                }
                else
                {
                    uniqueMeasurements[m]++;
                }
            }

            bool ValidMinimalUnitAmount()
            {
                foreach (var d in uniqueMeasurements)
                {
                    var item = items.FirstOrDefault(i => i.Measurement == d.Key);
                    if (item == null) continue;
                    var unappliedUnits = item.Units - item.AppliedUnits;
                    if (min * d.Value > unappliedUnits)
                    {
                        return false;
                    }
                }
                return true;
            }

            while (!ValidMinimalUnitAmount())
            {
                min--;
            }

            return min;

        }       
    }

    public class CutPatternComparer : IComparer<CutPattern>
    {
        private readonly OptimizerStrategy _strategy;

        public CutPatternComparer(OptimizerStrategy strategy)
        {
            _strategy = strategy;
        }

        public int Compare(CutPattern x, CutPattern y)
        {
            switch (_strategy)
            {
                case OptimizerStrategy.Optimize:
                    return CompareUsingOptimizeStrategy(x, y);
                case OptimizerStrategy.Traditional:
                    return CompareUsingTraditionalStrategy(x, y);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int CompareUsingTraditionalStrategy(CutPattern x, CutPattern y)
        {
            var xDistinctCount = x?.Measurements?.Distinct().Count() ?? 0;
            var yDistinctCount = y?.Measurements?.Distinct().Count() ?? 0;
            if (xDistinctCount > yDistinctCount)
            {
                return 1;
            }
            if (xDistinctCount < yDistinctCount)
            {
                return -1;
            }
            return CompareUsingOptimizeStrategy(x, y);
        }

        private static int CompareUsingOptimizeStrategy(CutPattern x, CutPattern y)
        {
            var xSum = x?.Measurements?.Sum() ?? 0;
            var ySum = y?.Measurements?.Sum() ?? 0;
            if (xSum < ySum)
            {
                return 1;
            }
            if (xSum > ySum)
            {
                return -1;
            }
            var xCount = x?.Measurements?.Count;
            var yCount = y?.Measurements?.Count;
            if (xCount > yCount)
            {
                return 1;
            }
            if (xCount < yCount)
            {
                return -1;
            }
            return 0;
        }
    }

    public static class CutPatternExtension
    {
        public static CutPattern Clone(this CutPattern original)
        {
            var clone = new CutPattern();
            foreach (var m in original.Measurements)
            {
                clone.Measurements.Add(m);
            }
            return clone;
        }        

        public static bool ContainsPattern(this List<CutPattern> list, CutPattern cutPattern)
        {            
            foreach (var thisPattern in list.Where(e => e.Measurements.Count == cutPattern.Measurements.Count).ToList())
            {
                if (thisPattern.Measurements.All(m => cutPattern.Measurements.Contains(m)) && cutPattern.Measurements.All(s => thisPattern.Measurements.Contains(s)))
                {
                    var thisPatternDict = new Dictionary<decimal, int>();
                    var patternDict = new Dictionary<decimal, int>();
                    for (int i = 0; i < thisPattern.Measurements.Count; i++)
                    {
                        if (!thisPatternDict.ContainsKey(thisPattern.Measurements[i]))
                        {
                            thisPatternDict.Add(thisPattern.Measurements[i], 1);
                        }
                        else
                        {
                            thisPatternDict[thisPattern.Measurements[i]]++;
                        }

                        if (!patternDict.ContainsKey(cutPattern.Measurements[i]))
                        {
                            patternDict.Add(cutPattern.Measurements[i], 1);
                        }
                        else
                        {
                            patternDict[cutPattern.Measurements[i]]++;
                        }
                    }
                    var result = false;
                    foreach (var dict in thisPatternDict)
                    {
                        if (patternDict.ContainsKey(dict.Key))                        
                        {
                            if (dict.Value != patternDict[dict.Key])
                            {
                                result = false;
                                break;
                            }
                        }
                        result = true;
                    }
                    if (result)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void Sort(this List<CutPattern> patterns, OptimizerStrategy strategy)
        {
            patterns.Sort(new CutPatternComparer(strategy));
        }        
    }
}
