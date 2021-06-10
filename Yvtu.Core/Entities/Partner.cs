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
        public double Balance { get; set; }
        public double Reserved { get; set; }
        public int WrongPwdAttempts { get; set; }
        public string IPAddress { get; set; }
        public AppUser RefPartner { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"الرقم = {Id}");
            sb.Append($"\nالحساب = {Account}");
            sb.Append($"\nالاسم = {Name}");
            sb.Append($"\nالتجاري = {BrandName}");
            sb.Append($"\nالرصيد = {Balance}");
            sb.Append($"\nمحجوز = {Reserved}");
            //sb.Append($"\nRole Id = {Role.Id}");
            sb.Append($"\nالنوع = {Role.Name}");
            sb.Append($"\nنوع الهوية = {PersonalId.IdType.Name}");
            sb.Append($"\nرقمها = {PersonalId.Id}");
            sb.Append($"\nتارخها = {PersonalId.Issued.ToShortDateString()}");
            sb.Append($"\nمكانها = {PersonalId.Place}");
            //sb.Append($"\nStatus Id = {Status.Id}");
            sb.Append($"\nالحالة = {Status.Name}");
            sb.Append($"\nوقت الحالة = {StatusOn.ToString("yyyy/MM/dd H:mm:ss")}");
            //sb.Append($"\nStatus By Id = {StatusBy.Id}");
            //sb.Append($"\nStatus By Name = {StatusBy.Name}");
            //sb.Append($"\nCreated Time = {CreatedOn.ToString("yyyy/MM/dd H:mm:ss")}");
            //sb.Append($"\nCreated By Id = {CreatedBy.Id}");
            //sb.Append($"\nCreated By Name = {CreatedBy.Name}");
            //sb.Append($"\nCity Id = {Address.City.Id}");
            sb.Append($"\nالمدينة = {Address.City.Name}");
            //sb.Append($"\nDistrict Id = {Address.District.Id}");
            sb.Append($"\nالمديرية = {Address.District.Name}");
            sb.Append($"\nرقم احتياطي = {PairMobile}");
            sb.Append($"\nموبايل = {ContactInfo.Mobile}");
            sb.Append($"\nثابت = {ContactInfo.Fixed}");
            sb.Append($"\nفاكس = {ContactInfo.Fax}");
            sb.Append($"\nايميل = {ContactInfo.Email}");
            sb.Append($"\nاضافية = {Address.ExtraInfo}");
            sb.Append($"\nسيرفر = {IPAddress}");
            sb.Append($"\nمرجع = {RefPartner.Id}");
            return sb.ToString();
        }
    }
}
