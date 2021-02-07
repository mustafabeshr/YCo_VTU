using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class UserNotifyDto
    {
        public UserNotifyDto()
        {
            Priorities = new List<CommonCode>();
            Statuses = new List<CommonCode>();
            CreatedBy = new AppUser();
            Roles = new List<Role>();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        [StringLength(250, ErrorMessage = "طول النص يجب ان لايزيد عن 250 حرف ")]
        [Required(ErrorMessage = "يجب كتابة نص التعميم")]
        public string Content { get; set; }
        [StringLength(200, ErrorMessage = "طول الموضوع يجب ان لايزيد عن 200 حرف ")]
        [Required(ErrorMessage = "يجب كتابة الموضوع")]
        public string Subject { get; set; }
        public string PriorityId { get; set; }
        public List<CommonCode> Priorities { get; set; }
        public string StatusId { get; set; }
        public List<CommonCode> Statuses { get; set; }
        public DateTime StatusOn { get; set; }
        public AppUser CreatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime ExpireOn { get; set; }
        public List<Role> Roles { get; set; }
        [Required(ErrorMessage = "يجب تحديد المستخدمين")]
        public string SelectedRoles { get; set; }
        public List<UserNotifyTo> NotifyToList { get; set; }
    }

    public class UserNotifyQueryDto
    {
        public UserNotifyQueryDto()
        {
            Statuses = new List<CommonCode>();
            Paging = new Paging();
        }
        public int Id { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string StatusId { get; set; }
        public List<CommonCode> Statuses { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public Paging Paging { get; set; }
        public List<UserNotify> Results { get; set; }
    }

    public class UserNotifyHisQueryDto
    {
        public UserNotifyHisQueryDto()
        {
            Statuses = new List<CommonCode>();
            Paging = new Paging();
        }
        public int Id { get; set; }
        public string Content { get; set; }
        [StringLength(9)]
        public string PartnerId { get; set; }
        public string Subject { get; set; }
        public string StatusId { get; set; }
        public List<CommonCode> Statuses { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public Paging Paging { get; set; }
        public List<UserNotifyHistory> Results { get; set; }
    }
}
