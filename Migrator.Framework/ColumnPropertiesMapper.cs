using System;
using System.Collections.Generic;

namespace Migrator.Framework
{
    /// <summary>
    ///   This is basically a just a helper base class per-database implementers may want to override ColumnSql
    /// </summary>
    public class ColumnPropertiesMapper
    {
        /// <summary>
        ///   the type of the column
        /// </summary>
        protected string columnSql;

        /// <summary>
        ///   Sql if this column has a default value
        /// </summary>
        protected object defaultVal;

        protected Dialect dialect;

        /// <summary>
        ///   Sql if This column is Indexed
        /// </summary>
        protected bool indexed;

        /// <summary>
        ///   The name of the column
        /// </summary>
        protected string name;

        /// <summary>
        ///   The SQL type
        /// </summary>
        protected string type;

        public ColumnPropertiesMapper(Dialect dialect, string type)
        {
            this.dialect = dialect;
            this.type = type;
        }

        /// <summary>
        ///   The sql for this column, override in database-specific implementation classes
        /// </summary>
        public virtual string ColumnSql
        {
            get { return columnSql; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public object Default
        {
            get { return defaultVal; }
            set { defaultVal = value; }
        }

        public string QuotedName
        {
            get { return dialect.Quote(Name); }
        }

        public string IndexSql
        {
            get
            {
                if (dialect.SupportsIndex && indexed)
                    return String.Format("INDEX({0})", dialect.Quote(name));
                return null;
            }
        }

        public void MapColumnProperties(Column column)
        {
            Name = column.Name;

            var vals = new List<string>();
            vals.Add(dialect.ColumnNameNeedsQuote || dialect.IsReservedWord(Name) ? QuotedName : Name);

            vals.Add(type);

            if (!dialect.IdentityNeedsType)
                AddValueIfSelected(column, ColumnProperty.Identity, vals);

            if (dialect.IsUnsignedCompatible(column.Type))
                AddValueIfSelected(column, ColumnProperty.Unsigned, vals);

            if (!PropertySelected(column.ColumnProperty, ColumnProperty.PrimaryKey) || dialect.NeedsNotNullForIdentity)
                AddValueIfSelected(column, ColumnProperty.NotNull, vals);

            AddValueIfSelected(column, ColumnProperty.PrimaryKey, vals);

            if (dialect.IdentityNeedsType)
                AddValueIfSelected(column, ColumnProperty.Identity, vals);

            AddValueIfSelected(column, ColumnProperty.Unique, vals);
            AddValueIfSelected(column, ColumnProperty.ForeignKey, vals);

            if (column.DefaultValue != null)
                vals.Add(dialect.Default(column.DefaultValue));

            columnSql = String.Join(" ", vals.ToArray());
        }

        private void AddValueIfSelected(Column column, ColumnProperty property, ICollection<string> vals)
        {
            if (PropertySelected(column.ColumnProperty, property))
                vals.Add(dialect.SqlForProperty(property));
        }

        public static bool PropertySelected(ColumnProperty source, ColumnProperty comparison)
        {
            return (source & comparison) == comparison;
        }
    }
}