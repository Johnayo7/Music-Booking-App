
using Music_Booking_App.Core.Constants;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.ViewModels;
using Music_Booking_App.Services.Authentication.Interfaces;
using Music_Booking_App.Services.BL.Interfaces;
using AutoMapper;
using Newtonsoft.Json;
using Serilog;

namespace Music_Booking_App.Services.BL.Implementations
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly IDapperQueryRepository<User> _userQueryRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TeamMemberService(IDapperQueryRepository<User> userQueryRepository,
                                 IUserService userService,
                                 IMapper mapper)
        {
            _userQueryRepository = userQueryRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<List<TeamMemberViewModel>>> GetAllMembersAsync(int pageSize, int pageNumber)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            try
            {
                var allMembers = await _userQueryRepository.GetAllAsync(pageSize, pageNumber);

                if (!allMembers.Any())
                    return PaginatedResponse<List<TeamMemberViewModel>>
                                                 .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);

                var totalCount = await _userQueryRepository.GetCountAsync();

                var membersView = new List<TeamMemberViewModel>();
                //var view = new TeamMemberViewModel();
                foreach (var member in allMembers)
                {
                    var view = _mapper.Map<TeamMemberViewModel>(member);
                    view.Status = member.AccountStatus;

                    if (member.IsSuperAdmin)
                        view.Role = Role.SuperAdmin.ToString();
                    else view.Role = Role.Admin.ToString();

                    membersView.Add(view);
                }

                Log.Information(
                 $"Successfully retrieved all tests: {JsonConvert.SerializeObject(allMembers)}");

                return PaginatedResponse<List<TeamMemberViewModel>>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                   membersView, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return PaginatedResponse<List<TeamMemberViewModel>>
                                        .Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }
    }
}
