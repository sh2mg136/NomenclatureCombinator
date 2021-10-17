using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NomenclatureCombinator
{
    internal class Nomenclature
    {
        public string Article { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Quantity * Price;

        public IEnumerator<Nomenclature> DiffPrices()
        {
            for (int i = 0; i <= Quantity; i++)
            {
                yield return new Nomenclature()
                {
                    Article = this.Article,
                    Quantity = i,
                    Price = this.Price,
                };
            }
        }

        public override string ToString()
        {
            var add = Quantity > 0 ? "+" : "";
            return $"{Article}  ->  {Quantity}  *  {Price}  =  {TotalPrice}         {add}\r\n";
        }

    }

    internal static class Ext
    {
        internal static bool HasDuplicateArticul(this List<Nomenclature> list)
        {
            return list.GroupBy(x => x.Article).Any(x => x.Count() > 1);
        }

        internal static int NumberOfDifferentArticuls(this List<Nomenclature> list)
        {
            return list.GroupBy(x => x.Article).Count();
        }
    }

}
