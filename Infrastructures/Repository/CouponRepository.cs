﻿using Infrastructures.Data;
using Infrastructures.Repository.IRepository;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {

        public CouponRepository(ApplicationDbContext context) : base(context)
        {


        }



    }
}
