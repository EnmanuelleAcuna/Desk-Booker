using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeskBooker.DataAccess.Repositories {
    public class DeskRepository : IDeskRepository {
        private readonly DeskBookerContext Context;

        public DeskRepository(DeskBookerContext Context) {
            this.Context = Context;
        }

        public IEnumerable<Desk> GetAll() {
            return Context.Desk.ToList();
        }

        public IEnumerable<Desk> GetAvailableDesks(DateTime Date) {
            var BookedDeskIds = Context.DeskBooking.Where(x => x.Date == Date).Select(b => b.DeskId).ToList();
            return Context.Desk.Where(x => !BookedDeskIds.Contains(x.Id)).ToList();
        }
    }
}