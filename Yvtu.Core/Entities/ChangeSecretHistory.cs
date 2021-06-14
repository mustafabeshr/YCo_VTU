using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class ChangeSecretHistory
    {
        public ChangeSecretHistory()
        {
            CreatedBy = new AppUser();
            PartAppUser = new AppUser();
            AccessChannel = new CommonCode();
            ChangeType = new CommonCode();
            NotifyBy = new CommonCode();
        }
        public int Seq { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public CommonCode AccessChannel { get; set; }
        public CommonCode ChangeType { get; set; }
        public CommonCode NotifyBy { get; set; }
        public AppUser PartAppUser { get; set; }
        public string OldSalt { get; set; }
        public string OldHash { get; set; }
        public string NewSalt { get; set; }
        public string NewHash { get; set; }
    }
}
