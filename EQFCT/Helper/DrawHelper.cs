using EQFCT.Model;
using System.Collections.ObjectModel;

namespace EQFCT.Helper
{
    public static class DrawHelper
    {
        public static void PreventTextOverLap(ObservableCollection<DmgModel> pCollection)
        {
            for (int i = pCollection.Count - 1; i > 0; i--)
            {
                if (pCollection[i - 1].Top <= pCollection[i].Top + pCollection[i].FontSize)
                {
                    pCollection[i - 1].Top = pCollection[i].Top + pCollection[i].FontSize + 1;
                }
            }
        }
    }
}
