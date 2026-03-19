using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AutoMapper;
using DotnetCrud.Api.Controllers.Base;
using DotnetCrud.Core.Models;
using DotnetCrud.Core.Interfaces;
using DotnetCrud.Core.DTOs;

namespace DotnetCrud.UnitTests.Controllers
{
    public class BaseApiControllerTests
    {
        private readonly Mock<IRepository<TestEntity>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TestController>> _mockLogger;
        private readonly TestController _controller;

        public BaseApiControllerTests()
        {
            _mockRepository = new Mock<IRepository<TestEntity>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TestController>>();
            _controller = new TestController(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var entities = GenerateTestEntities(5);
            var dtos = GenerateTestResponseDtos(5);
            
            _mockRepository
                .Setup(r => r.GetPaginatedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<TestEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TestEntity>, IOrderedQueryable<TestEntity>>>(),
                    It.IsAny<string>()))
                .ReturnsAsync((entities, 5));
            
            _mockMapper
                .Setup(m => m.Map<List<TestResponseDto>>(It.IsAny<IEnumerable<TestEntity>>()))
                .Returns(dtos);

            // Act
            var result = await _controller.GetAll(1, 20);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PaginatedResponse<TestResponseDto>>(okResult.Value);
            Assert.Equal(5, response.TotalCount);
            Assert.Equal(5, response.Items.Count);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity { Id = id, Name = "Test" };
            var dto = new TestResponseDto { Id = id, Name = "Test" };

            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(entity);
            
            _mockMapper
                .Setup(m => m.Map<TestResponseDto>(entity))
                .Returns(dto);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TestResponseDto>(okResult.Value);
            Assert.Equal(id, returnedDto.Id);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((TestEntity?)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new TestCreateDto { Name = "New Entity" };
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "New Entity" };
            var responseDto = new TestResponseDto { Id = entity.Id, Name = "New Entity" };

            _mockMapper
                .Setup(m => m.Map<TestEntity>(createDto))
                .Returns(entity);
            
            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<TestEntity>()))
                .ReturnsAsync(entity);
            
            _mockMapper
                .Setup(m => m.Map<TestResponseDto>(entity))
                .Returns(responseDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<TestResponseDto>(createdResult.Value);
            Assert.Equal(entity.Id, returnedDto.Id);
            Assert.Equal("GetById", createdResult.ActionName);
        }

        [Fact]
        public async Task Update_ExistingEntity_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new TestUpdateDto { Name = "Updated" };
            var entity = new TestEntity { Id = id, Name = "Original" };

            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(entity);
            
            _mockRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TestEntity>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingEntity_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new TestUpdateDto { Name = "Updated" };

            _mockRepository
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((TestEntity?)null);

            // Act
            var result = await _controller.Update(id, updateDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ExistingEntity_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_NonExistingEntity_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.ExistsAsync(id))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Helper methods and test classes
        private List<TestEntity> GenerateTestEntities(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => new TestEntity 
                { 
                    Id = Guid.NewGuid(), 
                    Name = $"Entity {i}",
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();
        }

        private List<TestResponseDto> GenerateTestResponseDtos(int count)
        {
            return Enumerable.Range(1, count)
                .Select(i => new TestResponseDto 
                { 
                    Id = Guid.NewGuid(), 
                    Name = $"Entity {i}",
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();
        }
    }

    // Test implementations
    public class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestResponseDto : BaseResponseDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestController : BaseApiController<TestEntity, TestCreateDto, TestUpdateDto, TestResponseDto>
    {
        public TestController(
            IRepository<TestEntity> repository,
            IMapper mapper,
            ILogger<TestController> logger)
            : base(repository, mapper, logger)
        {
        }
    }
}