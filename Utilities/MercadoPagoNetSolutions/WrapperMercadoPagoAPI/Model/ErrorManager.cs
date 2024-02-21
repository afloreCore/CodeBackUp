namespace WrapperMercadoPagoAPI.Model;
internal static class ErrorManager
{
    static string? desc, source;
    public static void SetErrorMesage(string descpError, string? sourceError)
    {
        (desc, source) = (descpError, sourceError);
    }
    public static string? MessageDescp => desc;
    public static string? MessageSource => source; 

}
