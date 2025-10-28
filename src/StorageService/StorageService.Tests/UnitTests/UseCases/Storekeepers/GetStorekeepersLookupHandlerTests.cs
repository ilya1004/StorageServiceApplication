using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.GetStorekeepersLookup;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Storekeepers;

public class GetStorekeepersLookupHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IRequestHandler<GetStorekeepersLookupQuery, List<StorekeeperCoreDto>> _handler;

    public GetStorekeepersLookupHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var storekeepersRepositoryMock = new Mock<IStorekeeperRepository>();
        _unitOfWorkMock.Setup(u => u.StorekeepersRepository).Returns(storekeepersRepositoryMock.Object);

        _handler = new GetStorekeepersLookupHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnActiveStorekeepersAsCoreDto_WhenDataExists()
    {
        // Arrange
        var query = new GetStorekeepersLookupQuery();
        var cancellationToken = CancellationToken.None;

        var activeStorekeepers = new List<Storekeeper>
        {
            new Storekeeper { Id = 1, FullName = "John Doe", IsDeleted = false },
            new Storekeeper { Id = 2, FullName = "Jane Smith", IsDeleted = false }
        };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken))
            .ReturnsAsync(activeStorekeepers);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(new List<StorekeeperCoreDto>
        {
            new StorekeeperCoreDto { Id = 1, FullName = "John Doe" },
            new StorekeeperCoreDto { Id = 2, FullName = "Jane Smith" }
        });

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.ListAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoActiveStorekeepers()
    {
        // Arrange
        var query = new GetStorekeepersLookupQuery();
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken))
            .ReturnsAsync(new List<Storekeeper>());

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEmpty();

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.ListAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilterOutDeletedStorekeepers()
    {
        // Arrange
        var query = new GetStorekeepersLookupQuery();
        var cancellationToken = CancellationToken.None;

        var storekeeper1 = new Storekeeper { Id = 1, FullName = "Active", IsDeleted = false };
        var storekeeper2 = new Storekeeper { Id = 2, FullName = "Deleted", IsDeleted = true };

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken))
            .ReturnsAsync(new List<Storekeeper> { storekeeper1 });

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().ContainSingle();
        result[0].Id.Should().Be(1);
        result[0].FullName.Should().Be("Active");
    }
}