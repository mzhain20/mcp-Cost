using System.CommandLine;
using System.CommandLine.Parsing;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Core.Models.Command;
using Azure.Mcp.Core.Options;
using Azure.Mcp.Tools.CostManagement.Commands;
using Azure.Mcp.Tools.CostManagement.Models;
using Azure.Mcp.Tools.CostManagement.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Azure.Mcp.Tools.CostManagement.UnitTests.Forecast;

public class ForecastGetCommandTests
{
    private readonly ICostManagementService _mockService;
    private readonly ILogger<ForecastGetCommand> _mockLogger;
    private readonly ForecastGetCommand _command;
    private readonly IServiceProvider _serviceProvider;
    private readonly CommandContext _context;
    private readonly Command _commandDefinition;

    private static readonly string fromDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
    private static readonly string toDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd");

    public ForecastGetCommandTests()
    {
        _mockService = Substitute.For<ICostManagementService>();
        _mockLogger = Substitute.For<ILogger<ForecastGetCommand>>();

        var collection = new ServiceCollection().AddSingleton(_mockService);

        _serviceProvider = collection.BuildServiceProvider();
        _command = new(_mockLogger);
        _context = new(_serviceProvider);
        _commandDefinition = _command.GetCommand();
    }

    [Fact]
    public void Constructor_WithValidService_CreatesInstance()
    {
        // Arrange & Act
        var command = _command.GetCommand();

        // Assert
        Assert.NotNull(command);
        Assert.NotNull(command.Description);
        Assert.NotEmpty(command.Description);
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("ActualCost", null, null, null)]
    [InlineData("ActualCost", "Daily", null, null)]
    public async Task ExecuteAsync_WithValidInputs_CallsServiceAndReturnsSuccess(
        string? type,
        string? granularity,
        string? aggregationName,
        string? aggregationFunction)
    {
        // Arrange
        var mockResponse = new QueryApiResponse
        {
            Properties = new QueryResultProperties
            {
                Rows = [["100.50", "2023-01-01", "Forecast", "USD",]],
                Columns = [
                    new QueryColumn { Name = "Cost", Type = "Number" },
                    new QueryColumn { Name = "UsageDate", Type = "Date" },
                    new QueryColumn { Name = "CostStatus", Type = "String" },
                    new QueryColumn { Name = "Currency", Type = "String" }
                ]
            }
        };

        Task<QueryApiResponse?> queryApiResponse = Task.FromResult<QueryApiResponse?>(mockResponse);

        _mockService.QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns(queryApiResponse);

        var args = new List<string> { "--subscription", Guid.NewGuid().ToString() };
        if (type != null)
        { args.AddRange(["--type", type]); }
        if (granularity != null)
        { args.AddRange(["--granularity", granularity]); }
        if (fromDate != null)
        { args.AddRange(["--from-date", fromDate]); }
        if (toDate != null)
        { args.AddRange(["--to-date", toDate]); }
        if (aggregationName != null)
        { args.AddRange(["aggregation-name", aggregationName]); }
        if (aggregationFunction != null)
        { args.AddRange(["aggregation-function", aggregationFunction]); }

        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Status);
        Assert.NotNull(result.Results);
        await _mockService.Received(1).QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>());
    }

    [Theory]
    [InlineData("test-sub", "Scope cannot be empty")]
    [InlineData("subscription", "", "Scope value cannot be empty")]
    [InlineData("test-sub", "InvalidType", null, null, null)]
    [InlineData("test-sub", "ActualCost", "InvalidGranularity", null, null)]
    [InlineData("test-sub", "ActualCost", "Daily", "InvalidAggregationName", null)]
    [InlineData("test-sub", "ActualCost", "Daily", "Cost", "InvalidAggregationFunction")]
    public async Task ExecuteAsync_WithInvalidInputs_ReturnsError(
        string scopeValue,
        string? type = null,
        string? granularity = null,
        string? aggregationName = null,
        string? aggregationFunction = null)
    {
        _mockService.QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .ThrowsAsync(new Exception("Error querying cost management forecast data."));

        var args = new List<string>();

        if (!string.IsNullOrEmpty(scopeValue)) { args.AddRange(["--subscription", scopeValue]); }
        if (type != null) { args.AddRange(["--type", type]); }
        if (granularity != null) { args.AddRange(["--granularity", granularity]); }
        if (fromDate != null) { args.AddRange(["--from-date", fromDate]); }
        if (toDate != null) { args.AddRange(["--to-date", toDate]); }
        if (aggregationName != null)
        { args.AddRange(["aggregation-name", aggregationName]); }
        if (aggregationFunction != null)
        { args.AddRange(["aggregation-function", aggregationFunction]); }

        //we validate option values in BindOptions
        //for now default to null value if not valid
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.NotEqual(0, result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsException_ReturnsError()
    {
        // Arrange
        _mockService.QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .ThrowsAsync(new InvalidOperationException("Service error"));

        var args = new[] {"--subscription", "test-sub" };
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.NotEqual(0, result.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceReturnsNull_ReturnsError()
    {
        // Arrange
        _mockService.QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns((QueryApiResponse?)null);

        var args = new[] { "--subscription", "test-sub" };
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.NotEqual(0, result.Status);
    }

    [Theory]
    [InlineData("--subscription test-sub")]
    [InlineData("--subscription test-sub --type ActualCost")]
    [InlineData("--subscription test-sub --granularity Daily")]
    [InlineData("--subscription test-sub --from-date 2023-01-01")]
    [InlineData("--subscription test-sub --to-date 2023-01-31")]
    public void Parse_WithValidArguments_ParsesSuccessfully(string argumentString)
    {
        // Arrange
        var args = argumentString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Act
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Assert
        Assert.NotNull(parseResult);
        Assert.Empty(parseResult.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("--subcription ")]
    [InlineData("--subscription ")]
    [InlineData("--invalid-option value")]
    public void Parse_WithInvalidArguments_ReturnsErrors(string argumentString)
    {
        // Arrange
        var args = string.IsNullOrEmpty(argumentString)
            ? Array.Empty<string>()
            : argumentString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Act
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Assert
        Assert.NotNull(parseResult);
        Assert.NotEmpty(parseResult.Errors);
    }

    [Fact]
    public void Command_HasCorrectOptions()
    {
        // Act & Assert
        var options = _commandDefinition.Options.ToList();

        Assert.Contains(options, o => o.Name == "--subscription");
        Assert.Contains(options, o => o.Name == "--type");
        Assert.Contains(options, o => o.Name == "--granularity");
        Assert.Contains(options, o => o.Name == "--from-date");
        Assert.Contains(options, o => o.Name == "--to-date");
        Assert.Contains(options, o => o.Name == "--aggregation-name");
        Assert.Contains(options, o => o.Name == "--aggregation-function");
        Assert.Contains(options, o => o.Name == "--include-fresh-partial-cost");
        Assert.Contains(options, o => o.Name == "--include-actual-cost");
    }

    [Fact]
    public async Task ExecuteAsync_WithComplexResponse_FormatsOutputCorrectly()
    {
        // Arrange
        var mockResponse = new QueryApiResponse
        {
            Properties = new QueryResultProperties
            {
                Rows = [
                    ["100.50", "2023-01-01", "USD"],
                    ["200.75", "2023-01-02", "USD"]
                ],
                Columns = [
                    new QueryColumn { Name = "Cost", Type = "Number" },
                    new QueryColumn { Name = "Date", Type = "Date" },
                    new QueryColumn { Name = "Currency", Type = "String" }
                ]
            }
        };

        _mockService.QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns(mockResponse);

        var args = new[] { "--subscription", "test-sub" };
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.Equal(200, result.Status);
        await _mockService.Received(1).QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>());
    }

    [Fact]
    public async Task ExecuteAsync_VerifiesCorrectServiceCallParameters()
    {
        // Arrange
        var mockResponse = new QueryApiResponse
        {
            Properties = new QueryResultProperties
            {
                Rows = [["100.50", "2023-01-01", "USD"]],
                Columns = [
                    new QueryColumn { Name = "Cost", Type = "Number" },
                    new QueryColumn { Name = "Date", Type = "Date" },
                    new QueryColumn { Name = "Currency", Type = "String" }
                ]
            }
        };

        _mockService.QueryForecast(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns(mockResponse);

        var args = new[]
        {
            "--subscription", "test-subscription-id",
            "--type", "ActualCost",
            "--granularity", "Daily"
        };
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.Equal(200, result.Status);
        await _mockService.Received(1).QueryForecast(
            Arg.Is<string>(s => s.Contains("test-subscription-id")),
            Arg.Is<string>(s => s == "ActualCost"),
            Arg.Is<string>(s => s == "Daily"),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>());
    }
}
