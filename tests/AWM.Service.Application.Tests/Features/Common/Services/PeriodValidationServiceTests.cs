namespace AWM.Service.Application.Tests.Features.Common.Services;

using AWM.Service.Application.Features.Common.Services;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class PeriodValidationServiceTests
{
    private readonly IPeriodRepository _periodRepository;
    private readonly PeriodValidationService _sut;

    public PeriodValidationServiceTests()
    {
        _periodRepository = Substitute.For<IPeriodRepository>();
        _sut = new PeriodValidationService(_periodRepository);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPeriodRepositoryIsNull()
    {
        // Act
        Action act = () => new PeriodValidationService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("periodRepository");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsStageOpenAsync_ShouldReturnRepositoryResult(bool expectedResult)
    {
        // Arrange
        var departmentId = 1;
        var academicYearId = 2023;
        var stage = WorkflowStage.TopicCreation;

        _periodRepository.IsStageOpenAsync(departmentId, academicYearId, stage, Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _sut.IsStageOpenAsync(departmentId, academicYearId, stage);

        // Assert
        result.Should().Be(expectedResult);
        await _periodRepository.Received(1).IsStageOpenAsync(departmentId, academicYearId, stage, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidateOperationInPeriodAsync_ShouldReturnSuccess_WhenStageIsOpen()
    {
        // Arrange
        var departmentId = 1;
        var academicYearId = 2023;
        var stage = WorkflowStage.TopicCreation;

        _periodRepository.IsStageOpenAsync(departmentId, academicYearId, stage, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var (isAllowed, errorMessage) = await _sut.ValidateOperationInPeriodAsync(departmentId, academicYearId, stage);

        // Assert
        isAllowed.Should().BeTrue();
        errorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ValidateOperationInPeriodAsync_ShouldReturnFailure_WhenStageIsClosed()
    {
        // Arrange
        var departmentId = 1;
        var academicYearId = 2023;
        var stage = WorkflowStage.TopicCreation;

        _periodRepository.IsStageOpenAsync(departmentId, academicYearId, stage, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var (isAllowed, errorMessage) = await _sut.ValidateOperationInPeriodAsync(departmentId, academicYearId, stage);

        // Assert
        isAllowed.Should().BeFalse();
        errorMessage.Should().Be($"The {stage} period is not currently open for this department.");
    }
}
