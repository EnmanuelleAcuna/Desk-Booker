using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.DataAccess.Repositories {
    public class DeskBookingRepository : IDeskBookingRepository {
        private readonly DeskBookerContext Context;

        public DeskBookingRepository(DeskBookerContext Context) {
            this.Context = Context;
        }

        public IEnumerable<DeskBooking> GetAll() {
            return Context.DeskBooking.OrderBy(x => x.Date).ToList();
        }

        public void Save(DeskBooking DeskBooking) {
            Context.DeskBooking.Add(DeskBooking);
            Context.SaveChanges();
        }
    }
}