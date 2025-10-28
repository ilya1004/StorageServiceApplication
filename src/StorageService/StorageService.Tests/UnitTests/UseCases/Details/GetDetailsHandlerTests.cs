using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.GetDetails;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;
using StorageService.Domain.Models;

namespace StorageService.Tests.UnitTests.UseCases.Details;

public class GetDetailsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<GetDetailsQuery, PaginatedResultModel<DetailCoreDto>> _handler;

    public GetDetailsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var detailsRepositoryMock = new Mock<IDetailsRepository>();
        _unitOfWorkMock.Setup(u => u.DetailsRepository).Returns(detailsRepositoryMock.Object);

        _handler = new GetDetailsHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedDetailsWithStorekeeper_WhenDataExists()
    {
        // Arrange
        var query = new GetDetailsQuery(PageNo: 1, PageSize: 2);
        var cancellationToken = CancellationToken.None;

        var details = new List<Detail>
        {
            new Detail
            {
                Id = 1,
                NomenclatureCode = "ABC-123",
                Name = "Engine Part",
                Count = 50,
                StorekeeperId = 1,
                IsDeleted = false,
                Storekeeper = new Storekeeper { Id = 1, FullName = "John Doe" }
            },
            new Detail
            {
                Id = 2,
                NomenclatureCode = "XYZ-789",
                Name = "Brake Pad",
                Count = 100,
                StorekeeperId = 2,
                IsDeleted = false,
                Storekeeper = new Storekeeper { Id = 2, FullName = "Jane Smith" }
            }
        };

        var totalCount = 5;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.PaginatedListAsync(
                0,
                query.PageSize,
                It.IsAny<Expression<Func<Detail, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<Detail, object>>[]>()))
            .ReturnsAsync(details);

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.CountAsync(
                It.IsAny<Expression<Func<Detail, bool>>>(),
                cancellationToken))
            .ReturnsAsync(totalCount);

        var dto1 = new DetailCoreDto { Id = 1, NomenclatureCode = "ABC-123", StorekeeperId = 1 };
        var dto2 = new DetailCoreDto { Id = 2, NomenclatureCode = "XYZ-789", StorekeeperId = 2 };

        _mapperMock.Setup(m => m.Map<DetailCoreDto>(details[0])).Returns(dto1);
        _mapperMock.Setup(m => m.Map<DetailCoreDto>(details[1])).Returns(dto2);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.PageNo.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(totalCount);
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(dto1);
        result.Items.Should().Contain(dto2);

        _unitOfWorkMock.Verify(u => u.DetailsRepository.PaginatedListAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Detail, bool>>>(),
            cancellationToken, It.IsAny<Expression<Func<Detail, object>>[]>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.DetailsRepository.CountAsync(
            It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);

        _mapperMock.Verify(m => m.Map<DetailCoreDto>(It.IsAny<Detail>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoActiveDetails()
    {
        // Arrange
        var query = new GetDetailsQuery(PageNo: 1, PageSize: 10);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.PaginatedListAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Detail, bool>>>(),
                cancellationToken, It.IsAny<Expression<Func<Detail, object>>[]>()))
            .ReturnsAsync(new List<Detail>());

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.CountAsync(
                It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
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
    public async Task Handle_ShouldCalculateOffsetCorrectly_ForPage2()
    {
        // Arrange
        var query = new GetDetailsQuery(PageNo: 2, PageSize: 3);
        var cancellationToken = CancellationToken.None;
        var expectedOffset = (2 - 1) * 3;

        var details = new List<Detail>
        {
            new Detail { Id = 4, IsDeleted = false }
        };

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.PaginatedListAsync(
                expectedOffset,
                query.PageSize,
                It.IsAny<Expression<Func<Detail, bool>>>(),
                cancellationToken,
                It.IsAny<Expression<Func<Detail, object>>[]>()))
            .ReturnsAsync(details);

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.CountAsync(
                It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
            .ReturnsAsync(10);

        _mapperMock.Setup(m => m.Map<DetailCoreDto>(It.IsAny<Detail>()))
            .Returns<Detail>(d => new DetailCoreDto { Id = d.Id });

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(u => u.DetailsRepository.PaginatedListAsync(
            expectedOffset, query.PageSize, It.IsAny<Expression<Func<Detail, bool>>>(),
            cancellationToken, It.IsAny<Expression<Func<Detail, object>>[]>()), Times.Once);
    }
}