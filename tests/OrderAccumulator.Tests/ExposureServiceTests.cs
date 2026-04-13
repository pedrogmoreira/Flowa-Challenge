using Microsoft.Extensions.Logging.Abstractions;
using OrderAccumulator.Application;

namespace OrderAccumulator.Tests;

public class ExposureServiceTests
{
    private readonly ExposureService _sut;

    public ExposureServiceTests()
    {
        _sut = new ExposureService(NullLogger<ExposureService>.Instance);
    }

    [Fact]
    public void Apply_BuyOrder_IncreasesExposure()
    {
        _sut.Apply("PETR4", '1', 10.50m, 100m);

        var exposure = _sut.GetExposure("PETR4");
        Assert.Equal(1050.00m, exposure);
    }

    [Fact]
    public void Apply_SellOrder_DecreasesExposure()
    {
        _sut.Apply("PETR4", '2', 10.50m, 100m);

        var exposure = _sut.GetExposure("PETR4");
        Assert.Equal(-1050.00m, exposure);
    }

    [Fact]
    public void Apply_BuyThenSell_CalculatesNetExposure()
    {
        _sut.Apply("PETR4", '1', 10.50m, 200m);
        _sut.Apply("PETR4", '2', 10.50m, 100m);

        var exposure = _sut.GetExposure("PETR4");
        Assert.Equal(1050.00m, exposure);
    }

    [Fact]
    public void Apply_MultipleSymbols_TracksExposureIndependently()
    {
        _sut.Apply("PETR4", '1', 10.50m, 100m);
        _sut.Apply("VALE3", '1', 20.00m, 50m);

        Assert.Equal(1050.00m, _sut.GetExposure("PETR4"));
        Assert.Equal(1000.00m, _sut.GetExposure("VALE3"));
    }

    [Fact]
    public void GetExposure_UnknownSymbol_ReturnsZero()
    {
        var exposure = _sut.GetExposure("VIIA4");
        Assert.Equal(0m, exposure);
    }
}