namespace TESTPROJESI.Models
{
    public class ModuleConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public string Endpoint { get; set; }
        public string Controller { get; set; }
        public bool Enabled { get; set; } = true;
        public List<FieldConfiguration> Fields { get; set; } = new();
    }

    public class FieldConfiguration
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; } = "text";
        public bool Required { get; set; }
        public string Placeholder { get; set; }
        public object DefaultValue { get; set; }
    }

    public static class ModuleRegistry
    {
        public static List<ModuleConfiguration> Modules = new()
        {
            new ModuleConfiguration
            {
                Name = "Cari",
                DisplayName = "Cari Kartları",
                Icon = "bi-people",
                Endpoint = "ARPs",
                Controller = "Cari",
                Fields = new List<FieldConfiguration>
                {
                    new() { Name = "CARI_KOD", Label = "Cari Kodu", Required = true },
                    new() { Name = "CARI_ISIM", Label = "Cari İsmi", Required = true },
                    new() { Name = "CARI_TEL", Label = "Telefon", Type = "tel" },
                    new() { Name = "CARI_IL", Label = "İl" },
                    new() { Name = "EMAIL", Label = "E-posta", Type = "email" }
                }
            },
            new ModuleConfiguration
            {
                Name = "Stok",
                DisplayName = "Stok Kartları",
                Icon = "bi-box",
                Endpoint = "Items",
                Controller = "Stok",
                Fields = new List<FieldConfiguration>
                {
                    new() { Name = "STOK_KOD", Label = "Stok Kodu", Required = true },
                    new() { Name = "STOK_ADI", Label = "Stok Adı", Required = true },
                    new() { Name = "BIRIM", Label = "Birim" },
                    new() { Name = "FIYAT", Label = "Fiyat", Type = "number" }
                }
            },
            new ModuleConfiguration
            {
                Name = "Fatura",
                DisplayName = "Faturalar",
                Icon = "bi-receipt",
                Endpoint = "Invoices",
                Controller = "Fatura",
                Fields = new List<FieldConfiguration>
                {
                    new() { Name = "FATURA_NO", Label = "Fatura No", Required = true },
                    new() { Name = "TARIH", Label = "Tarih", Type = "date", Required = true },
                    new() { Name = "CARI_KOD", Label = "Cari Kodu", Required = true },
                    new() { Name = "TUTAR", Label = "Tutar", Type = "number" }
                }
            }
        };

        public static ModuleConfiguration GetModule(string name)
        {
            return Modules.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static void RegisterModule(ModuleConfiguration module)
        {
            if (!Modules.Any(m => m.Name == module.Name))
            {
                Modules.Add(module);
            }
        }
    }
}
