using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Exceptions;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.GetStorekeeperById;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Storekeepers;

public class GetStorekeeperByIdHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<GetStorekeeperByIdQuery, StorekeeperCoreDto> _handler;

    public GetStorekeeperByIdHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var storekeepersRepositoryMock = new Mock<IStorekeeperRepository>();
        _unitOfWorkMock.Setup(u => u.StorekeepersRepository).Returns(storekeepersRepositoryMock.Object);

        _handler = new GetStorekeeperByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnStorekeeperDto_WhenStorekeeperExistsAndNotDeleted()
    {
        // Arrange
        var storekeeperId = 1;
        var query = new GetStorekeeperByIdQuery(Id: storekeeperId);
        var cancellationToken = CancellationToken.None;

        var storekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "John Doe",
            IsDeleted = false
        };

        var expectedDto = new StorekeeperCoreDto
        {
            Id = storekeeperId,
            FullName = "John Doe"
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(storekeeper);

        _mapperMock
            .Setup(m => m.Map<StorekeeperCoreDto>(storekeeper))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);

        _mapperMock.Verify(m => m.Map<StorekeeperCoreDto>(storekeeper), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenStorekeeperDoesNotExist()
    {
        // Arrange
        var storekeeperId = 999;
        var query = new GetStorekeeperByIdQuery(Id: storekeeperId);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock.Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        // Act
        var act = async () => await _handler.Handle(query, cancellationToken);

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
        var query = new GetStorekeeperByIdQuery(Id: storekeeperId);
        var cancellationToken = CancellationToken.None;

        var deletedStorekeeper = new Storekeeper
        {
            Id = storekeeperId,
            FullName = "Deleted User",
            IsDeleted = true
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                    cancellationToken))
            .ReturnsAsync((Storekeeper?)null);

        // Act
        var act = async () => await _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Storekeeper is not found");

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }
}