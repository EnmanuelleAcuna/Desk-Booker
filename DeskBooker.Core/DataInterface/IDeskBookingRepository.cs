using DeskBooker.Core.Domain;
using System.Collections.Generic;

namespace DeskBooker.Core.DataInterface {
    public interface IDeskBookingRepository {
        void Save(DeskBooking DeskBooking);
        IEnumerable<DeskBooking> GetAll();
    }
}
