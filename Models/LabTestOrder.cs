using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practo_backend.Models
{
    public class LabTestOrder
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public decimal TotalAmount { get; set; }

        public string? ShippingAddress { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime OrderDate { get; set; }

        public string? RazorpayOrderId { get; set; }

        public string? RazorpayPaymentId { get; set; }

        public ICollection<LabTestOrderItem> Items { get; set; } = new List<LabTestOrderItem>();
    }

    public class LabTestOrderItem
    {
        [Key]
        public int Id { get; set; }

        public int LabTestOrderId { get; set; }
        
        [ForeignKey("LabTestOrderId")]
        public LabTestOrder? Order { get; set; }

        public int? DiagnosticTestId { get; set; }
        [ForeignKey("DiagnosticTestId")]
        public DiagnosticTest? DiagnosticTest { get; set; }

        public int? HealthCheckupPackageId { get; set; }
        [ForeignKey("HealthCheckupPackageId")]
        public HealthCheckupPackage? HealthCheckupPackage { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
