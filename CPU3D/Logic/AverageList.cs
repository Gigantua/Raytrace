using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Logic
{
    public class AverageList : List<double>
    {
        List<double> items;
        int size;
        double min = double.PositiveInfinity;

        public AverageList(int size)
        {
            items = new List<double>(size);
            this.size = size;
        }

        public new void Add(double item)
        {
            if (items.Count > size)
            {
                items.RemoveAt(0);
            }
            items.Add(item);
        }

        public double Average => items.Average();
        public double Min
        {
            get
            {
                double m = Average;
                if (m < min) min = m;
                return min;
            }
        }

    }
}
