using System.Text;
using System.Text.RegularExpressions;

namespace NewsWebsite.Helpers
{
    public static class TextHelper
    {
        /// <summary>
        /// Truncate text to a specific number of words and append "..." if exceeded.
        /// Removes HTML tags, decodes HTML entities, and fixes encoding issues.
        /// </summary>
        public static string TruncateText(string input, int wordLimit)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Loại bỏ các thẻ HTML
            string plainText = Regex.Replace(input, "<.*?>", string.Empty);

            // Giải mã các ký tự HTML
            plainText = System.Net.WebUtility.HtmlDecode(plainText);

            // Chuyển chuỗi về UTF-8 để đảm bảo encoding đúng
            plainText = Encoding.UTF8.GetString(Encoding.Default.GetBytes(plainText));

            // Tách từ và giới hạn số lượng
            var words = plainText.Split(' ');
            if (words.Length <= wordLimit)
                return plainText;

            return string.Join(" ", words.Take(wordLimit)) + "...";
        }
    }
}
