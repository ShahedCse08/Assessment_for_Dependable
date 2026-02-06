using System;

namespace PMS.Domain.Entities
{
    public class Audit
    {
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } 
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
