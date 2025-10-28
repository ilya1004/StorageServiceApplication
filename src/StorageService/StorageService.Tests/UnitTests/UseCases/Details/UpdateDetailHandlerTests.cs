using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.UpdateDetail;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Details;

public class UpdateDetailHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<UpdateDetailCommand, DetailCoreDto> _handler;

    public UpdateDetailHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var detailsRepositoryMock = new Mock<IDetailsRepository>();
        _unitOfWorkMock.Setup(u => u.DetailsRepository).Returns(detailsRepositoryMock.Object);

        _handler = new UpdateDetailHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDetailAndReturnDto_WhenDetailExistsAndCodeIsUnique()
    {
        // Arrange
        var detailId = 1;
        var command = new UpdateDetailCommand(
            Id: detailId,
            NomenclatureCode: "NEW-456",
            Name: "Updated Engine Part",
            Count: 75,
            StorekeeperId: 2,
            CreatedAtDate: DateTime.UtcNow.AddDays(1));

        var cancellationToken = CancellationToken.None;

        var existingDetail = new Detail
        {
            Id = detailId,
            NomenclatureCode = "OLD-123",
            Name = "Old Engine Part",
            Count = 50,
            StorekeeperId = 1,
            IsDeleted = false,
            CreatedAtDate = DateTime.UtcNow
        };

        _unitOfWorkMock.Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(existingDetail);

        _unitOfWorkMock.Setup(u => u.DetailsRepository
                .AnyAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        var expectedDto = new DetailCoreDto
        {
            Id = detailId,
            NomenclatureCode = command.NomenclatureCode,
            Name = command.Name,
            Count = command.Count,
            StorekeeperId = command.StorekeeperId
        };

        _mapperMock.Setup(m => m.Map<DetailCoreDto>(existingDetail))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);

        existingDetail.NomenclatureCode.Should().Be(command.NomenclatureCode);
        existingDetail.Name.Should().Be(command.Name);
        existingDetail.Count.Should().Be(command.Count);
        existingDetail.StorekeeperId.Should().Be(command.StorekeeperId);
        existingDetail.CreatedAtDate.Should().Be(command.CreatedAtDate);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDetailIsDeleted()
    {
        // Arrange
        var detailId = 1;
        var command = new UpdateDetailCommand(
            Id: detailId,
            NomenclatureCode: "NEW-456",
            Name: "Updated Engine Part",
            Count: 75,
            StorekeeperId: 2,
            CreatedAtDate: DateTime.UtcNow.AddDays(1));
        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync((Detail?)null);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Detail not found");

        _unitOfWorkMock.Verify(u => u.DetailsRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExistsException_WhenNomenclatureCodeIsDuplicate()
    {
        // Arrange
        var detailId = 1;
        var command = new UpdateDetailCommand(
            Id: detailId,
            NomenclatureCode: "DUPLICATE-789",
            Name: "Updated Engine Part",
            Count: 75,
            StorekeeperId: 2,
            CreatedAtDate: DateTime.UtcNow.AddDays(1));

        var cancellationToken = CancellationToken.None;

        var existingDetail = new Detail { Id = detailId, IsDeleted = false };

        _unitOfWorkMock.Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(existingDetail);

        _unitOfWorkMock.Setup(u => u.DetailsRepository
                .AnyAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Detail with same nomenclature code already exists");
    }

    [Fact]
    public async Task Handle_ShouldAllowSameCode_WhenUpdatingToOwnCurrentCode()
    {
        // Arrange
        var detailId = 1;
        var command = new UpdateDetailCommand(
            Id: detailId,
            NomenclatureCode: "ABC-123",
            Name: "Updated Engine Part",
            Count: 75,
            StorekeeperId: 2,
            CreatedAtDate: DateTime.UtcNow.AddDays(1));

        var cancellationToken = CancellationToken.None;

        var existingDetail = new Detail
        {
            Id = detailId,
            NomenclatureCode = command.NomenclatureCode,
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
            .ReturnsAsync(existingDetail);

        _unitOfWorkMock.Setup(u => u.DetailsRepository
                .AnyAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        _mapperMock.Setup(m => m.Map<DetailCoreDto>(existingDetail))
            .Returns(new DetailCoreDto());

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(cancellationToken), Times.Once);
    }
}