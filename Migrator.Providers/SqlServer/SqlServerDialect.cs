using System;
using System.Data;
using Migrator.Framework;

namespace Migrator.Providers.SqlServer
{
    public class SqlServerDialect : Dialect
    {
        public SqlServerDialect()
        {
            RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "CHAR($l)");
            RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
            RegisterColumnType(DbType.AnsiString, 8000, "VARCHAR($l)");
            RegisterColumnType(DbType.AnsiString, 2147483647, "VARCHAR(MAX)");
            RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
            RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
            RegisterColumnType(DbType.Binary, 2147483647, "VARBINARY(MAX)");
            RegisterColumnType(DbType.Boolean, "BIT");
            RegisterColumnType(DbType.Byte, "TINYINT");
            RegisterColumnType(DbType.Currency, "MONEY");
            RegisterColumnType(DbType.Date, "DATETIME");
            RegisterColumnType(DbType.DateTime, "DATETIME");
            RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
            RegisterColumnType(DbType.Decimal, 19, "DECIMAL(19, $l)");
            RegisterColumnType(DbType.Double, "DOUBLE PRECISION"); //synonym for FLOAT(53)
            RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INT");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24) 
            RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($l)");
            RegisterColumnType(DbType.String, "NVARCHAR(255)");
            RegisterColumnType(DbType.String, 4000, "NVARCHAR($l)");
            RegisterColumnType(DbType.String, 1073741823, "NVARCHAR(MAX)");
            RegisterColumnType(DbType.Time, "DATETIME");

            RegisterColumnType(DbType.Xml, "XML");

            RegisterProperty(ColumnProperty.Identity, "IDENTITY");
        }

        public override bool SupportsIndex
        {
            get { return false; }
        }

        public override bool SupportsMultiDb
        {
            get { return true; }
        }

        public override bool ColumnNameNeedsQuote
        {
            get { return true; }
        }

        public override bool TableNameNeedsQuote
        {
            get { return true; }
        }

        public override string QuoteTemplate
        {
            get { return "[{0}]"; }
        }

        public override string Quote(string value)
        {
            int firstDotIndex = value.IndexOf('.');
            if (firstDotIndex >= 0)
            {
                string owner = value.Substring(0, firstDotIndex);
                string table = value.Substring(firstDotIndex + 1);
                return (string.Format(QuoteTemplate, owner) + "." + string.Format(QuoteTemplate, table));
            }
            return string.Format(QuoteTemplate, value);
        }

        public override string Default(object defaultValue)
        {
            if (defaultValue is bool)
            {
                defaultValue = ((bool) defaultValue) ? 1 : 0;
            }
            return String.Format("DEFAULT {0}", defaultValue);
        }
    }
}