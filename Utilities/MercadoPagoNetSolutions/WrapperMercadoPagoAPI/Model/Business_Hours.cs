using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.Model;
public partial class Business_Hours
{
    /*
    Lunes – Monday (Mondei)
    Martes – Tuesday (Tusdei)
    Miércoles – Wednesday (Güensdei)
    Jueves – Thursday (Tursdei)
    Viernes – Friday (Fraidei)
    Sábado – Saturday (Saturdei)
    Domingo – Sunday (Sondei)
    */
    public enum DayName
    {
        monday,
        tuesday,
        wednesday,
        thursday,
        friday,
        saturday,
        sanday
    }

    //public Business_Hours()
    //{  
    //    //List<OpenClose> list = new List<OpenClose>()
    //    //{
    //    //    new OpenClose() {open = "00:00", close = "23:59"}
    //    //};
    //    //monday = list;
    //    //tuesday = list;
    //    //wednesday = list;
    //    //thursday = list;
    //    //friday = list;
    //    //saturday = list;
    //    //sunday = list;
    //}
    [JsonPropertyName("Wind")]
    public Dictionary<DayName, List<OpenClose>> day { get; set; } = null!;
    //public List<OpenClose> tuesday { get; set; } = null!;
    //public List<OpenClose> wednesday { get; set; } = null!;
    //public List<OpenClose> thursday { get; set; } = null!;
    //public List<OpenClose> friday { get; set; } = null!;
    //public List<OpenClose> saturday { get; set; } = null!;
    //public List<OpenClose> sunday { get; set; } = null!;
}
