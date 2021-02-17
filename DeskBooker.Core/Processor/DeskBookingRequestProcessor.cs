using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System;
using System.Linq;

namespace DeskBooker.Core.Processor {
    public class DeskBookingRequestProcessor {
        public DeskBookingRequestProcessor(IDeskBookingRepository DeskBookingRepository, IDeskRepository DeskRepository) {
            this.DeskBookingRepository = DeskBookingRepository;
            this.DeskRepository = DeskRepository;
        }

        private readonly IDeskBookingRepository DeskBookingRepository;
        private readonly IDeskRepository DeskRepository;

        public DeskBookingResult BookDesk(DeskBookingRequest Request) {
            if (Request is null) {
                throw new ArgumentNullException(nameof(Request));
            }

            var Result = Create<DeskBookingResult>(Request);

            var AvailableDesks = DeskRepository.GetAvailableDesks(Request.Date);

            if (AvailableDesks.FirstOrDefault() is Desk AvailableDesk) {
                var DeskBooking = Create<DeskBooking>(Request);
                DeskBooking.DeskId = AvailableDesk.Id;

                DeskBookingRepository.Save(DeskBooking);

                Result.DeskBookingId = DeskBooking.Id;
                Result.Code = DeskBookingResultCode.Success;
            }
            else {
                Result.Code = DeskBookingResultCode.NoDeskAvailable;
            }

            return Result;
        }

        private static T Create<T>(DeskBookingRequest Request) where T : DeskBookingBase, new() {
            return new T {
                FirstName = Request.FirstName,
                LastName = Request.LastName,
                Email = Request.Email,
                Date = Request.Date
            };
        }
    }
}