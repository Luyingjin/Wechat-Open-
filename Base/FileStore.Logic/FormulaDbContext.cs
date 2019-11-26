using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;


namespace Formula
{
    public class FormulaDbContext : DbContext
    {
        protected string ConnName = "";

        public FormulaDbContext(string connectionString)
            : base(connectionString)
        {
            ConnName = connectionString;
        }
    }
}
