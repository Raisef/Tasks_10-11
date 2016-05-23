using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersBase.Entities
{
    public class Award
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<int,string> Owners { get; set; }
        public string ImageType { get; set; }
    }
}
