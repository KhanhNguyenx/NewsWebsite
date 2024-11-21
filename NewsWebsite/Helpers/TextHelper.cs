namespace NewsWebsite.Helpers
{
    public static class TextHelper
    {
        /// <summary>
        /// Truncate text to a specific number of words and append "..." if exceeded.
        /// </summary>
        public static string TruncateText(string input, int wordLimit)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var words = input.Split(' ');
            if (words.Length <= wordLimit)
                return input;

            return string.Join(" ", words.Take(wordLimit)) + "...";
        }
    }
}
