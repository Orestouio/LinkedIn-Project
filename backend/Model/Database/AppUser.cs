using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BackendApp.Model.Enums;

namespace BackendApp.Model
{
    public abstract class AppUser
    (
        string email,
        string passwordHash,
        UserRole userRole
    )
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get; set;}
        public string Email { get; set; } = email;
        [JsonIgnore]
        public string? PasswordHash { get; set; } = passwordHash;
        public UserRole UserRole{ get; set; } = userRole;
    }
}