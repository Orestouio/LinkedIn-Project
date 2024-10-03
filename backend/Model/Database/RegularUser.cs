using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BackendApp.Model.Enums;

namespace BackendApp.Model
{
    [method : JsonConstructor]
    public class RegularUser(
        string email,
        string passwordHash,
        string name,
        string surname,
        string? imagePath,
        RegularUserHideableInfo hideableInfo
    ) : AppUser(email, passwordHash, UserRole.User)
    
    {
        public static RegularUser MapNewWithHiddenPassword(RegularUser user)
        {
            return new RegularUser(user){
                Id = user.Id, 
                PasswordHash = ""
            };
        }

        public RegularUser( RegularUser user ) 
        : this
        (user.Email, user.PasswordHash ?? "", user.Name, user.Surname, user.ImagePath, new(user.HideableInfo))
        {}
        
        protected RegularUser( 
            string email,
            string passwordHash,
            string name,
            string surname,
            string? imagePath
        )
        : this(email, passwordHash, name, surname, imagePath, null!)
        {}

        public string Name { get; set; } = name;
        public string Surname { get; set; } = surname;
        public string? ImagePath { get; set; } = imagePath;
        public virtual RegularUserHideableInfo HideableInfo { get; set; } = hideableInfo;
        public string FullName => $"{this.Name} {this.Surname}";
    }
}