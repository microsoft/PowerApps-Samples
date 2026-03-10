using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    internal class EntityDataToCsvHelper
    {
        private DataTable dataStore;

        // Not all types are handled here as this is just a sample
        private readonly Type[] _types =
        {
            typeof(DateTime),
            typeof(Money),
            typeof(EntityReference),
            typeof(OptionSetValue),
            typeof(AliasedValue),
            typeof(OptionSetValueCollection)
        };

        public EntityDataToCsvHelper(IEnumerable<Entity> entityList)
        {
            dataStore = new DataTable();
            PopulateData(entityList);
        }

        private void AddColumnIfRequired(string columnName)
        {
            if (!dataStore.Columns.Contains(columnName))
            {
                dataStore.Columns.Add(columnName);
            }
        }

        private void PopulateData(IEnumerable<Entity> entityList)
        {
            foreach (var entity in entityList)
            {
                var dataRow = dataStore.NewRow();
                foreach (var attribute in entity.Attributes)
                {
                    AddColumnIfRequired(attribute.Key);
                    dataRow[attribute.Key] = _types.Contains(attribute.Value.GetType()) ? SerializeCrmDataTypes(attribute.Value) : attribute.Value.ToString();
                }
                dataStore.Rows.Add(dataRow);
            }
        }

        private string SerializeCrmDataTypes(object value)
        {
            if (value is Money money)
            {
                return money.Value.ToString();
            }
            else if (value is OptionSetValue optionSetValue)
            {
                return optionSetValue.Value.ToString();
            }
            else if (value is EntityReference entityReference)
            {
                return entityReference.Id.ToString();
            }
            else if (value is OptionSetValueCollection optionSetValueCollection)
            {
                return string.Join(",", optionSetValueCollection.Select(option => option.Value).ToList());
            }
            else if (value is DateTime datetime)
            {
                return datetime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else if (value is AliasedValue aliasedValue)
            {
                return SerializeCrmDataTypes(aliasedValue.Value);
            }
            else
            {
                return value.ToString();
            }
        }
        public string ConvertToCsv()
        {
            var csv = new StringBuilder();
            var columnNames = dataStore.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            csv.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in dataStore.Rows)
            {
                var fields = row.ItemArray.Select(field =>
                {
                    return $"\"{field.ToString().Replace("\"", "\"\"")}\"";
                });
                csv.AppendLine(string.Join(",", fields));
            }
            return csv.ToString();
        }
    }
}
