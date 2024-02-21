using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GrwPinPadNet
{
    [ComVisible(true)]
    [Guid("1ea80845-fed5-4366-b3cd-704dd921cbed"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IReturnValue
    {
        void ReturnStringValue(Int32 TipoCampo, string Message, ref bool YesNot);

    }
}
