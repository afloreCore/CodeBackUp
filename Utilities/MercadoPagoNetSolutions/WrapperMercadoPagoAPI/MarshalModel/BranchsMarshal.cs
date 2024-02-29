using System.Runtime.InteropServices;

namespace WrapperMercadoPagoAPI.MarshalModel;
[ComVisible(true), Guid("B2E14858-9E66-40CB-9A0B-2B9878593C91"), ClassInterface(ClassInterfaceType.AutoDispatch), ProgId("BranchsMarshal")]
public class BranchsMarshal
{
    /// <summary>
    /// Lista de BranchRequestMarshal
    /// </summary>
    public List<BranchRequestMarshal> results { get; set; } = null!;


}
