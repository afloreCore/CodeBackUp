using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrwPinPadNet
{
    public class TransPending : IDisposable, ICloneable
    {
        public string Date;
        public string Hour;
        public string Function;
        public string Import;
        public string Comprobante;

        public TransPending()
        {
            Clean();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public void Clean()
        {
            this.Date = "";
            this.Hour = "";
            this.Function = "";
            this.Import = "";
            this.Comprobante = "";
        }

        public object Clone()
        {
            return this.MemberwiseClone();
          
        }
    }
}
