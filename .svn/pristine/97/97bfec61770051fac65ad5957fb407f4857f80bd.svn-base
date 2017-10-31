namespace Krista.FM.Common
{
    public static class BoolParser
    {
        public static bool GetValue(string value)
        {
            return IsTrue(value);
        }

        public static bool IsFalse(string value)
        {
            return !IsTrue(value);
        }

        public static bool IsTrue(string value)
        {
            try
            {
                if (value == null)
                {
                    return false;
                }

                value = value.Trim();

                value = value.ToLower();

                if (value == "true")
                {
                    return true;
                }

                if (value == "1")
                {
                    return true;
                }

                if (value == "yes")
                {
                    return true;
                }

                if (value == "y")
                {
                    return true;
                }

                if (value == "да")
                {
                    return true;
                }

                if (value == "+")
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}