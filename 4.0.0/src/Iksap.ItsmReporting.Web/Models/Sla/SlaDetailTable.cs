
namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class SlaDetailTable
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }

        public SingleSlaTable[] data { get; set; }
    }
    
}