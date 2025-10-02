using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.Models.Entities
{
    public class SchoolUserRoleModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public virtual RoleModel Role { get; set; }
        public virtual SchoolUserModel User { get; set; }
    }
}
