using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchuBan_ECom.Utility
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";
        public const string Role_Employee = "Employee";
        public const string Role_Company = "Company";

        public const string Status_Submitted = "Submitted";
        public const string Status_InProcess = "In Process";
        public const string Status_Ready = "Ready for Pickup";
        public const string Status_Completed = "Completed";
        public const string Status_Cancelled = "Cancelled";

        public const string PaymentStatus_Pending = "Pending";
        public const string PaymentStatus_Approved = "Approved";
        public const string PaymentStatus_Declined = "Declined";
    }
}
