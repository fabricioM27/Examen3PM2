using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen3PM2.Models
{
    public class Notas
    {
        public string Id_Notas { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public byte[] Photo_Record { get; set; }
        public byte[] Audio_Record { get; set; }

    }
}
