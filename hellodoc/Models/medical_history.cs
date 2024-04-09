using System.ComponentModel.DataAnnotations;

namespace hellodoc.Models
{
    public class medical_history
    {
        [Key]
        public int Id { get; set; }

        public string Date{ get; set; }

        public string Status { get; set; }    

        public bool document { get; set; }

    }
}
