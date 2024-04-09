using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class ProviderLocation
    {
        public int Id { get; set; }
        public string ProviderName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
