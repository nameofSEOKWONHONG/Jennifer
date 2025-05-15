// using Jennifer.SharedKernel.Consts;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Jennifer.Jwt.Models;
//
// public class UserZone
// {
//     public string ZoneId { get; set; }
//     public string UserId { get; set; }
//     public User User { get; set; }
//     
//     public class UserZoneEntityConfiguration: IEntityTypeConfiguration<UserZone>
//     {
//         public void Configure(EntityTypeBuilder<UserZone> builder)
//         {
//             builder.ToTable($"{nameof(UserZone)}s", EntitySettings.Schema);
//             
//             builder.HasKey(m => new{m.ZoneId, m.UserId});
//         }
//     }
// }