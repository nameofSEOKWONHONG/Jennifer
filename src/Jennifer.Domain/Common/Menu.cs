using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Common;

public class Menu : IAuditable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public string Url { get; private set; }
    public Guid? ParentId { get; private set; }
    public Menu Parent { get; private set; }
    public ICollection<Menu> Children { get; private set; } = new List<Menu>();
    public int Order { get; private set; }
    public bool IsVisible { get; private set; } = true;

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }

    private Menu() { }

    public static Menu Create(string name, string icon, string url, Guid? parentId = null, int order = 0)
    {
        return new Menu
        {
            Name = name,
            Icon = icon,
            Url = url,
            ParentId = parentId,
            Order = order,
            IsVisible = true
        };
    }

    public void Update(string name, string icon, string url,bool isVisible, Guid? parentId = null, int order = 0)
    {
        Name = name;
        Icon = icon;
        Url = url;       
        IsVisible = isVisible;
    }
}

public class MenuEntityConfiguration: IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus", "common");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Icon)
            .HasMaxLength(255);

        builder.Property(m => m.Url)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(m => m.Order)
            .IsRequired();

        builder.Property(m => m.IsVisible)
            .IsRequired();

        builder.Property(m => m.CreatedOn).IsRequired();
        builder.Property(m => m.CreatedBy).IsRequired();
        builder.Property(m => m.ModifiedOn);
        builder.Property(m => m.ModifiedBy);
        
        builder.HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);        
    }
}