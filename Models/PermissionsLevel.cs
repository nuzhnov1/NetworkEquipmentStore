namespace NetworkEquipmentStore.Models
{
    public enum PermissionsLevel
    {
        CLIENT,
        ADMIN
    }

    public static class PermissionsLevelExtensions
    {
        public static string ToWebRepresentation(this PermissionsLevel level)
        {
            switch (level)
            {
                case PermissionsLevel.ADMIN: return "Администратор";
                default: return "Клиент";
            }
        }
    }
}
