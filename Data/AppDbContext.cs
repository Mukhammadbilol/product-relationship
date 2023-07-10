using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductDetails> ProductDetails { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>()
            .HasKey(p => p.Id);
        
        builder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Entity<Product>()
            .Property(p => p.Price)
            .IsRequired();

        builder.Entity<Product>()
            .Property(p => p.Status)
            .IsRequired();

        builder.Entity<ProductDetails>()
            .Property(u => u.Description)
            .HasMaxLength(125);

        builder.Entity<ProductDetails>()
            .Property(u => u.Color)
            .HasMaxLength(100);

        builder.Entity<ProductDetails>()
            .Property(u => u.Weight)
            .HasMaxLength(100);

        builder.Entity<ProductDetails>()
            .Property(u => u.Size)
            .HasMaxLength(100);

        builder.Entity<ProductDetails>()
            .Property(u => u.Manufacturer)
            .HasMaxLength(100);

        builder.Entity<ProductDetails>()
            .Property(u => u.CountryOfOrigin)
            .HasMaxLength(100);

        builder.Entity<Product>()
            .HasOne(d => d.ProductDetails)
            .WithOne(u => u.Product)
            .HasForeignKey<ProductDetails>(u => u.Id)
            .HasPrincipalKey<Product>(u => u.Id);

        base.OnModelCreating(builder);
    }
}