using System.Linq;
using System.Collections.Generic;
using NewsAPI.DTOs;

namespace NewsWebsite.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Get the image URL for a given post.
        /// </summary>
        /// <param name="postId">ID of the post</param>
        /// <param name="imageList">List of images fetched from the API</param>
        /// <returns>URL of the image or default image path</returns>
        public static string GetImageUrl(int postId, List<ImageDTO> imageList)
        {
            if (imageList == null || imageList.Count == 0)
                return "/DATA/Images/default.jpg"; // Path to your default image

            // Get the first image for the given PostId
            var image = imageList.FirstOrDefault(img => img.PostId == postId);

            if (image != null && !string.IsNullOrEmpty(image.ImageUrl))
            {
                return $"{image.ImageUrl}";
            }

            return "/DATA/Images/default.jpg"; // Fallback to default image
        }
    }
}
