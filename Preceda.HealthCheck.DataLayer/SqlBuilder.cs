using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Preceda.HealthCheck.DataLayer
{
    static class SqlBuilder
    {
        public static string SelectByIdCommand<T>(object id) where T : class
        {
            var builder = new StringBuilder(1024);

            var type = typeof(T);

            builder.Append("SELECT * FROM `");
            builder.Append(TableName<T>());
            builder.Append("` WHERE ");

            foreach (var key in KeyNames<T>())
            {
                builder.Append('`');
                builder.Append(key);
                builder.Append("` = ");
                builder.Append(SQLValue(id));
                builder.Append(" AND ");
            }
            builder.Remove(builder.Length - 4, 4);

            builder.Append(';');

            return builder.ToString();
        }

        public static string UpdateCommand<T>(T entity) where T : class
        {
            var builder = new StringBuilder(1024);

            var type = typeof(T);

            builder.Append("UPDATE `");
            builder.Append(TableName<T>());
            builder.Append("` SET ");

            foreach (var field in Fields<T>(entity))
            {
                builder.Append('`');
                builder.Append(field.Item1);
                builder.Append("` = ");

                builder.Append(SQLValue(field.Item2));
                builder.Append(", ");
            }
            builder.Remove(builder.Length - 2, 2);

            builder.Append(" WHERE ");
            foreach (var key in Keys<T>(entity))
            {
                builder.Append('`');
                builder.Append(key.Item1);
                builder.Append("` = ");

                builder.Append(key.Item2);
                builder.Append(" AND ");
            }
            builder.Remove(builder.Length - 4, 4);

            builder.Append(';');

            return builder.ToString();
        }

        public static string DeleteByIdCommand<T>(object id) where T : class
        {
            var builder = new StringBuilder(1024);

            var type = typeof(T);

            builder.Append("DELETE FROM `");
            builder.Append(TableName<T>());
            builder.Append("` WHERE ");

            foreach (var key in KeyNames<T>())
            {
                builder.Append('`');
                builder.Append(key);
                builder.Append("` = ");
                builder.Append(SQLValue(id));
                builder.Append(" AND ");
            }
            builder.Remove(builder.Length - 4, 4);

            builder.Append(';');

            return builder.ToString();
        }

        public static string DeleteAllCommand<T>() where T : class
        {
            var builder = new StringBuilder(1024);

            var type = typeof(T);

            builder.Append("DELETE FROM `");
            builder.Append(TableName<T>());
            builder.Append("`;");

            return builder.ToString();
        }

        public static string InsertCommand<T>(T entity) where T : class
        {
            var builder = new StringBuilder(1024);

            var type = typeof(T);

            builder.Append("INSERT INTO `");
            builder.Append(TableName<T>());
            builder.Append("` (");

            foreach (var field in FieldNames<T>())
            {
                builder.Append("`");
                builder.Append(field);
                builder.Append("`,");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(") VALUES(");
            foreach (var value in FieldValues<T>(entity))
            {
                builder.Append(value);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(");");

            return builder.ToString();
        }

        private static string TableName<T>() where T : class
        {
            var type = typeof(T);

            var tableNameAttribute = type.GetCustomAttribute<TableNameAttribute>();
            if (tableNameAttribute != null)
                return tableNameAttribute.Name;
            else
                return type.Name;
        }
        private static string FieldName(PropertyInfo property)
        {
            var fieldNameAttribute = property.GetCustomAttribute<FieldNameAttribute>();
            if (fieldNameAttribute != null)
                return fieldNameAttribute.Name;
            else
                return property.Name;
        }

        private static IEnumerable<string> FieldNames<T>() where T : class
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var ignoreFieldAttribute = property.GetCustomAttribute<IgnoreFieldAttribute>();
                if (ignoreFieldAttribute == null)
                {
                    yield return FieldName(property);
                }
            }
        }

        private static IEnumerable<string> FieldValues<T>(T entity) where T : class
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var ignoreFieldAttribute = property.GetCustomAttribute<IgnoreFieldAttribute>();
                if (ignoreFieldAttribute == null)
                {
                    if (property.PropertyType == typeof(DateTime))
                        yield return SQLValue<DateTime>(property.GetValue(entity));
                    else if (property.PropertyType == typeof(TimeSpan))
                        yield return SQLValue<TimeSpan>(property.GetValue(entity));
                    else
                        yield return SQLValue(property.GetValue(entity));
                }
            }
        }

        private static IEnumerable<Tuple<string, string>> Fields<T>(T entity) where T : class
        {

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var ignoreFieldAttribute = property.GetCustomAttribute<IgnoreFieldAttribute>();
                if (ignoreFieldAttribute == null)
                {
                    if (property.PropertyType == typeof(DateTime))
                        yield return new Tuple<string, string>(FieldName(property), SQLValue<DateTime>(property.GetValue(entity)));
                    else if (property.PropertyType == typeof(TimeSpan))
                        yield return new Tuple<string, string>(FieldName(property), SQLValue<TimeSpan>(property.GetValue(entity)));
                    else
                        yield return new Tuple<string, string>(FieldName(property), SQLValue(property.GetValue(entity)));
                }
            }
        }

        private static IEnumerable<string> KeyNames<T>() where T : class
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var keyFieldAttribute = property.GetCustomAttribute<KeyFieldAttribute>();
                if (keyFieldAttribute != null)
                {
                    yield return FieldName(property);
                }
            }
        }

        private static IEnumerable<Tuple<string, string>> Keys<T>(T entity) where T : class
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var keyFieldAttribute = property.GetCustomAttribute<KeyFieldAttribute>();
                if (keyFieldAttribute != null)
                {
                    if (property.PropertyType == typeof(DateTime))
                        yield return new Tuple<string, string>(FieldName(property), SQLValue<DateTime>(property.GetValue(entity)));
                    else if (property.PropertyType == typeof(TimeSpan))
                        yield return new Tuple<string, string>(FieldName(property), SQLValue<TimeSpan>(property.GetValue(entity)));
                    else
                        yield return new Tuple<string, string>(FieldName(property), SQLValue(property.GetValue(entity)));
                }
            }
        }

        private static string SQLValue<T>(object value) 
        {
            var type = typeof(T);
            if (type == typeof(DateTime))
                return '"' + ((DateTime)value).ToString("yyyy-MM-dd hh:MM:ss") + '"';
            else if (type == typeof(TimeSpan))
                return '"' + ((TimeSpan)value).ToString(@"hh\:mm\:ss") + '"';
            else
                return '"' + value.ToString().Replace("\"", "\"\"") + '"';
        }

        private static string SQLValue(object value)
        {
            var type = typeof(object);
            if (type == typeof(DateTime))
                return '"' + ((DateTime)value).ToString("yyyy-MM-dd hh:MM:ss") + '"';
            else if (type == typeof(TimeSpan))
                return '"' + ((TimeSpan)value).ToString(@"hh\:mm\:ss") + '"';
            else
                return '"' + value.ToString().Replace("\"", "\"\"") + '"';
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public string Name;
        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FieldNameAttribute : Attribute
    {
        public string Name;
        public FieldNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class KeyFieldAttribute : Attribute
    {
     
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreFieldAttribute : Attribute
    {
    }
}
