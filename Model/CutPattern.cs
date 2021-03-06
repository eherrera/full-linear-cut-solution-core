﻿using System.Collections.Generic;
using System.Linq;

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
                var unappliedUnits = item.Units - item.AppliedUnits;                    
                if (unappliedUnits < min)
                {
                    min = unappliedUnits;
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
            
            var result = true;
            for (int i = 0; i < Measurements.Count; i++)
            {
                var item = items.FirstOrDefault(e => e.Measurement == Measurements[i]);
                var unappliedUnits = item.Units - item.AppliedUnits;
                if (unappliedUnits < min)
                {                    
                    min = unappliedUnits;
                }
                item.AppliedUnits += min;
                Units.Add(min);
            }
            return result;
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
                    if (item != null)
                    {
                        var unappliedUnits = item.Units - item.AppliedUnits;
                        if (min * d.Value > unappliedUnits)
                        {
                            return false;
                        }
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
                        return result;
                    }
                }
            }
            return false;
        }

        public static List<CutPattern> SortPatterns(this List<CutPattern> patterns)
        {
            for (var i = 0; i < patterns.Count - 1; i++)
            {
                for (var j = i + 1; j < patterns.Count; j++)
                {
                    if (patterns[j].Measurements.Sum() > patterns[i].Measurements.Sum())
                    {
                        var temp = patterns[i];
                        patterns[i] = patterns[j];
                        patterns[j] = temp;
                    }
                }
            }
            return patterns;
        }        
    }
}
