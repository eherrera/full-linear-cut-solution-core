using System.Collections.Generic;

namespace FullLinearCutSolution.Core.Model
{
    public class OrderItem
    {
        public decimal Measurement { get; set; }
        public int Units { get; set; }
        public string Reference { get; set; }
        public int AppliedUnits { get; set; } = 0;
        public bool Applied => Units == AppliedUnits;
    }

    public class OrderItemComparer : IComparer<OrderItem>
    {
        public int Compare(OrderItem x, OrderItem y)
        {
            if (x?.Measurement < y?.Measurement)
            {
                return 1;
            }
            if (x?.Measurement > y?.Measurement)
            {
                return -1;
            }
            return 0;
        }
    }
}
