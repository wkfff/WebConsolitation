using System.Collections.Generic;

namespace Krista.FM.Utils.MakeConfigurationLibrary
{
    public class BaseObject
    {
        public BaseObject(string name)
        {
            Name = name;
            // п умолчанию все пакеты входят в конфигурацию
            IsContain = true;
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool IsContain { get; set; }
    }

    public class PackageConfig : BaseObject
    {
        public PackageConfig(string name) : base(name)
        {
            Packages = new List<PackageConfig>();
            Items = new List<ItemConfig>();
        }

        public List<PackageConfig> Packages { get; set; }

        public List<ItemConfig> Items { get; set; }
    }

    public class ItemConfig : BaseObject
    {
        public ItemConfig(string name) : base(name)
        {
        }

        public ItemConfig(string name, string path)
            : this(name)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
