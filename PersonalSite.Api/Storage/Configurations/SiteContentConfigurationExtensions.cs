using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain;

namespace PersonalSite.Api.Storage.Configurations
{
    public static class SiteContentConfigurationExtensions
    {
        public static void ConfigureSiteContent<T>(
            this EntityTypeBuilder<T> builder)
            where T : SiteContent
        {
            builder.Property(x => x.Source);

            builder.ComplexProperty(x => x.Created);

            builder.ComplexProperty(x => x.Edited);
        }
    }
}