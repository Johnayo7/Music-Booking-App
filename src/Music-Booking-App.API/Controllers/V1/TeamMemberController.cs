
using Music_Booking_App.API.Helpers;
using Music_Booking_App.Services.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Music_Booking_App.API.Controllers.V1
{
    [Route("api/v1/auth")]
    [ApiController]
    public class TeamMemberController : ControllerBase
    {
        private readonly ITeamMemberService _teamMemberService;

        public TeamMemberController(ITeamMemberService teamMemberService)
        {
            _teamMemberService = teamMemberService;
        }

        [HttpGet("GetAllMembers")]
        public async Task<IActionResult> GetAllMembers(int pageSize, int pageNumber)
        {
            var response = await _teamMemberService.GetAllMembersAsync(pageSize, pageNumber);
            return HttpResponseHelper.GetHttpResponse(response);
        }
    }
}
