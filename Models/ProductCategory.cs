namespace NetworkEquipmentStore.Models
{
    public enum ProductCategory
    {
        NONE,
        ROUTER,
        SWITCH,
        CABEL,
        ADAPTER,
        OTHER
    }

    public static class ProductCategoryExtensions
    {
        public static string ToWebRepresentation(this ProductCategory category)
        {
            switch (category)
            {
                case ProductCategory.NONE: return "Все товары";
                case ProductCategory.ROUTER: return "Маршрутизаторы";
                case ProductCategory.SWITCH: return "Коммутаторы";
                case ProductCategory.CABEL: return "Кабели";
                case ProductCategory.ADAPTER: return "Адаптеры";
                default: return "Другое";
            }
        }

        public static string ToUrlRepresentation(this ProductCategory category)
        {
            switch (category)
            {
                case ProductCategory.NONE: return "";
                case ProductCategory.ROUTER: return "routers";
                case ProductCategory.SWITCH: return "switchers";
                case ProductCategory.CABEL: return "cabels";
                case ProductCategory.ADAPTER: return "adapters";
                default: return "other";
            }
        }

        public static ProductCategory ToProductCategoryFromUrl(this string url)
        {
            switch (url)
            {
                case "routers": return ProductCategory.ROUTER;
                case "switchers": return ProductCategory.SWITCH;
                case "cabels": return ProductCategory.CABEL;
                case "adapters": return ProductCategory.ADAPTER;
                case "other": return ProductCategory.OTHER;
                default: return ProductCategory.NONE;
            }
        }
    }
}
