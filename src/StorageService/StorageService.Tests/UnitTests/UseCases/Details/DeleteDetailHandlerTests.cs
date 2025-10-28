using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Details.UseCases.DeleteDetail;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Details;

public class DeleteDetailHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IRequestHandler<DeleteDetailCommand> _handler;

    public DeleteDetailHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var detailsRepositoryMock = new Mock<IDetailsRepository>();
        _unitOfWorkMock.Setup(u => u.DetailsRepository).Returns(detailsRepositoryMock.Object);

        _handler = new DeleteDetailHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMarkDetailAsDeleted_WhenDetailExists()
    {
        // Arrange
        var detailId = 1;
        var command = new DeleteDetailCommand(Id: detailId);
        var cancellationToken = CancellationToken.None;

        var existingDetail = new Detail
        {
            Id = detailId,
            NomenclatureCode = "ABC-123",
            Name = "Engine Part",
            Count = 50,
            StorekeeperId = 1,
            IsDeleted = false
        };

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .GetByIdAsync(detailId, cancellationToken))
            .ReturnsAsync(existingDetail);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        existingDetail.IsDeleted.Should().BeTrue();

        _unitOfWorkMock.Verify(u => u.DetailsRepository
            .GetByIdAsync(detailId, cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(cancellationToken), Times.Once); }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDetailDoesNotExist()
    {
        // Arrange
        var detailId = 999;
        var command = new DeleteDetailCommand(Id: detailId);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .GetByIdAsync(detailId, cancellationToken))
            .ReturnsAsync((Detail?)null);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Detail is not found");

        _unitOfWorkMock.Verify(u => u.DetailsRepository
            .GetByIdAsync(detailId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotModifyOtherFields_WhenDeleting()
    {
        // Arrange
        var detailId = 2;
        var command = new DeleteDetailCommand(Id: detailId);
        var cancellationToken = CancellationToken.None;

        var originalDetail = new Detail
        {
            Id = detailId,
            NomenclatureCode = "XYZ-789",
            Name = "Brake Pad",
            Count = 100,
            StorekeeperId = 2,
            IsDeleted = false,
            CreatedAtDate = new DateTime(2025, 1, 1)
        };

        var detailCopy = new Detail
        {
            Id = originalDetail.Id,
            NomenclatureCode = originalDetail.NomenclatureCode,
            Name = originalDetail.Name,
            Count = originalDetail.Count,
            StorekeeperId = originalDetail.StorekeeperId,
            IsDeleted = originalDetail.IsDeleted,
            CreatedAtDate = originalDetail.CreatedAtDate
        };

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .GetByIdAsync(detailId, cancellationToken))
            .ReturnsAsync(detailCopy);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        detailCopy.Should().BeEquivalentTo(originalDetail, options =>
            options.Excluding(d => d.IsDeleted));

        detailCopy.IsDeleted.Should().BeTrue();
    }
}