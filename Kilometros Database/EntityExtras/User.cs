using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KilometrosDatabase {
    public partial class User {
        public UserDataTotalDistance UserDataTotalDistanceSum {
            get {
                if ( this._userDataTotalDistanceSum != null )
                    return this._userDataTotalDistanceSum;

                this._userDataTotalDistanceSum
                    = (
                        from d in this.UserDataTotalDistance
                        where
                            d.User.Guid == this.Guid
                            && d.Activity != DataActivity.Sleep
                        group d by new {
                            userGuid
                                = d.User.Guid
                        } into g
                        select new UserDataTotalDistance {
                            TotalDistance
                                = g.Sum(s => s.TotalDistance),
                            TotalSteps
                                = g.Sum(s => s.TotalSteps),
                            TotalKcal
                                = g.Sum(s => s.TotalKcal),
                            TotalCo2
                                = g.Sum(s => s.TotalCo2),
                            TotalCash
                                = g.Sum(s => s.TotalCash)
                        }
                    ).FirstOrDefault();

                if ( this._userDataTotalDistanceSum == null )
                    this._userDataTotalDistanceSum
                        = new UserDataTotalDistance() {
                            TotalDistance
                                = 0,
                            TotalSteps
                                = 0,
                            TotalKcal
                                = 0,
                            TotalCo2
                                = 0,
                            TotalCash
                                = 0
                        };
                
                return this._userDataTotalDistanceSum;
            }
        }
        private UserDataTotalDistance _userDataTotalDistanceSum = null;
    }
}
