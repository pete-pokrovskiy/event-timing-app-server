using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTiming.Data.Conventions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Удаляем практику "по умолчанию" именовать таблицы по имени DbSet-a (то есть во множественном числе)
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void SingularTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }
        }
    }
}
