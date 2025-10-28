using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using StorageService.Application.Details.Dtos;
using StorageService.Application.Details.UseCases.CreateDetail;
using StorageService.Application.Exceptions;
using StorageService.Domain.Abstractions.Data;
using StorageService.Domain.Entities;

namespace StorageService.Tests.UnitTests.UseCases.Details;

public class CreateDetailHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IRequestHandler<CreateDetailCommand, DetailCoreDto> _handler;

    public CreateDetailHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var detailsRepositoryMock = new Mock<IDetailsRepository>();
        _unitOfWorkMock.Setup(u => u.DetailsRepository)
            .Returns(detailsRepositoryMock.Object);

        _handler = new CreateDetailHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateDetailAndReturnDto_WhenNomenclatureCodeIsUnique()
    {
        // Arrange
        var command = new CreateDetailCommand(
            NomenclatureCode: "ABC-123",
            Name: "Engine Part",
            Count: 50,
            StorekeeperId: 1,
            CreatedAtDate: DateTime.UtcNow);

        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync((Detail?)null);

        var expectedDto = new DetailCoreDto
        {
            Id = 1,
            NomenclatureCode = command.NomenclatureCode,
            Name = command.Name,
            Count = command.Count,
            StorekeeperId = command.StorekeeperId
        };

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.AddAsync(
                It.Is<Detail>(d =>
                    d.NomenclatureCode == command.NomenclatureCode &&
                    d.Name == command.Name &&
                    d.Count == command.Count &&
                    d.StorekeeperId == command.StorekeeperId &&
                    d.IsDeleted == false &&
                    d.CreatedAtDate == command.CreatedAtDate),
                cancellationToken))
            .Callback<Detail, CancellationToken>((entity, _) => entity.Id = 1)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<DetailCoreDto>(It.Is<Detail>(d => d.Id == 1)))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);

        _unitOfWorkMock.Verify(u => u.DetailsRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.DetailsRepository.AddAsync(
            It.IsAny<Detail>(), cancellationToken), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveAllAsync(cancellationToken), Times.Once);

        _mapperMock.Verify(m => m.Map<DetailCoreDto>(It.IsAny<Detail>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowAlreadyExistsException_WhenNomenclatureCodeAlreadyExists()
    {
        // Arrange
        var command = new CreateDetailCommand(
            NomenclatureCode: "ABC-123",
            Name: "Engine Part",
            Count: 50,
            StorekeeperId: 1,
            CreatedAtDate: DateTime.UtcNow);

        var cancellationToken = CancellationToken.None;

        var existingDetail = new Detail { NomenclatureCode = "ABC-123" };

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(
                    It.IsAny<Expression<Func<Detail, bool>>>(),
                    cancellationToken))
            .ReturnsAsync(existingDetail);

        // Act
        var act = async () => await _handler.Handle(command, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Detail with same nomenclature code already exists");

        _unitOfWorkMock.Verify(u => u.DetailsRepository.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetIsDeletedToFalse_WhenCreatingNewDetail()
    {
        // Arrange
        var command = new CreateDetailCommand(
            NomenclatureCode: "XYZ-789",
            Name: "Brake Pad",
            Count: 100,
            StorekeeperId: 2,
            CreatedAtDate:
            DateTime.UtcNow.AddDays(-1));

        var cancellationToken = CancellationToken.None;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository
                .FirstOrDefaultAsync(It.IsAny<Expression<Func<Detail, bool>>>(), cancellationToken))
            .ReturnsAsync((Detail?)null);

        Detail? capturedDetail = null;

        _unitOfWorkMock
            .Setup(u => u.DetailsRepository.AddAsync(
                It.IsAny<Detail>(), cancellationToken))
            .Callback<Detail, CancellationToken>((d, _) =>
            {
                d.Id = 5;
                capturedDetail = d;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveAllAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<DetailCoreDto>(It.IsAny<Detail>()))
            .Returns<Detail>(d => new DetailCoreDto
            {
                Id = d.Id,
                NomenclatureCode = d.NomenclatureCode,
                Name = d.Name,
                Count = d.Count,
                StorekeeperId = d.StorekeeperId
            });

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        capturedDetail.Should().NotBeNull();
        capturedDetail!.IsDeleted.Should().BeFalse();
    }
}