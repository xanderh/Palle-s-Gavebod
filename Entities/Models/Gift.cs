using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Gift
    {
        [Key]
        public int GiftNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool BoyGift { get; set; }
        public bool GirlGift { get; set; }
    }
}
