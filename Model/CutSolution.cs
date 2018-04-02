using System.Collections.Generic;
using System.Linq;
using LinealCutOptimizer.Core.Helpers;

namespace FullLinearCutSolution.Core.Model
{
    public class CutSolution
    {
        private readonly Bar _bar;

        private CutPattern _cutPattern;
        public int BarCount => ParentSolutionId != null ? 0 : _cutPattern?.BarCount ?? 0;

        public CutSolution(Bar bar)
        {
            _bar = bar;
            Id = Autonumeric.GetNextId();
        }

        public void SetPattern(CutPattern pattern)
        {
            _cutPattern = pattern;
        }

        public CutPattern GetCutPattern()
        {
            return _cutPattern;
        }

        public Bar GetBar()
        {
            return _bar;
        }

        public IList<CutSolution> SubSolutions { get; set; } = new List<CutSolution>();

        public decimal Waste => _bar.Length - _cutPattern?.Measurements.Sum() ?? 0;

        public Dictionary<decimal, int> WasteDictionary
        {
            get
            {
                var maxUnits = _cutPattern.Units.Max();
                var mainWaste = _bar.Length - _cutPattern.Measurements.Sum();
                var result = new Dictionary<decimal, int> {{mainWaste, maxUnits}};
                for (var i = 0; i < _cutPattern.Measurements.Count; i++)
                {
                    if (_cutPattern.Measurements[i] < maxUnits)
                    {
                        var diff = maxUnits - _cutPattern.Units[i];
                        if (diff == 0)
                        {
                            continue;
                        }

                        if (result.ContainsKey(_cutPattern.Measurements[i]))
                        {
                            result[_cutPattern.Measurements[i]] += diff;
                        }
                        else
                        {
                            result.Add(_cutPattern.Measurements[i], diff);
                        }
                    }
                }

                foreach (var subSolution in SubSolutions)
                {
                    if (result.ContainsKey(subSolution.GetBar().Length))
                    {
                        result[subSolution.GetBar().Length] -= subSolution._cutPattern.BarCount;
                    }
                }

                return result;
            }
        }

        public decimal WastePercent => Waste * 100 / _bar.Length;

        public int Id { get; }

        public int? ParentSolutionId { get; set; }
    }
}