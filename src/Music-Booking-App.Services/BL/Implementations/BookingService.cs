using AutoMapper;
using Music_Booking_App.Core.Constants;
using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Models.Enums;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;
using Music_Booking_App.Services.Authentication.Interfaces;
using Music_Booking_App.Services.BL.Interfaces;
using Music_Booking_App.Services.Helpers;
using Newtonsoft.Json;
using Serilog;
using System.Security.Claims;

namespace Music_Booking_App.Services.BL.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IDapperCommandRepository<Artiste> _artisteCommandRepository;
        private readonly IDapperQueryRepository<Artiste> _artisteQueryRepository;
        private readonly IDapperCommandRepository<Event> _eventCommandRepository;
        private readonly IDapperQueryRepository<Event> _eventQueryRepository;
        private readonly IDapperCommandRepository<Booking> _bookingCommandRepository;
        private readonly IDapperQueryRepository<Booking> _bookingQueryRepository;
        private readonly IDapperCommandRepository<Ticket> _ticketCommandRepository;
        private readonly IDapperQueryRepository<Ticket> _ticketQueryRepository;
        private readonly IMapper _mapper;
        private readonly IBookingValidator _bookingValidator;
        private readonly IUserService _userService;

        public BookingService(IDapperCommandRepository<Artiste> artisteCommandRepository,
                              IDapperQueryRepository<Artiste> artisteQueryRepository,
                              IDapperCommandRepository<Event> eventCommandRepository,
                              IDapperQueryRepository<Event> eventQueryRepository,
                              IDapperCommandRepository<Booking> bookingCommandRepository,
                              IDapperQueryRepository<Booking> bookingQueryRepository,
                              IDapperCommandRepository<Ticket> ticketCommandRepository,
                              IDapperQueryRepository<Ticket> ticketQueryRepository,
                              IMapper mapper,
                              IBookingValidator bookingValidator,
                              IUserService userService)
        {
            _artisteCommandRepository = artisteCommandRepository;
            _artisteQueryRepository = artisteQueryRepository;
            _eventCommandRepository = eventCommandRepository;
            _eventQueryRepository = eventQueryRepository;
            _bookingCommandRepository = bookingCommandRepository;
            _bookingQueryRepository = bookingQueryRepository;
            _ticketCommandRepository = ticketCommandRepository;
            _ticketQueryRepository = ticketQueryRepository;
            _mapper = mapper;
            _bookingValidator = bookingValidator;
            _userService = userService;
        }

        public async Task<BaseResponse<CreationViewModel>> CreateArtisteProfileAsync(CreateArtisteRequestModel requestModel, ClaimsPrincipal userClaims)
        {
            Log.Information(
                 $"Starting creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _bookingValidator.ValidateCreateArtisteProfileRequest(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<CreationViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);

                if (userRole != UserCategory.Artiste.ToString())
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

                var queryFilter = new Dictionary<string, string>
                {
                    {nameof(Artiste.CreatedBy), userId}
                };

                var profileExists = await _artisteQueryRepository.GetByDefaultAsync(queryFilter);
                if (profileExists != null)
                {
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.DuplicateRecord, StatusCodes.DuplicateRecord);
                }
                Log.Information($"Sending creation request: {JsonConvert.SerializeObject(requestModel)}");

                var profile = _mapper.Map<Artiste>(requestModel);
                profile.CreatedBy = Guid.Parse(userId);
                await _artisteCommandRepository.AddAsync(profile);

                return BaseResponse<CreationViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<CreationViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<CreationViewModel>> CreateEventAsync(CreateEventRequestModel requestModel, ClaimsPrincipal userClaims)
        {
            Log.Information(
                 $"Starting creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _bookingValidator.ValidateCreateEventRequest(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<CreationViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);

                if (userRole != UserCategory.EventOrganizer.ToString())
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

                var queryFilter = new Dictionary<string, string>
                {
                    {nameof(Event.Name), requestModel.Name},
                    {nameof(Event.EventOrganizerId), userId}
                };

                var eventExists = await _eventQueryRepository.GetByDefaultAsync(queryFilter);
                if (eventExists != null)
                {
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.DuplicateRecord, StatusCodes.DuplicateRecord);
                }
                Log.Information($"Sending creation request: {JsonConvert.SerializeObject(requestModel)}");

                var newEvent = _mapper.Map<Event>(requestModel);
                newEvent.EventOrganizerId = Guid.Parse(userId);
                await _eventCommandRepository.AddAsync(newEvent);

                return BaseResponse<CreationViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<CreationViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<PaginatedResponse<List<ArtistesViewModel>>> GetAllArtistes(AccountStatus status, int pageSize, int pageNumber, string searchParam = null)
        {
            try
            {
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 20 : pageSize;

                IEnumerable<Artiste> getArtistes;
                int totalCount;

                if (status == 0 && searchParam == null)
                {
                    getArtistes = await _artisteQueryRepository.GetAllAsync(pageSize, pageNumber);
                    totalCount = await _artisteQueryRepository.GetCountAsync();
                }
                else if (status == 0 && searchParam != null)
                {
                    //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd");
                    var criteria = new Dictionary<string, string>
                                    {
                                       // {nameof(Company.CreationDate), searchParam },
                                        {nameof(Artiste.Name), searchParam },
                                        {nameof(Artiste.Genre), searchParam }
                                    };

                    getArtistes = await _artisteQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                    totalCount = await _artisteQueryRepository.GetCountAsync(criteria);
                }
                else if (status > 0 && searchParam == null)
                {
                    var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Artiste.AccountStatus), status.ToString() }
                                    };

                    getArtistes = await _artisteQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                    totalCount = await _artisteQueryRepository.GetCountAsync(criteria);
                }
                else
                {
                    //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd"); 

                    var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Artiste.AccountStatus), status.ToString() },
                                        {nameof(Artiste.Name), searchParam },
                                        {nameof(Artiste.Genre), searchParam }
                                    };

                    getArtistes = await _artisteQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                    totalCount = await _artisteQueryRepository.GetCountAsync(criteria);
                }

                var artistesView = new List<ArtistesViewModel>();
                foreach (var artiste in getArtistes)
                {
                    var artisteDetails = await _artisteQueryRepository.FindByIdAsync(artiste.Id);
                    var view = _mapper.Map<ArtistesViewModel>(artiste);

                    artistesView.Add(view);
                }
                return PaginatedResponse<List<ArtistesViewModel>>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                   artistesView, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return PaginatedResponse<List<ArtistesViewModel>>
                    .Failure(ResponseMessages.InternalServerError, StatusCodes.GeneralError);
            }
        }

        public async Task<BaseResponse<ArtisteDetailViewModel>> GetArtiste(string id)
        {
            Log.Information(
               $"Starting test delete process for user with Id: {JsonConvert.SerializeObject(id)}");

            var validationResult = _bookingValidator.ValidateGuid(id);
            if (!validationResult.IsValid)
                return BaseResponse<ArtisteDetailViewModel>
                    .Failure(validationResult.Message, StatusCodes.ModelValidationError);
            try
            {
                var artiste = await _artisteQueryRepository.FindByIdAsync(Guid.Parse(id));

                if (artiste == null)
                    return BaseResponse<ArtisteDetailViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);


                var artisteView = _mapper.Map<ArtisteDetailViewModel>(artiste);

                return BaseResponse<ArtisteDetailViewModel>
                   .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, artisteView);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<ArtisteDetailViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<PaginatedResponse<List<EventsViewModel>>> GetAllEvents(AccountStatus status, int pageSize, int pageNumber, string searchParam = null)
        {
            try
            {
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 20 : pageSize;

                IEnumerable<Event> allEvents;
                int totalCount;

                if (status == 0 && searchParam == null)
                {
                    allEvents = await _eventQueryRepository.GetAllAsync(pageSize, pageNumber);
                    totalCount = await _eventQueryRepository.GetCountAsync();
                }
                else if (status == 0 && searchParam != null)
                {
                    //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd");
                    var criteria = new Dictionary<string, string>
                                    {
                                       // {nameof(Company.CreationDate), searchParam },
                                        {nameof(Event.Name), searchParam },
                                        {nameof(Event.Location), searchParam },
                                        {nameof(Event.OrganizerName), searchParam }
                                    };

                    allEvents = await _eventQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                    totalCount = await _eventQueryRepository.GetCountAsync(criteria);
                }
                else if (status > 0 && searchParam == null)
                {
                    var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Event.EventStatus), status.ToString() }
                                    };

                    allEvents = await _eventQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                    totalCount = await _eventQueryRepository.GetCountAsync(criteria);
                }
                else
                {
                    //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd"); 

                    var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Event.EventStatus), status.ToString() },
                                        {nameof(Event.Name), searchParam },
                                        {nameof(Event.Location), searchParam },
                                        {nameof(Event.OrganizerName), searchParam }
                                    };

                    allEvents = await _eventQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                    totalCount = await _eventQueryRepository.GetCountAsync(criteria);
                }

                var eventsView = new List<EventsViewModel>();
                foreach (var thisEvent in allEvents)
                {
                    var eventDetails = await _eventQueryRepository.FindByIdAsync(thisEvent.Id);
                    var view = _mapper.Map<EventsViewModel>(thisEvent);

                    eventsView.Add(view);
                }
                return PaginatedResponse<List<EventsViewModel>>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                   eventsView, totalCount);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return PaginatedResponse<List<EventsViewModel>>
                    .Failure(ResponseMessages.InternalServerError, StatusCodes.GeneralError);
            }
        }

        public async Task<BaseResponse<EventDetailsViewModel>> GetEvent(string id)
        {
            Log.Information(
               $"Starting fetch  process for event with Id: {JsonConvert.SerializeObject(id)}");

            var validationResult = _bookingValidator.ValidateGuid(id);
            if (!validationResult.IsValid)
                return BaseResponse<EventDetailsViewModel>
                    .Failure(validationResult.Message, StatusCodes.ModelValidationError);
            try
            {
                var eventDetails = await _eventQueryRepository.FindByIdAsync(Guid.Parse(id));

                if (eventDetails == null)
                    return BaseResponse<EventDetailsViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);


                var eventView = _mapper.Map<EventDetailsViewModel>(eventDetails);

                return BaseResponse<EventDetailsViewModel>
                   .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, eventView);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<EventDetailsViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<ArtisteDetailViewModel>> UpdateArtisteProfileAsync(UpdateArtisteRequestModel requestModel, ClaimsPrincipal userClaims)
        {
            Log.Information(
                 $"Starting creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _bookingValidator.ValidateCreateArtisteProfileRequest(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<ArtisteDetailViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);

                if (userRole != UserCategory.Artiste.ToString())
                    return BaseResponse<ArtisteDetailViewModel>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

                var queryFilter = new Dictionary<string, string>
                {
                    {nameof(Artiste.Id), requestModel.Id.ToString()},
                    {nameof(Artiste.CreatedBy), userId}
                };

                var profileExists = await _artisteQueryRepository.GetByDefaultAsync(queryFilter);
                if (profileExists == null)
                {
                    return BaseResponse<ArtisteDetailViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }
                Log.Information($"Sending creation request: {JsonConvert.SerializeObject(requestModel)}");

                profileExists.Name = requestModel.Name;
                profileExists.Genre = requestModel.Genre;
                profileExists.Bio = requestModel.Bio;
                profileExists.BookingRate = requestModel.BookingRate;
                profileExists.AccountStatus = AccountStatus.Pending.ToString();
                profileExists.LastUpdateDate = DateTime.UtcNow;
                await _artisteCommandRepository.UpdateAsync(profileExists);

                var view = _mapper.Map<ArtisteDetailViewModel>(profileExists);

                return BaseResponse<ArtisteDetailViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, view);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<ArtisteDetailViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<EventDetailsViewModel>> UpdateEventAsync(UpdateEventRequestModel requestModel, ClaimsPrincipal userClaims)
        {
            Log.Information(
                 $"Starting creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _bookingValidator.ValidateCreateEventRequest(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<EventDetailsViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);

                if (userRole != UserCategory.EventOrganizer.ToString())
                    return BaseResponse<EventDetailsViewModel>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

                var queryFilter = new Dictionary<string, string>
                {
                    {nameof(Event.Id), requestModel.Id.ToString()},
                    {nameof(Event.EventOrganizerId), userId}
                };

                var eventExists = await _eventQueryRepository.GetByDefaultAsync(queryFilter);
                if (eventExists == null)
                {
                    return BaseResponse<EventDetailsViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }
                Log.Information($"Sending creation request: {JsonConvert.SerializeObject(requestModel)}");

                eventExists.Name = requestModel.Name;
                eventExists.TicketPrice = requestModel.TicketPrice;
                eventExists.Location = requestModel.Location;
                eventExists.LastUpdateDate = DateTime.UtcNow;
                eventExists.EventStatus = AccountStatus.Pending.ToString();
                await _eventCommandRepository.UpdateAsync(eventExists);

                var view = _mapper.Map<EventDetailsViewModel>(eventExists);


                return BaseResponse<EventDetailsViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, view);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<EventDetailsViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }



    }
}
