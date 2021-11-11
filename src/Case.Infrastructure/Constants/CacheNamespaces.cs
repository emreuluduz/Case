namespace Case.Infrastructure.Constants
{
    public static class CacheNamespaces
    {
        public const string Posts = "posts";
        public static string SliderPosts(string slug) => $"{slug}_posts";
    }
}
