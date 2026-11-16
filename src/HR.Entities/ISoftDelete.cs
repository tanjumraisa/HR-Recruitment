namespace HR.Entities
{
    // Entities with this interface are never removed from the database:
    // Delete sets IsDeleted = true and every query hides them (global query filter).
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
