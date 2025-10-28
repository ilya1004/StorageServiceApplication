using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.GetDetailById;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Details;

public class GetDetailByIdHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<GetDetailByIdQuery, DetailCoreDto> _handler;

    public GetDetailByIdHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var detailsRepositoryMock = new Mock<IDetailsRepository>();
        _unitOfWorkMock.Setup(u => u.DetailsRepository).Returns(detailsRepositoryMock.Object);

        _handler = new GetDetailByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDetailDto_WhenDetailExistsAndNotDeleted()
    {
        // Arrange
        var detailId = 1;
        var query = new GetDetailByIdQuery(Id: detailId);
        var cancellationToken = CancellationToken.None;

        var detail = new Detail
        {
            Id = detailId,
            NomenclatureCode = "ABC-123",
            Name = "Engine Part",
            Count = 50,
            StorekeeperId = 1,
            IsDeleted = false,
            CreatedAtDate = DateTime.UtcNow
        };

        var expectedDto = new DetailCoreDto
        {
            Id = detailId,
            NomenclatureCode = "ABC-123",
            Name = "Engine Part",
            Count = 50,
            StorekeeperId = 1
        };

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(detail);

        _mapperMock
            .Setup(m => m.Map<DetailCoreDto>(detail))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);

        _unitOfWorkMock.Verify(u => u.DetailsRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);

        _mapperMock.Verify(m => m.Map<DetailCoreDto>(detail), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDetailDoesNotExist()
    {
        // Arrange
        var detailId = 999;
        var query = new GetDetailByIdQuery(Id: detailId);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
            .ReturnsAsync((Detail?)null);

        // Act
        var act = async () => await _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Detail is not found");

        _unitOfWorkMock.Verify(u => u.DetailsRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDetailIsDeleted()
    {
        // Arrange
        var detailId = 1;
        var query = new GetDetailByIdQuery(Id: detailId);
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync((Detail?)null);

        // Act
        var act = async () => await _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Detail is not found");
    }
}