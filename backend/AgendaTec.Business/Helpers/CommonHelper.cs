namespace AgendaTec.Business.Helpers
{
    public static class CommonHelper
    {
        public static string RemoveLastOccurrence(string originalString, string text)
        {
            var result = string.Empty;

            var lastOccurrence = originalString.LastIndexOf(text);

            if (lastOccurrence < 1)
                return originalString;

            return 
                (originalString.Length - text.Length).Equals(lastOccurrence) 
                ? originalString.Substring(0, originalString.Length - text.Length) 
                : originalString;            
        }
    }
}
