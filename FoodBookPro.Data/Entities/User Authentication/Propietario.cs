using FoodBookPro.Data.Base;

namespace FoodBookPro.Data.Entities
{
    public class Propietario : Usuario
    {
        public string RestauranteNombre { get; set; } = string.Empty;

        public EstadoPropietario Estado { get; set; }

        public Propietario()
        {
            Rol = RolUsuario.Propietario;
            Estado = EstadoPropietario.Pendiente;
        }
    }
}