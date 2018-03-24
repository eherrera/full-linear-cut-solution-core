using System.Linq;

namespace FullLinearCutSolution.Core.Model
{
    public class CutSolution
    {
        private Bar _bar;

        private CutPattern _cutPattern;
        public int BarCount
        {
            get
            {
                return _cutPattern.Units.Max();
            }
        }
        public CutSolution(Bar bar)
        {
            _bar = bar;
        }
        public void SetPattern(CutPattern pattern)
        {
            _cutPattern = pattern;    
        }
        public CutPattern GetCutPattern()
        {
            return _cutPattern;
        }
        public decimal Waste
        {
            get
            {
                return _bar.Length - _cutPattern?.Measurements.Sum() ?? 0;
            }
        }
        public decimal WastePercent
        {
            get
            {
                return Waste * 100 / _bar.Length;
            }
        }
    }
}
