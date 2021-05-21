using System;

namespace Yvtu.Core.Entities
{
    public class PaymentValues
    {
        public PaymentValues()
        {
            CreatedBy = new AppUser();
        }
        public int Seq { get; set; }
        public double PayValue { get; set; }
        public double ProfileId { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return
                $"Seq={Seq}\n, PayValue={PayValue}\n, ProfileId={ProfileId}, CreatedBy.Id={CreatedBy.Id}\n, CreatedBy.Account={CreatedBy.Account}\n,  CreatedBy.Name={CreatedBy.Name}\n" +
                $" CreatedOn={CreatedOn.ToString("dd/MM/yyyy HH:mm:ss")}";
        }
    }
}
