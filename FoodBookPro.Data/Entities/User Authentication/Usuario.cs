namespace FoodBookPro.Data.Entities
{
    public enum RolUsuario
    {
        Cliente = 1,
        Propietario = 2
    }

    public abstract class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public RolUsuario Rol { get; protected set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }

}
