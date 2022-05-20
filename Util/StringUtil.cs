namespace GeekDesk.Util
{
    public class StringUtil
    {


        public static bool IsEmpty(object str)
        {
            if (str == null || str.ToString().Length == 0 || str.ToString().Trim().Length == 0)
            {
                return true;
            }
            return false;
        }
    }
}
