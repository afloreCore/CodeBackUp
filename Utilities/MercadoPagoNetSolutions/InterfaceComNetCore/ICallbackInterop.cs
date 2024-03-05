using System.Runtime.InteropServices;

namespace InterfaceComNetCore
{
    [ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), Guid("3883EEA5-0577-40DD-9A8A-9748713E6146") ]

    public interface ICallbackInterop
    {
        void ReturnValue(string topic, string topicId);
    }
}
