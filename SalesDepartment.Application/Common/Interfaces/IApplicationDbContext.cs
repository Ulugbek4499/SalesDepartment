using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesDepartment.Domain.Entities;
using StackExchange.Redis;
using Telegram.Bot.Types;

namespace SalesDepartment.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Contract>  Contracts{ get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Founder> Founders { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
