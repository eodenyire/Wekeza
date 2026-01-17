public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.KraPin).HasMaxLength(11);
        builder.Property(c => c.RegistrationNumber).HasMaxLength(50);

        // Store Directors as a JSON column (2026 Modern PostgreSQL approach)
        builder.OwnsMany(c => c.Directors, d =>
        {
            d.ToJson();
            d.Property(x => x.FullName).IsRequired();
        });
    }
}
