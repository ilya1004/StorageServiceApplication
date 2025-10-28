using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.CreateStorekeeper;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Storekeepers;

public class CreateStorekeeperHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<CreateStorekeeperCommand, StorekeeperCoreDto> _handler;

    public CreateStorekeeperHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var storekeepersRepositoryMock = new Mock<IStorekeeperRepository>();
        _unitOfWorkMock.Setup(u => u.StorekeepersRepository).Returns(storekeepersRepositoryMock.Object);

        _handler = new CreateStorekeeperHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateStorekeeperAndReturnDto_WhenFullNameIsUnique()
    {
        // Arrange
        var command = new CreateStorekeeperCommand(FullName: "John Doe");

        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                    cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        var expectedDto = new StorekeeperCoreDto
        {
            Id = 1,
            FullName = command.FullName
        };

        Storekeeper? capturedStorekeeper = null;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.AddAsync(
                It.Is<Storekeeper>(s =>
                    s.FullName == command.FullName &&
                    s.IsDeleted == false),
                cancellationToken))
            .Callback<Storekeeper, CancellationToken>((entity, _) =>
            {
                capturedStorekeeper = entity;
                entity.Id = 1;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<StorekeeperCoreDto>(It.Is<Storekeeper>(s => s.Id == 1)))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);

        capturedStorekeeper.Should().NotBeNull();
        capturedStorekeeper!.FullName.Should().Be(command.FullName);
        capturedStorekeeper.IsDeleted.Should().BeFalse();

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.AddAsync(
            It.IsAny<Storekeeper>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(cancellationToken), Times.Once);

        _mapperMock.Verify(m => m.Map<StorekeeperCoreDto>(It.IsAny<Storekeeper>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExistsException_WhenStorekeeperWithSameFullNameExists()
    {
        // Arrange
        var command = new CreateStorekeeperCommand(FullName: "John Doe");

        var cancellationToken = CancellationToken.None;

        var existingStorekeeper = new Storekeeper
        {
            Id = 1,
            FullName = command.FullName,
            IsDeleted = false
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(existingStorekeeper);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Storekeeper with this Full name already exists");

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAllowCreate_WhenStorekeeperWithSameNameIsDeleted()
    {
        // Arrange
        var command = new CreateStorekeeperCommand(FullName: "Jane Smith");

        var cancellationToken = CancellationToken.None;

        var deletedStorekeeper = new Storekeeper
        {
            Id = 1,
            FullName = command.FullName,
            IsDeleted = true
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                    cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        var expectedDto = new StorekeeperCoreDto
        {
            Id = 2,
            FullName = command.FullName
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.AddAsync(
                It.IsAny<Storekeeper>(), cancellationToken))
            .Callback<Storekeeper, CancellationToken>((entity, _) => entity.Id = 2)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<StorekeeperCoreDto>(It.IsAny<Storekeeper>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }
}