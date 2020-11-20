using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class Partner
    {
        public Partner()
        {
            Role = new Role();
            PersonalId = new PersonalId();
            PersonalId.IdType = new IdType();
            Status = new PartnerStatus();
            StatusBy = new AppUser();
            Address = new Address();
            Address.City = new City();
            Address.District = new District();
            ContactInfo = new ContactInfo();
            CreatedBy = new AppUser();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public Role Role { get; set; }
        public PersonalId PersonalId { get; set; }
        public PartnerStatus Status { get; set; }
        public AppUser StatusBy { get; set; }
        public DateTime StatusOn { get; set; }
        public Address Address { get; set; }
        public String PairMobile { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser CreatedBy { get; set; } 
        public DateTime LockTime { get; set; }
        public bool VerificationCodeNext { get; set; }
        public string Pwd { get; set; }
        public string Extra { get; set; }
        public long Balance { get; set; }
        public long Reserved { get; set; }
        public int WrongPwdAttempts { get; set; }
        public string IPAddress { get; set; }
    }
}
