﻿using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Mantle.DictionaryStorage.Azure.Entities
{
    public class AzureTableDictionaryStorageEntity<T> : ITableEntity
        where T : class, new()
    {
        private readonly TypeMetadata typeMetadata;

        public AzureTableDictionaryStorageEntity()
        {
            typeMetadata = new TypeMetadata(typeof (T));
        }

        public T Data { get; set; }
        public string ETag { get; set; }
        public string PartitionKey { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            var t = new T();

            foreach (var outputProperty in typeMetadata.Properties)
            {
                if (properties.ContainsKey(outputProperty.PropertyInfo.Name))
                {
                    var inputProperty = properties[outputProperty.PropertyInfo.Name];
                    var propertyType = outputProperty.PropertyInfo.PropertyType;

                    if ((propertyType == typeof (bool)) && (inputProperty.BooleanValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.BooleanValue.Value);
                    else if ((propertyType == typeof (bool?)) && (inputProperty.BooleanValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.BooleanValue);
                    else if ((propertyType == typeof (byte)) && (inputProperty.Int32Value.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, ((byte) (inputProperty.Int32Value.Value)));
                    else if ((propertyType == typeof (byte?)) && (inputProperty.Int32Value.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, ((byte) (inputProperty.Int32Value.Value)));
                    else if ((propertyType == typeof (byte[])) && (inputProperty.BinaryValue != null))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.BinaryValue);
                    else if ((propertyType == typeof (decimal)) && (inputProperty.DoubleValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, ((decimal) (inputProperty.DoubleValue.Value)));
                    else if ((propertyType == typeof (decimal?)) && (inputProperty.DoubleValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, ((decimal) (inputProperty.DoubleValue.Value)));
                    else if ((propertyType == typeof (DateTime)) && (inputProperty.DateTime.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.DateTime.Value);
                    else if ((propertyType == typeof (DateTime?)) && (inputProperty.DateTime.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.DateTime);
                    else if ((propertyType == typeof (double)) && (inputProperty.DoubleValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.DoubleValue.Value);
                    else if ((propertyType == typeof (double?)) && (inputProperty.DoubleValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.DoubleValue);
                    else if ((propertyType == typeof (float)) && (inputProperty.DoubleValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, ((float) (inputProperty.DoubleValue.Value)));
                    else if ((propertyType == typeof (float?)) && (inputProperty.DoubleValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, ((float) (inputProperty.DoubleValue.Value)));
                    else if ((propertyType == typeof (Guid)) && (inputProperty.GuidValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.GuidValue.Value);
                    else if ((propertyType == typeof (Guid?)) && (inputProperty.GuidValue.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.GuidValue);
                    else if ((propertyType == typeof (int)) && (inputProperty.Int32Value.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.Int32Value.Value);
                    else if ((propertyType == typeof (int?)) && (inputProperty.Int32Value.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.Int32Value);
                    else if ((propertyType == typeof (long)) && (inputProperty.Int64Value.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.Int64Value.Value);
                    else if ((propertyType == typeof (long?)) && (inputProperty.Int64Value.HasValue))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.Int64Value);
                    else if ((propertyType == typeof (string)) && (inputProperty.StringValue != null))
                        outputProperty.PropertyInfo.SetValue(t, inputProperty.StringValue);
                    else if (inputProperty.StringValue != null)
                        outputProperty.PropertyInfo.SetValue(t,
                                                             JsonConvert.DeserializeObject(inputProperty.StringValue,
                                                                                           propertyType));
                }
            }

            Data = t;
        }

        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var dictionary = new Dictionary<string, EntityProperty>();

            foreach (var inputProperty in typeMetadata.Properties)
            {
                var propertyName = inputProperty.PropertyInfo.Name;
                var propertyType = inputProperty.PropertyInfo.PropertyType;
                var propertyValue = inputProperty.PropertyInfo.GetValue(Data);

                if (propertyValue != null)
                {
                    if (propertyType == typeof (bool))
                        dictionary[propertyName] = new EntityProperty((bool) (propertyValue));
                    else if (propertyType == typeof (bool?))
                        dictionary[propertyName] = new EntityProperty((bool?) (propertyValue));
                    else if (propertyType == typeof (byte))
                        dictionary[propertyName] = new EntityProperty((byte) (propertyValue));
                    else if (propertyType == typeof (byte?))
                        dictionary[propertyName] = new EntityProperty((byte?) (propertyValue));
                    else if (propertyType == typeof (byte[]))
                        dictionary[propertyName] = new EntityProperty((byte[]) (propertyValue));
                    else
                        dictionary[propertyName] =
                            new EntityProperty(JsonConvert.SerializeObject(propertyValue, Formatting.Indented));
                }

                return dictionary;
            }
        }
    }