using FoodBookPro.Data.Entities;
using Xunit;

namespace FoodBookPro.Test.Entities;

public class EstadoOrdenTests
{
    [Theory]
    [InlineData(EstadoOrden.Pendiente, 0)]
    [InlineData(EstadoOrden.Confirmada, 1)]
    [InlineData(EstadoOrden.Preparando, 2)]
    [InlineData(EstadoOrden.Lista, 3)]
    [InlineData(EstadoOrden.Completada, 4)]
    [InlineData(EstadoOrden.Cancelada, 5)]
    public void EstadoOrden_TieneValoresCorrectos(EstadoOrden estado, int valorEsperado)
    {
        Assert.Equal(valorEsperado, (int)estado);
    }

    [Fact]
    public void EstadoOrden_ExistenSeisEstados()
    {
        var estados = Enum.GetValues<EstadoOrden>();
        Assert.Equal(6, estados.Length);
    }
}
