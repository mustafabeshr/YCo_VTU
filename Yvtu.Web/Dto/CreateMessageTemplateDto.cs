using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yvtu.Core.Entities;

namespace Yvtu.Web.Dto
{
    public class CreateMessageTemplateDto
    {
        public int Id { get; set; }
        public int ToWho { get; set; }
        [Required(ErrorMessage = "يجب ادخال العنوان")]
        [StringLength(200, ErrorMessage = "يجب ان يكون طول العنوان بين 10 و 200 حرف", MinimumLength = 10)]
        public string Title { get; set; }

        [Required(ErrorMessage = "يجب ادخال نص الرسالة")]
        [StringLength(400, ErrorMessage = "يجب ان يكون طول نص الرسالة بين 10 و 400 حرف", MinimumLength = 10)]
        public string Message { get; set; }

        public virtual List<MessageDictionary> Dictionary { get; set; }
    }
}
