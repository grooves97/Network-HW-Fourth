using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulticastUDPClient
{
    public class UserClient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserClient(string name)
        {
            Name = name;
        }
    }
}
