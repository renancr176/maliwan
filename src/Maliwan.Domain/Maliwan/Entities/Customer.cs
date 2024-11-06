﻿using Maliwan.Domain.Core.DomainObjects;
using Maliwan.Domain.Maliwan.Enums;

namespace Maliwan.Domain.Maliwan.Entities;

public class Customer : Entity
{
    public string Name { get; set; }
    public string Document { get; set; }
    public CustomerTypeEnum Type { get; set; }

    #region Relationships

    public virtual ICollection<Order> Orders { get; set; }

    #endregion

    public Customer()
    {
    }

    public Customer(string name, string document, CustomerTypeEnum type)
    {
        Name = name;
        Document = document;
        Type = type;
    }
}