using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapperMercadoPagoAPI.General
{
    public static class SaveStatesUtilities
    {
        public struct topicStruct
        {
            public string topic { get; set; }
            public string topicId { get; set; }
            public DateTime fchMov { get; set; }
        }

        public static List<topicStruct> listTopics = null!;
    }
}
