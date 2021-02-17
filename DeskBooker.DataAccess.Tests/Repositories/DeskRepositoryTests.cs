using DeskBooker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeskBooker.DataAccess.Repositories {
    public class DeskRepositoryTests {
        [Fact]
        public void ShouldReturnTheAvailableDesks() {
            // Arrange
            var Date = new DateTime(2020, 1, 25);

            var Options = new DbContextOptionsBuilder<DeskBookerContext>()
              .UseInMemoryDatabase(databaseName: "ShouldReturnTheAvailableDesks")
              .Options;

            using (var Context = new DeskBookerContext(Options)) {
                Context.Desk.Add(new Desk { Id = 1 });
                Context.Desk.Add(new Desk { Id = 2 });
                Context.Desk.Add(new Desk { Id = 3 });

                Context.DeskBooking.Add(new DeskBooking { DeskId = 1, Date = Date });
                Context.DeskBooking.Add(new DeskBooking { DeskId = 2, Date = Date.AddDays(1) });

                Context.SaveChanges();
            }

            using (var context = new DeskBookerContext(Options)) {
                var Repository = new DeskRepository(context);

                // Act
                var Desks = Repository.GetAvailableDesks(Date);

                // Assert
                Assert.Equal(2, Desks.Count());
                Assert.Contains(Desks, d => d.Id == 2);
                Assert.Contains(Desks, d => d.Id == 3);
                Assert.DoesNotContain(Desks, d => d.Id == 1);
            }
        }

        [Fact]
        public void ShouldGetAll() {
            // Arrange
            var Options = new DbContextOptionsBuilder<DeskBookerContext>()
              .UseInMemoryDatabase(databaseName: "ShouldGetAll")
              .Options;

            var StoredList = new List<Desk>
            {
                new Desk(),
                new Desk(),
                new Desk()
            };

            using (var Context = new DeskBookerContext(Options)) {
                foreach (var desk in StoredList) {
                    Context.Add(desk);
                    Context.SaveChanges();
                }
            }

            // Act
            List<Desk> actualList;
            using (var context = new DeskBookerContext(Options)) {
                var repository = new DeskRepository(context);
                actualList = repository.GetAll().ToList();
            }

            // Assert
            Assert.Equal(StoredList.Count(), actualList.Count());
        }
    }
}