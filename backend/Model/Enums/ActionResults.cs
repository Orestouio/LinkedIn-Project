using Microsoft.AspNetCore.Mvc;

namespace BackendApp.Model.Enums
{
    public enum UpdateResult{
        KeyAlreadyExists,
        NotFound,
        Ok,
        Unauthorised
    }



    public static class ActionResultEnumExtensions
    {
        public static IActionResult ToResultObject(this UpdateResult updateResult, ControllerBase controller)
        {
            return updateResult switch
            {
                UpdateResult.KeyAlreadyExists => controller.Conflict(),
                UpdateResult.NotFound => controller.NotFound(),
                UpdateResult.Ok => controller.Ok(),
                _  => controller.StatusCode(500, "Something went terribly wrong for you to see this.") 
            };
        }
    }
}