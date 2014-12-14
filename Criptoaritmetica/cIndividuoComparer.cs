using System;
using System.Collections.Generic;
using System.Text;

namespace Criptoaritmetica
{
    public class cIndividuoComparer : IComparer<cIndividuo>
    {
        public int Compare(cIndividuo x, cIndividuo y)
        {
            if (x.aptidao > y.aptidao)
            {
                return -1;
            }
            else
                if (x.aptidao < y.aptidao)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
        }
    }
}
