using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.Models.Entities
{
    public class SchoolUserModel
    {
        public SchoolUserModel()
        {
            UserRoles = new List<SchoolUserRoleModel>();
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [Column(TypeName = "varchar(100)")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Column(TypeName = "varchar(100)")]
        public string Password { get; set; }
        public virtual ICollection<SchoolUserRoleModel> UserRoles { get; set; }
    }
}
