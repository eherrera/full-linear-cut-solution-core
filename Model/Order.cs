using System.Collections.Generic;
using System.Linq;

namespace FullLinearCutSolution.Core.Model
{
    public class Order
    {
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public List<OrderItem> UnappliedItems
        {
            get
            {
                return Items.Where(i => !i.Applied).ToList();
            }
        }

        public int TotalPieces
        {
            get
            {
                return Items.Sum(i => i.Units);
            }
        }

        public void Normalize()
        {
            var items = new List<OrderItem>();
            for (int i = 0; i < Items.Count; i++)
            {
                var item = new OrderItem
                {
                    Measurement = Items[i].Measurement,
                    Reference = Items[i].Reference,
                    Units = Items[i].Units
                };
                var similar = items.FirstOrDefault(o => o.Measurement == Items[i].Measurement);
                if (similar == null)
                {
                    items.Add(item);
                }
                else
                {
                    similar.Units += item.Units;
                }
            }
            Items = items;
        }

        public void Sort()
        {
            Items.Sort(new OrderItemComparer());
        }
    }
}
