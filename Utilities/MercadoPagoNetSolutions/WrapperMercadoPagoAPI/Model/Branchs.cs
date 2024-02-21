using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Model;
public class Branchs
{
    public Paging paging { get; set; } = null!;
    public List<BranchRequest> results { get; set; } = null!;

}
