using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.UseCases.DeleteStorekeeper;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Storekeepers;

public class DeleteStorekeeperHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IRequestHandler<DeleteStorekeeperCommand> _handler;

    public DeleteStorekeeperHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var storekeepersRepositoryMock = new Mock<IStorekeeperRepository>();
        var detailsRepositoryMock = new Mock<IDetailsRepository>();

        _unitOfWorkMock.Setup(u => u.StorekeepersRepository).Returns(storekeepersRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.DetailsRepository).Returns(detailsRepositoryMock.Object);

        _handler = new DeleteStorekeeperHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMarkStorekeeperAsDeleted_WhenNoRelatedDetails()
    {
        // Arrange
        var storekeeperId = 1;
        var command = new DeleteStorekeeperCommand(Id: storekeeperId);
        var cancellationToken = CancellationToken.None;

        var storekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "John Doe",
            IsDeleted = false
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .GetByIdAsync(storekeeperId, cancellationToken))
            .ReturnsAsync(storekeeper);

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .AnyAsync(It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        storekeeper.IsDeleted.Should().BeTrue();

        _unitOfWorkMock
            .Verify(u => u.StorekeepersRepository
            .GetByIdAsync(storekeeperId, cancellationToken), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.DetailsRepository
            .AnyAsync(It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenStorekeeperDoesNotExist()
    {
        // Arrange
        var storekeeperId = 999;
        var command = new DeleteStorekeeperCommand(Id: storekeeperId);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .GetByIdAsync(storekeeperId, cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Storekeeper is not found");

        _unitOfWorkMock.Verify(u => u.DetailsRepository
            .AnyAsync(It.IsAny<Expression<Func<Detail, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenStorekeeperHasRelatedDetails()
    {
        // Arrange
        var storekeeperId = 1;
        var command = new DeleteStorekeeperCommand(Id: storekeeperId);
        var cancellationToken = CancellationToken.None;

        var storekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "Jane Smith",
            IsDeleted = false
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .GetByIdAsync(storekeeperId, cancellationToken))
            .ReturnsAsync(storekeeper);

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .AnyAsync(It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("You cannot delete storekeeper as he has details");

        storekeeper.IsDeleted.Should().BeFalse();

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}