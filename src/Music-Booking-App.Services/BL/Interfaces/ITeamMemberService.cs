
using Music_Booking_App.Models.ViewModels;

namespace Music_Booking_App.Services.BL.Interfaces
{
    public interface ITeamMemberService
    {
        Task<PaginatedResponse<List<TeamMemberViewModel>>> GetAllMembersAsync(int pageSize, int pageNumber);
    }
}
