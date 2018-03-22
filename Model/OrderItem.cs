using System.Collections.Generic;

namespace FullLinearCutSolution.Core.Model
{
    public class OrderItem
    {
        public decimal Measurement { get; set; }
        public int Units { get; set; }
        public string Reference { get; set; }
        public int AppliedUnits { get; set; } = 0;
        public bool Applied
        {
            get
            {
                return Units == AppliedUnits;
            }
        }
    }

    public class OrderItemComparer : Comparer<OrderItem>
    {
        public override int Compare(OrderItem x, OrderItem y)
        {
            return x.Measurement > y.Measurement ? 1 : 0;
        }
    }
}
