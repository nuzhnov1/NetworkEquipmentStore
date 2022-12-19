using System;
using System.Text;

namespace NetworkEquipmentStore.Pages.Helpers
{
    public static class EncoderHelper
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
