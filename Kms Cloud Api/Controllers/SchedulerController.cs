using Kms.Cloud.Api.MagicTriggers;
using Kms.Cloud.Api.Properties;
using System.Web.Http;

namespace Kms.Cloud.Api.Controllers {
    [AllowAnonymous]
    public class SchedulerController : BaseController {
        [HttpGet, Route("scheduler/{key}/tips")]
        public IHttpActionResult Tips(string key) {
            if ( key != Settings.Default.SchedulerKey )
                return BadRequest();
            
            foreach ( var user in Database.UserStore.GetAll() )
                new RewardTipTrigger(user, Database).TriggerTipsByDays();

            Database.SaveChanges();

            return Ok();
        }
    }
}
