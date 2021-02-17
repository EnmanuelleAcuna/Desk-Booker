using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeskBooker.Core.Processor {
    public class DeskBookingRequestProcessorTest {
        // Setup
        public DeskBookingRequestProcessorTest() {
            Request = new DeskBookingRequest {
                FirstName = "Enmanuelle",
                LastName = "Acuna",
                Email = "emanuelacu@outlook.com",
                Date = new DateTime(2020, 1, 28)
            };

            AvailableDesks = new List<Desk> { new Desk { Id = 7 } };

            DeskBookingRepositoryMock = new Mock<IDeskBookingRepository>();

            DeskRepositoryMock = new Mock<IDeskRepository>();
            DeskRepositoryMock.Setup(x => x.GetAvailableDesks(Request.Date)).Returns(AvailableDesks);

            Processor = new DeskBookingRequestProcessor(DeskBookingRepositoryMock.Object, DeskRepositoryMock.Object);
        }

        private readonly DeskBookingRequest Request;
        private readonly List<Desk> AvailableDesks;
        private readonly Mock<IDeskBookingRepository> DeskBookingRepositoryMock;
        private readonly Mock<IDeskRepository> DeskRepositoryMock;
        private readonly DeskBookingRequestProcessor Processor;

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues() {
            // Act
            DeskBookingResult Result = Processor.BookDesk(Request);

            // Assert
            Assert.NotNull(Result);
            Assert.Equal(Request.FirstName, Result.FirstName);
            Assert.Equal(Request.LastName, Result.LastName);
            Assert.Equal(Request.Email, Result.Email);
            Assert.Equal(Request.Date, Result.Date);
        }

        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull() {
            var Exception = Assert.Throws<ArgumentNullException>(() => Processor.BookDesk(null));

            Assert.Equal("Request", Exception.ParamName);
        }

        [Fact]
        public void ShouldSaveDeskBooking() {
            DeskBooking SavedDeskBooking = null;

            // Simular guardado de objeto en bd
            DeskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking => { SavedDeskBooking = deskBooking; });

            // Act
            Processor.BookDesk(Request);

            DeskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            Assert.NotNull(SavedDeskBooking);
            Assert.Equal(Request.FirstName, SavedDeskBooking.FirstName);
            Assert.Equal(Request.LastName, SavedDeskBooking.LastName);
            Assert.Equal(Request.Email, SavedDeskBooking.Email);
            Assert.Equal(Request.Date, SavedDeskBooking.Date);
            Assert.Equal(AvailableDesks.First().Id, SavedDeskBooking.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable() {
            AvailableDesks.Clear();

            Processor.BookDesk(Request);

            DeskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(DeskBookingResultCode.Success, true)]
        [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
        public void ShouldReturnExpectedResultCode(DeskBookingResultCode ExpectedResultCode, bool IsDeskAvailable) {
            if (!IsDeskAvailable) {
                AvailableDesks.Clear();
            }

            var Result = Processor.BookDesk(Request);

            Assert.Equal(ExpectedResultCode, Result.Code);
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(null, false)]
        public void ShouldReturnExpectedDeskBookingId(int? ExpectedDeskBookingId, bool IsDeskAvailable) {
            if (!IsDeskAvailable) {
                AvailableDesks.Clear();
            }
            else {
                // Simular guardado de objeto en bd y asignacion de id "autogenerado"
                DeskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking => { deskBooking.Id = ExpectedDeskBookingId.Value; });
            }

            var Result = Processor.BookDesk(Request);

            Assert.Equal(ExpectedDeskBookingId, Result.DeskBookingId);
        }
    }
}
