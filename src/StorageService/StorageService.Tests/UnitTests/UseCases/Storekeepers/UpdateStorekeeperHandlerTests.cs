using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.UpdateStorekeeper;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Storekeepers;

public class UpdateStorekeeperHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<UpdateStorekeeperCommand, StorekeeperCoreDto> _handler;

    public UpdateStorekeeperHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var storekeepersRepositoryMock = new Mock<IStorekeeperRepository>();
        _unitOfWorkMock.Setup(u => u.StorekeepersRepository).Returns(storekeepersRepositoryMock.Object);

        _handler = new UpdateStorekeeperHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStorekeeperAndReturnDto_WhenStorekeeperExistsAndNameIsUnique()
    {
        // Arrange
        var storekeeperId = 1;
        var command = new UpdateStorekeeperCommand(
            Id: storekeeperId, FullName: "John Doe Updated");

        var cancellationToken = CancellationToken.None;

        var existingStorekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "John Doe",
            IsDeleted = false
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(existingStorekeeper);

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .AnyAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var expectedDto = new StorekeeperCoreDto
        {
            Id = storekeeperId,
            FullName = command.FullName
        };

        _mapperMock
            .Setup(m => m.Map<StorekeeperCoreDto>(existingStorekeeper))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        existingStorekeeper.FullName.Should().Be(command.FullName);

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.AnyAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(cancellationToken), Times.Once);

        _mapperMock.Verify(m => m.Map<StorekeeperCoreDto>(existingStorekeeper), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenStorekeeperDoesNotExist()
    {
        // Arrange
        var storekeeperId = 999;
        var command = new UpdateStorekeeperCommand(
            Id: storekeeperId, FullName: "Non-existent");

        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Storekeeper is not found");

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenStorekeeperIsDeleted()
    {
        // Arrange
        var storekeeperId = 1;
        var command = new UpdateStorekeeperCommand(
            Id: storekeeperId, FullName: "Deleted User");

        var cancellationToken = CancellationToken.None;

        var deletedStorekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "Deleted User",
            IsDeleted = true
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Storekeeper is not found");

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExistsException_WhenFullNameIsDuplicate()
    {
        // Arrange
        var storekeeperId = 1;
        var command = new UpdateStorekeeperCommand(
            Id: storekeeperId, FullName: "DUPLICATE NAME");

        var cancellationToken = CancellationToken.None;

        var existingStorekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "Original",
            IsDeleted = false
        };

        var duplicateStorekeeper = new Storekeeper
        {
            Id = 2,
            FullName = command.FullName,
            IsDeleted = false
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(existingStorekeeper);

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .AnyAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Storekeeper with this Full name already exists");

        existingStorekeeper.FullName.Should().NotBe(command.FullName);
    }
}