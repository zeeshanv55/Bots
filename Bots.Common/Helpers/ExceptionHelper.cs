namespace Bots.Common.Helpers
{
    using System;
    using System.Text;

    public static class ExceptionHelper
    {
        public static string GetAllExceptionText(Exception e)
        {
            var sb = new StringBuilder();
            var notFirst = false;

            while (e != null)
            {
                if (notFirst)
                {
                    sb.Append("Inner: ");
                }

                notFirst = true;

                sb.Append(e.GetType().Name);
                sb.Append(": ");

                sb.AppendLine(e.Message);
                e = e.InnerException;
            }

            return sb.ToString();
        }
	}
}
