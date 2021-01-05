using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class BackgroundService
    {
        public BackgroundService()
        {
            CreatedBy = new AppUser();
            Partner = new AppUser();
            Status = new CommonCode();
            Source = new CommonCode();
        }
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser CreatedBy { get; set; }
        public CommonCode Source { get; set; }
        public AppUser Partner { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RecordCount { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public CommonCode Status { get; set; }
        public DateTime StatusTime { get; set; }
        public DateTime ActiveTime { get; set; }
        public int DurationInSec { get; set; }
        public string Note { get; set; }
    }
}
