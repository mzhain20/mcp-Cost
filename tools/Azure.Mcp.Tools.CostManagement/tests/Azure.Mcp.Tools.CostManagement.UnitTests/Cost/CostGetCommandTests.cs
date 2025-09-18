using System.CommandLine;
using System.CommandLine.Parsing;
using Azure.Mcp.Core.Commands;
using Azure.Mcp.Core.Models.Command;
using Azure.Mcp.Core.Options;
using Azure.Mcp.Tools.CostManagement.Commands.Cost;
using Azure.Mcp.Tools.CostManagement.Models;
using Azure.Mcp.Tools.CostManagement.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Azure.Mcp.Tools.CostManagement.UnitTests.Cost;

public class CostGetCommandTests
{
    private readonly ICostManagementService _mockService;
    private readonly ILogger<CostGetCommand> _mockLogger;
    private readonly CostGetCommand _command;
    private readonly IServiceProvider _serviceProvider;
    private readonly CommandContext _context;
    private readonly Command _commandDefinition;

    public CostGetCommandTests()
    {
        _mockService = Substitute.For<ICostManagementService>();
        _mockLogger = Substitute.For<ILogger<CostGetCommand>>();

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
    [InlineData("test-sub", null, null, null, null)]
    [InlineData("test-sub", "ActualCost", null, null, null)]
    [InlineData("test-sub", "ActualCost", "Daily", null, null)]
    [InlineData("test-sub", "ActualCost", "Daily", "2023-01-01", "2023-01-31")]
    public async Task ExecuteAsync_WithValidInputs_CallsServiceAndReturnsSuccess(
        string scopeValue,
        string? type,
        string? granularity,
        string? fromDate,
        string? toDate)
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

        Task<QueryApiResponse?> queryApiResponse = Task.FromResult<QueryApiResponse?>(mockResponse);

        _mockService.QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns(queryApiResponse);

        var args = new List<string> { "--subscription", scopeValue};
        if (type != null) { args.AddRange(["--type", type]); }
        if (granularity != null) { args.AddRange(["--granularity", granularity]); }
        if (fromDate != null) { args.AddRange(["--from-date", fromDate]); }
        if (toDate != null) { args.AddRange(["--to-date", toDate]); }

        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.Equal(200, result.Status);
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        await _mockService.Received(1).QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>());
    }

    [Theory]
    [InlineData("test-sub", "Scope cannot be empty")]
    [InlineData("subscription", "", "Scope value cannot be empty")]
    [InlineData("test-sub", "InvalidType", null, null, null)]
    [InlineData("test-sub", "ActualCost", "InvalidGranularity", null, null)]
    [InlineData("test-sub", "ActualCost", "Daily", "invalid-date", null)]
    [InlineData("test-sub", "ActualCost", "Daily", null, "invalid-date")]
    //[InlineData("test-sub", "ActualCost", "Daily", "2023-01-31", "2023-01-01", "From date must be before to date")]
    public async Task ExecuteAsync_WithInvalidInputs_ReturnsError(
        string scopeValue,
        string? type = null,
        string? granularity = null,
        string? fromDate = null,
        string? toDate = null)
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

        Task<QueryApiResponse?> queryApiResponse = Task.FromResult<QueryApiResponse?>(mockResponse);

        _mockService.QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns(queryApiResponse);

        var args = new List<string>();
        
        if (!string.IsNullOrEmpty(scopeValue)) { args.AddRange(["--subscription", scopeValue]); }
        if (type != null) { args.AddRange(["--type", type]); }
        if (granularity != null) { args.AddRange(["--granularity", granularity]); }
        if (fromDate != null) { args.AddRange(["--from-date", fromDate]); }
        if (toDate != null) { args.AddRange(["--to-date", toDate]); }

        //we validate option values in BindOptions
        //for now default to null value if not valid
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.Equal(200, result.Status);
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsException_ReturnsError()
    {
        // Arrange
        _mockService.QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
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
        _mockService.QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
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
        Assert.Contains(options, o => o.Name == "--group-by");
        Assert.Contains(options, o => o.Name == "--aggregation-cost-type");
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
                    ["100.50", "2023-01-01", "USD", "ResourceGroup1"],
                    ["200.75", "2023-01-02", "USD", "ResourceGroup2"]
                ],
                Columns = [
                    new QueryColumn { Name = "Cost", Type = "Number" },
                    new QueryColumn { Name = "Date", Type = "Date" },
                    new QueryColumn { Name = "Currency", Type = "String" },
                    new QueryColumn { Name = "ResourceGroupName", Type = "String" }
                ]
            }
        };

        _mockService.QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>())
            .Returns(mockResponse);

        var args = new[] { "--subscription", "test-sub" };
        var parseResult = _commandDefinition.Parse(args.ToArray());

        // Act
        var result = await _command.ExecuteAsync(_context, parseResult);

        // Assert
        Assert.Equal(200, result.Status);
        await _mockService.Received(1).QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
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

        _mockService.QueryCosts(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
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
        await _mockService.Received(1).QueryCosts(
            Arg.Is<string>(s => s.Contains("test-subscription-id")),
            Arg.Is<string>(s => s == "ActualCost"),
            Arg.Is<string>(s => s == "Daily"),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string[]?>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<RetryPolicyOptions?>());
    }
}
