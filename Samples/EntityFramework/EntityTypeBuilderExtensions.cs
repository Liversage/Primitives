using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFramework
{
    static class EntityTypeBuilderExtensions
    {
        public static PropertyBuilder UseFieldInsteadOfProperty<T>(this EntityTypeBuilder<T> builder, string fieldName, string propertyName) where T : class
        {
            builder.Ignore(propertyName);
            return builder.Property(fieldName)
                .HasColumnName(propertyName);
        }

        public static PropertyBuilder UseFieldInsteadOfPropertyAsKey<T>(this EntityTypeBuilder<T> builder, string fieldName, string propertyName) where T : class
        {
            builder.Ignore(propertyName);
            builder.HasKey(fieldName);
            return builder
                .Property(fieldName).HasColumnName(propertyName)
                .ValueGeneratedOnAdd();
        }
    }
}
