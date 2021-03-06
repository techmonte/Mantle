﻿using System.Collections.Generic;

namespace Mantle.Configuration
{
    public class ConfigurableObject<T>
    {
        public ConfigurableObject()
        {
            Properties = new List<ConfigurableProperty>();
        }

        public string Name { get; set; }

        public List<ConfigurableProperty> Properties { get; set; }
        public T Target { get; set; }
        public TypeMetadata TypeMetadata { get; set; }
    }
}