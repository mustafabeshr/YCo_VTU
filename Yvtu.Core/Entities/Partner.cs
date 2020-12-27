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
            RefPartner = new AppUser();
        }
        public string Id { get; set; }
        public int Account { get; set; }
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
        public DateTime LastLoginOn { get; set; }
        public AppUser CreatedBy { get; set; } 
        public DateTime LockTime { get; set; }
        public bool VerificationCodeNext { get; set; }
        public string Pwd { get; set; }
        public string Extra { get; set; }
        public long Balance { get; set; }
        public long Reserved { get; set; }
        public int WrongPwdAttempts { get; set; }
        public string IPAddress { get; set; }
        public AppUser RefPartner { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Id = {Id}");
            sb.Append($"\nAccount = {Account}");
            sb.Append($"\nBrand Name = {BrandName}");
            sb.Append($"\nBalance = {Balance}");
            sb.Append($"\nReserved = {Reserved}");
            sb.Append($"\nRole Id = {Role.Id}");
            sb.Append($"\nRole Name = {Role.Name}");
            sb.Append($"\nId Type = {PersonalId.IdType.Name}");
            sb.Append($"\nId No = {PersonalId.Id}");
            sb.Append($"\nId Date = {PersonalId.Issued.ToShortDateString()}");
            sb.Append($"\nId Place = {PersonalId.Place}");
            sb.Append($"\nStatus Id = {Status.Id}");
            sb.Append($"\nStatus Name = {Status.Name}");
            sb.Append($"\nStatus Time = {StatusOn.ToString("yyyy/MM/dd H:mm:ss")}");
            sb.Append($"\nStatus By Id = {StatusBy.Id}");
            sb.Append($"\nStatus By Name = {StatusBy.Name}");
            sb.Append($"\nCreated Time = {CreatedOn.ToString("yyyy/MM/dd H:mm:ss")}");
            sb.Append($"\nCreated By Id = {CreatedBy.Id}");
            sb.Append($"\nCreated By Name = {CreatedBy.Name}");
            sb.Append($"\nCity Id = {Address.City.Id}");
            sb.Append($"\nCity Name = {Address.City.Name}");
            sb.Append($"\nDistrict Id = {Address.District.Id}");
            sb.Append($"\nDistrict Name = {Address.District.Name}");
            sb.Append($"\nPair Mobile = {PairMobile}");
            sb.Append($"\nMobile No = {ContactInfo.Mobile}");
            sb.Append($"\nFixed No = {ContactInfo.Fixed}");
            sb.Append($"\nFax No = {ContactInfo.Fax}");
            sb.Append($"\nEmail = {ContactInfo.Email}");
            sb.Append($"\nExtra Address = {Address.ExtraInfo}");
            sb.Append($"\nIp = {IPAddress}");
            sb.Append($"\nRef = {RefPartner.Id}");
            return sb.ToString();
        }
    }
}
