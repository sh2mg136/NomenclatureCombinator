using System.Collections.Generic;
using System.Linq;

namespace NomenclatureCombinator
{
    internal class NomenclatureContainer
    {
        public NomenclatureContainer()
        {
            Nomenclatures = new List<Nomenclature>();
        }

        public int Id { get; set; } = 0;

        public virtual List<Nomenclature> Nomenclatures { get; set; }

        public bool HasDublicate => Nomenclatures.GroupBy(x => x.Article).Any(x => x.Count() > 1);
    }
}