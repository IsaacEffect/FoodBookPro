namespace FoodBookPro.Data.Entities
{
    public class Cliente : Usuario
    {
        public Cliente()
        {
            Rol = RolUsuario.Cliente;
        }
    }
}
