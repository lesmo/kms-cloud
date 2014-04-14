using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public partial class User {
        public long UserDataTotalDistanceSum {
            get {
                if ( this._userDataTotalDistanceSum.HasValue )
                    return this._userDataTotalDistanceSum.Value;

                var result
                    = (
                        from d in this.UserDataTotalDistance
                        where
                            d.User.Guid == this.Guid
                            && d.Activity != DataActivity.Sleep
                        group d by new {
                            userGuid
                                = d.User.Guid
                        } into g
                        select new {
                            totalDistance
                                = g.Sum(s => s.TotalDistance)
                        }
                    ).FirstOrDefault();
                
                this._userDataTotalDistanceSum
                    = result == null
                    ? 0
                    : result.totalDistance;

                return this._userDataTotalDistanceSum.Value;
            }
        }
        private long? _userDataTotalDistanceSum = null;
    }
}
