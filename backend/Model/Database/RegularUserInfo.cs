using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BackendApp.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Model
{
    [method : JsonConstructor]
    public class RegularUserHideableInfo
    (
        string phoneNumber,
        bool phoneNumberIsPublic,
        string location,
        bool locationIsPublic,
        List<string> experience,
        bool experienceIsPublic,
        string? currentPosition,
        bool currentPositionIsPublic,
        List<string> capabilities,
        bool capabilitiesArePublic,
        List<string> education,
        bool educationIsPublic
    )
    {
        public RegularUserHideableInfo(RegularUserHideableInfo hideableInfo)
        : this
        (
            hideableInfo.PhoneNumber, 
            hideableInfo.PhoneNumberIsPublic,
            hideableInfo.Location, 
            hideableInfo.LocationIsPublic,
            hideableInfo.Experience,
            hideableInfo.ExperienceIsPublic,
            hideableInfo.CurrentPosition,
            hideableInfo.CurrentPositionIsPublic,
            hideableInfo.Capabilities,
            hideableInfo.CapabilitiesArePublic,
            hideableInfo.Education,
            hideableInfo.EducationIsPublic
        )
        {}

        public RegularUserHideableInfo
        (
            string phoneNumber,
            string location,
            List<string> experience,
            string? currentPosition,
            List<string> capabilities,
            List<string> education
        )
        : this
        (
            phoneNumber, true,
            location, true,
            experience, true,
            currentPosition, true,
            capabilities, true,
            education, true
        )
        {}
        
        /// <summary>
        /// DO NOT USE!!! This is the EFCore Constructor.
        /// </summary>
        protected RegularUserHideableInfo
        (
            string phoneNumber,
            string location,
            string? currentPosition
        )
        : this
        (
            phoneNumber, location, [],
            currentPosition, [], []
        )
        {}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get; set;}
        public string PhoneNumber {get; set;}= phoneNumber;
        public bool PhoneNumberIsPublic {get; set;}= phoneNumberIsPublic;
        public string Location {get; set;} = location;
        public bool LocationIsPublic {get; set;} = locationIsPublic;
        public string? CurrentPosition {get; set;} = currentPosition;
        public bool CurrentPositionIsPublic {get; set;} = currentPositionIsPublic;
        public List<string> Experience {get; set;} = experience;
        public bool ExperienceIsPublic {get; set;} = experienceIsPublic;
        public List<string> Capabilities {get; set;} = capabilities;
        public bool CapabilitiesArePublic {get; set;} = capabilitiesArePublic;
        public List<string> Education {get; set;} = education;
        public bool EducationIsPublic {get; set;} = educationIsPublic;

        public RegularUserHideableInfo MapToHidden()
        {
            return new(
                this.PhoneNumberIsPublic ? PhoneNumber : "",
                this.PhoneNumberIsPublic,
                this.LocationIsPublic ? Location : "",
                this.LocationIsPublic,
                this.ExperienceIsPublic ? Experience : [],
                this.ExperienceIsPublic,
                this.CurrentPositionIsPublic ? CurrentPosition : "",
                this.CurrentPositionIsPublic,
                this.CapabilitiesArePublic ? this.Capabilities : [],
                this.CapabilitiesArePublic,
                this.EducationIsPublic ? this.Education : [],
                this.EducationIsPublic
            );
        }
    }
}