using System;
using System.Collections.Generic;

namespace WebApp.Web.Models
{
    public class Order
    {
        public Order()
        {
            UserId = string.Empty;
            Status = "Pending";
            Items = new List<OrderItem>();
            ShippingAddress = string.Empty;
            PaymentMethod = string.Empty;
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItem> Items { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Product? Product { get; set; }
    }
} 