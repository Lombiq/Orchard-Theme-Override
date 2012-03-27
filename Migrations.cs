using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Piedone.ThemeOverride.Models;

namespace Piedone.ThemeOverride
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ThemeOverrideSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Style")
            );


            return 1;
        }
    }
}
