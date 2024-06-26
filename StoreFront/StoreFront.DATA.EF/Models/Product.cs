﻿using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public string? ProductImage { get; set; }
        public int? OuncesPerUnit { get; set; }
        public int? MillilitersPerUnit { get; set; }
        public int StatusId { get; set; }
        public bool Tool { get; set; }
        public int ProductTypeId { get; set; }

        //public virtual Category Category { get; set; } = null!;
        public virtual Category? Category { get; set; }
        //public virtual ProductType ProductType { get; set; } = null!;
        public virtual ProductType? ProductType { get; set; }
        //public virtual ProductStatus Status { get; set; } = null!;
        public virtual ProductStatus? ProductStatus { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
