using System;
using System.Collections.Generic;
using System.Text;
using Yvtu.Core.Utility;

namespace Yvtu.Core.Entities
{
    public class AppBackgroundService
    {
        public AppBackgroundService()
        {
            CreatedBy = new AppUser();
            Partner = new AppUser();
            Status = new CommonCode();
            Source = new CommonCode();
        }
        public int Id { get; set; }
        public string Name { get; set; }
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
        public DateTime ActualStartTime { get; set; }
        public int DurationInSec { get; set; }
        public string DurationTime { get {
                if (this.DurationInSec == 0 && (this.ActualStartTime != null && this.ActualStartTime > DateTime.MinValue))
                {
                    var duration = Convert.ToInt32(DateTime.Now.Subtract(this.ActualStartTime).TotalSeconds);
                    return ServicesLib.ConvertToTimeFormatter(duration);
                }
                else
                {
                    return ServicesLib.ConvertToTimeFormatter(this.DurationInSec);
                }
            } }
        public string Note { get; set; }
        public Int64 FileSize { get; set; }
        public string FileSizeFormat { get {
                return FileSizeFormatter.FormatSize(this.FileSize);
            } }
    }
}
