namespace CommonCode.BusinessLayer.Helpers
{
    public static class Validate
    {
        public static bool IsValidEmailAddress(string address)
        {
            try
            {
                return new System.Net.Mail.MailAddress(address).Address.Equals(address);
            }
            catch
            {
                return false;
            }
        }
    }
}