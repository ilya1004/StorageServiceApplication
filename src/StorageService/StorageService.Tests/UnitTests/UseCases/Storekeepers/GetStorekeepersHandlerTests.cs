using System.Linq.Expressions;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Storekeepers.Dtos;
using StorageService.Application.Storekeepers.UseCases.GetStorekeepers;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;
using StorageService.Domain.Models;

namespace StorageService.Tests.UnitTests.UseCases.Storekeepers;

public class GetStorekeepersHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IRequestHandler<GetStorekeepersQuery, PaginatedResultModel<StorekeeperWithDetailsCountDto>> _handler;

    public GetStorekeepersHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var storekeepersRepositoryMock = new Mock<IStorekeeperRepository>();
        _unitOfWorkMock.Setup(u => u.StorekeepersRepository).Returns(storekeepersRepositoryMock.Object);

        _handler = new GetStorekeepersHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedStorekeepersWithDetailsCount_WhenDataExists()
    {
        // Arrange
        var query = new GetStorekeepersQuery(PageNo: 1, PageSize: 2);
        var cancellationToken = CancellationToken.None;
        var offset = (query.PageNo - 1) * query.PageSize;

        var totalCount = 3;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListWithProjectionAsync(
                offset,
                query.PageSize,
                It.IsAny<Expression<Func<Storekeeper, StorekeeperWithDetailsCountDto>>>(),
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<Storekeeper, object>>[]>()))
            .ReturnsAsync(new List<StorekeeperWithDetailsCountDto>
            {
                new StorekeeperWithDetailsCountDto { Id = 1, FullName = "John Doe", DetailsCount = 30 },
                new StorekeeperWithDetailsCountDto { Id = 2, FullName = "Jane Smith", DetailsCount = 5 }
            });

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.CountAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.PageNo.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(totalCount);
        result.Items.Should().HaveCount(2);

        result.Items[0].Should().BeEquivalentTo(new StorekeeperWithDetailsCountDto
        {
            Id = 1,
            FullName = "John Doe",
            DetailsCount = 30
        });

        result.Items[1].Should().BeEquivalentTo(new StorekeeperWithDetailsCountDto
        {
            Id = 2,
            FullName = "Jane Smith",
            DetailsCount = 5
        });

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.ListWithProjectionAsync(
            offset, query.PageSize,
            It.IsAny<Expression<Func<Storekeeper, StorekeeperWithDetailsCountDto>>>(),
            It.IsAny<Expression<Func<Storekeeper, bool>>>(),
            cancellationToken,
            It.IsAny<Expression<Func<Storekeeper, object>>[]>()),
            Times.Once);

        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.CountAsync(
            It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoStorekeepers()
    {
        // Arrange
        var query = new GetStorekeepersQuery(PageNo: 1, PageSize: 10);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListWithProjectionAsync(
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Storekeeper, StorekeeperWithDetailsCountDto>>>(),
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<Storekeeper, object>>[]>()))
            .ReturnsAsync(new List<StorekeeperWithDetailsCountDto>());

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.CountAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNo.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_ShouldCalculateOffsetCorrectly_ForPage3()
    {
        // Arrange
        var query = new GetStorekeepersQuery(PageNo: 3, PageSize: 5);
        var cancellationToken = CancellationToken.None;
        var expectedOffset = (3 - 1) * 5; // 10

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListWithProjectionAsync(
                expectedOffset,
                query.PageSize,
                It.IsAny<Expression<Func<Storekeeper, StorekeeperWithDetailsCountDto>>>(),
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<Storekeeper, object>>[]>()))
            .ReturnsAsync(new List<StorekeeperWithDetailsCountDto>());

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.CountAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(20);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(u => u.StorekeepersRepository.ListWithProjectionAsync(
            expectedOffset, query.PageSize,
            It.IsAny<Expression<Func<Storekeeper, StorekeeperWithDetailsCountDto>>>(),
            It.IsAny<Expression<Func<Storekeeper, bool>>>(),
            cancellationToken,
            It.IsAny<Expression<Func<Storekeeper, object>>[]>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCorrectlySumDetailsCount_WhenMultipleDetails()
    {
        // Arrange
        var query = new GetStorekeepersQuery(PageNo: 1, PageSize: 1);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.ListWithProjectionAsync(
                It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Expression<Func<Storekeeper, StorekeeperWithDetailsCountDto>>>(),
                It.IsAny<Expression<Func<Storekeeper, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<Storekeeper, object>>[]>()))
            .ReturnsAsync(new List<StorekeeperWithDetailsCountDto>
            {
                new StorekeeperWithDetailsCountDto { Id = 1, FullName = "Test User", DetailsCount = 50 }
            });

        _unitOfWorkMock
            .Setup(u => u.StorekeepersRepository.CountAsync(
                It.IsAny<Expression<Func<Storekeeper, bool>>>(), cancellationToken))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].DetailsCount.Should().Be(50);
    }
}