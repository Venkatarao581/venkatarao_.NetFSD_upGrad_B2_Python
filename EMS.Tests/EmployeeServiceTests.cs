using System;
using System.Threading.Tasks;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace EMS.Tests.Services
{
    [TestFixture]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _repoMock = null!;
        private AppDbContext _db = null!;
        private EmployeeService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IEmployeeRepository>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);

            _service = new EmployeeService(_repoMock.Object, _db);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        // ── GetByIdAsync ──────────────────────────────────────────────────────

        [Test]
        public async Task GetByIdAsync_ValidId_ReturnsMappedDto()
        {
            var fakeEmp = new Employee
            {
                Id = 1, FirstName = "Priya", LastName = "Prabhu",
                Email = "priya@test.com", Phone = "9876543210",
                Department = "Engineering", Designation = "Engineer",
                Salary = 850000, JoinDate = DateTime.UtcNow,
                Status = "Active", CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEmp);

            var result = await _service.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FirstName, Is.EqualTo("Priya"));
            Assert.That(result.Email, Is.EqualTo("priya@test.com"));
            _repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(9999)).ReturnsAsync((Employee?)null);
            var result = await _service.GetByIdAsync(9999);
            Assert.That(result, Is.Null);
        }

        // ── AddAsync ──────────────────────────────────────────────────────────

        [Test]
        public async Task AddAsync_UniqueEmail_CreatesEmployee()
        {
            var dto = new EmployeeRequestDto
            {
                FirstName = "Arjun", LastName = "Sharma",
                Email = "arjun@test.com", Phone = "9123456780",
                Department = "Marketing", Designation = "Executive",
                Salary = 600000, JoinDate = DateTime.UtcNow, Status = "Active"
            };

            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null))
                     .ReturnsAsync(false);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                     .ReturnsAsync((Employee e) => { e.Id = 99; return e; });

            var result = await _service.AddAsync(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(99));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Test]
        public async Task AddAsync_DuplicateEmail_ReturnsNull()
        {
            var dto = new EmployeeRequestDto
            {
                FirstName = "Neha", LastName = "Kapoor",
                Email = "existing@test.com", Phone = "9988776655",
                Department = "HR", Designation = "Executive",
                Salary = 500000, JoinDate = DateTime.UtcNow, Status = "Active"
            };
            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, null))
                     .ReturnsAsync(true);

            var result = await _service.AddAsync(dto);

            Assert.That(result, Is.Null);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────

        [Test]
        public async Task UpdateAsync_ValidId_ReturnsUpdatedDto()
        {
            var existingEmp = new Employee
            {
                Id = 5, FirstName = "Old", LastName = "Name",
                Email = "old@test.com", Phone = "9000000000",
                Department = "HR", Designation = "Manager",
                Salary = 700000, JoinDate = DateTime.UtcNow,
                Status = "Active", CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var dto = new EmployeeRequestDto
            {
                FirstName = "New", LastName = "Name",
                Email = "new@test.com", Phone = "9111111111",
                Department = "Finance", Designation = "Analyst",
                Salary = 800000, JoinDate = DateTime.UtcNow, Status = "Active"
            };

            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existingEmp);
            _repoMock.Setup(r => r.EmailExistsAsync(dto.Email, 5)).ReturnsAsync(false);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                     .ReturnsAsync((Employee e) => e);

            var (result, conflict) = await _service.UpdateAsync(5, dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(conflict, Is.False);
            Assert.That(result!.FirstName, Is.EqualTo("New"));
        }

        [Test]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);
            var (result, conflict) = await _service.UpdateAsync(999, new EmployeeRequestDto());
            Assert.That(result, Is.Null);
            Assert.That(conflict, Is.False);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────

        [Test]
        public async Task DeleteAsync_ValidId_ReturnsTrue()
        {
            _repoMock.Setup(r => r.RemoveAsync(1)).ReturnsAsync(true);
            var result = await _service.DeleteAsync(1);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_InvalidId_ReturnsFalse()
        {
            _repoMock.Setup(r => r.RemoveAsync(9999)).ReturnsAsync(false);
            var result = await _service.DeleteAsync(9999);
            Assert.That(result, Is.False);
        }
    }
}