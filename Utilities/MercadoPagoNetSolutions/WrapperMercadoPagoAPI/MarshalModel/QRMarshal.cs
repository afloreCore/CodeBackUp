using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.MarshalModel;

public class QRMarshal
{
    public string image { get; set; } = string.Empty;
    public string template_document { get; set; } = string.Empty;
    public string template_image { get; set; } = string.Empty;
    

}
