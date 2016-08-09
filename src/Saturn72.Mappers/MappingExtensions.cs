namespace Saturn72.Mappers
{
    public static class MappingExtensions
    {
        public static TDestination Map<TSource, TDestination>(this TSource source) 
            where TSource : class 
            where TDestination : class

        {
            return SimpleMapper.Map<TSource, TDestination>(source);
        }

        public static void Map<TSource, TDestination>(this TSource source, TDestination destination)
           where TSource : class
           where TDestination : class

        {
            SimpleMapper.Map<TSource, TDestination>(source, destination);
        }
    }
}
