﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace StoreFront.DATA.EF.Models//Metadata
{
    //internal class Partials
    //{
    //}
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { }

    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }

    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product 
    {
        [NotMapped]
        public IFormFile? Image { get; set; }
    }

    [ModelMetadataType(typeof(CustomerDetailMetadata))]
    public partial class CustomerDetail { }

    [ModelMetadataType(typeof(ProductTypeMetadata))]
    public partial class ProductType { }

    [ModelMetadataType(typeof(ProductStatusMetadata))]
    public partial class ProductStatus { }

}
