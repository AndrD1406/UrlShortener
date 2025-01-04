using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlShortener.DataAccess.Models.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Urls)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)  
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
