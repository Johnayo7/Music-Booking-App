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
        private readonly IOtpService _otpService;

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
                              IUserService userService,
                              IOtpService otpService)
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
            _otpService = otpService;
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

        public async Task<BaseResponse<CreationViewModel>> IniatiateBookingAsync(BookingRequestModel requestModel, ClaimsPrincipal userClaims)
        {
            Log.Information(
                 $"Starting creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _bookingValidator.ValidateBookingRequest(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<CreationViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);
                var userName = userClaims.FindFirstValue(ClaimTypes.Name);

                var allowedRoles = new[] { UserCategory.EventOrganizer.ToString(), UserCategory.Admin.ToString() };
                if (!allowedRoles.Contains(userRole))
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

                var queryFilter = new Dictionary<string, string>
                {
                    {nameof(Artiste.Id), requestModel.ArtisteId}
                };

                var profileExists = await _artisteQueryRepository.GetByDefaultAsync(queryFilter);
                if (profileExists == null)
                {
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                queryFilter = new Dictionary<string, string>
                {
                    {nameof(Event.Id), requestModel.EventId}
                };

                var eventExists = await _eventQueryRepository.GetByDefaultAsync(queryFilter);
                if (eventExists == null)
                {
                    return BaseResponse<CreationViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                var booking = new Booking
                {
                    ArtisteId = Guid.Parse(requestModel.ArtisteId),
                    EventId = Guid.Parse(requestModel.EventId),
                    ArtisteName = profileExists.Name,
                    EventName = eventExists.Name,
                    EventOrganizerId = Guid.Parse(userId),
                    OrganizerName = userName,
                    ProposedAmount = requestModel.ProposedAmount
                };

                await _bookingCommandRepository.AddAsync(booking);

                Log.Information($"Sending creation request: {JsonConvert.SerializeObject(requestModel)}");

                return BaseResponse<CreationViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<CreationViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<BookingsViewModel>> UpdateBookingAsync(UpdateBookingRequestModel requestModel, ClaimsPrincipal userClaims)
        {
            Log.Information(
                 $"Starting creation process for model: {JsonConvert.SerializeObject(requestModel)}");

            var validationResult = await _bookingValidator.ValidateBookingRequest(requestModel);
            if (!validationResult.IsValid)
                return BaseResponse<BookingsViewModel>
                    .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);

            try
            {
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);
                var userName = userClaims.FindFirstValue(ClaimTypes.Name);

                var allowedRoles = new[] { UserCategory.EventOrganizer.ToString() };
                if (!allowedRoles.Contains(userRole))
                    return BaseResponse<BookingsViewModel>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

                var existingBooking = await _bookingQueryRepository.FindByIdAsync(requestModel.BookingId);
                if (existingBooking == null)
                {
                    return BaseResponse<BookingsViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (existingBooking.EventOrganizerId != Guid.Parse(userId))
                {
                    return BaseResponse<BookingsViewModel>
                      .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);
                }

                var queryFilter = new Dictionary<string, string>
                {
                    {nameof(Artiste.Id), requestModel.ArtisteId}
                };

                var profileExists = await _artisteQueryRepository.GetByDefaultAsync(queryFilter);
                if (profileExists == null)
                {
                    return BaseResponse<BookingsViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                queryFilter = new Dictionary<string, string>
                {
                    {nameof(Event.Id), requestModel.EventId}
                };

                var eventExists = await _eventQueryRepository.GetByDefaultAsync(queryFilter);
                if (eventExists == null)
                {
                    return BaseResponse<BookingsViewModel>
                       .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                existingBooking.ArtisteId = Guid.Parse(requestModel.ArtisteId);
                existingBooking.EventId = Guid.Parse(requestModel.EventId);
                existingBooking.ArtisteName = profileExists.Name;
                existingBooking.EventName = eventExists.Name;
                existingBooking.ProposedAmount = requestModel.ProposedAmount;
                existingBooking.Status = AccountStatus.Pending.ToString();

                await _bookingCommandRepository.UpdateAsync(existingBooking);

                Log.Information($"Sending creation request: {JsonConvert.SerializeObject(requestModel)}");

                return BaseResponse<BookingsViewModel>
                                .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, null);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");

                return BaseResponse<BookingsViewModel>.Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<PaginatedResponse<List<BookingsViewModel>>> GetBookingsByRole(ClaimsPrincipal userClaims, AccountStatus status, int pageSize, int pageNumber, string searchParam = null)
        {
            try
            {
                pageNumber = pageNumber <= 0 ? 1 : pageNumber;
                pageSize = pageSize <= 0 ? 20 : pageSize;

                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);
                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);

                IEnumerable<Booking> getBookings;
                int totalCount;

                if (userRole == UserCategory.Artiste.ToString())
                {
                    if (status == 0 && searchParam == null)
                    {
                        var criteria = new Dictionary<string, string>
                                    {
                                       // {nameof(Company.CreationDate), searchParam },
                                        {nameof(Booking.ArtisteId), userId }
                                    };


                        getBookings = await _bookingQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);

                    }
                    else if (status == 0 && searchParam != null)
                    {
                        //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd");
                        var criteria = new Dictionary<string, string>
                                    {
                                        //{nameof(Booking.ArtisteName), searchParam },
                                        {nameof(Booking.EventName), searchParam },
                                        {nameof(Booking.OrganizerName), searchParam },
                                        {nameof(Booking.ArtisteId), userId }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }
                    else if (status > 0 && searchParam == null)
                    {
                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.Status), status.ToString() },
                                        {nameof(Booking.ArtisteId), userId }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }
                    else
                    {
                        //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd"); 

                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.Status), status.ToString() },
                                        //{nameof(Booking.ArtisteName), searchParam },
                                        {nameof(Booking.EventName), searchParam },
                                        {nameof(Booking.OrganizerName), searchParam },
                                        {nameof(Booking.ArtisteId), userId }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }

                    var bookingView = new List<BookingsViewModel>();
                    foreach (var booking in getBookings)
                    {
                        //var bookingDetails = await _bookingQueryRepository.FindByIdAsync(booking.Id);
                        var view = _mapper.Map<BookingsViewModel>(booking);

                        bookingView.Add(view);
                    }
                    return PaginatedResponse<List<BookingsViewModel>>
                        .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                       bookingView, totalCount);
                }

                if (userRole == UserCategory.EventOrganizer.ToString())
                {


                    if (status == 0 && searchParam == null)
                    {
                        var criteria = new Dictionary<string, string>
                                    {
                                       // {nameof(Company.CreationDate), searchParam },
                                        {nameof(Booking.EventOrganizerId), userId }
                                    };


                        getBookings = await _bookingQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);

                    }
                    else if (status == 0 && searchParam != null)
                    {
                        //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd");
                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.ArtisteName), searchParam },
                                        {nameof(Booking.EventName), searchParam },
                                        //{nameof(Booking.OrganizerName), searchParam },
                                        {nameof(Booking.EventOrganizerId), userId }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }
                    else if (status > 0 && searchParam == null)
                    {
                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.Status), status.ToString() },
                                        {nameof(Booking.EventOrganizerId), userId }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }
                    else
                    {
                        //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd"); 

                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.Status), status.ToString() },
                                        {nameof(Booking.ArtisteName), searchParam },
                                        {nameof(Booking.EventName), searchParam },
                                        //{nameof(Booking.OrganizerName), searchParam },
                                        {nameof(Booking.EventOrganizerId), userId }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }

                    var bookingView = new List<BookingsViewModel>();
                    foreach (var booking in getBookings)
                    {
                        //var bookingDetails = await _bookingQueryRepository.FindByIdAsync(booking.Id);
                        var view = _mapper.Map<BookingsViewModel>(booking);

                        bookingView.Add(view);
                    }
                    return PaginatedResponse<List<BookingsViewModel>>
                        .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                       bookingView, totalCount);
                }

                if (userRole == UserCategory.Admin.ToString())
                {


                    if (status == 0 && searchParam == null)
                    {
                        getBookings = await _bookingQueryRepository.GetAllAsync(pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync();
                    }
                    else if (status == 0 && searchParam != null)
                    {
                        //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd");
                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.ArtisteName), searchParam },
                                        {nameof(Booking.EventName), searchParam },
                                        {nameof(Booking.OrganizerName), searchParam }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }
                    else if (status > 0 && searchParam == null)
                    {
                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.Status), status.ToString() }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsync(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }
                    else
                    {
                        //var dateOnly = searchParam.Date.ToString("yyyy-MM-dd"); 

                        var criteria = new Dictionary<string, string>
                                    {
                                        {nameof(Booking.Status), status.ToString() },
                                        {nameof(Booking.ArtisteName), searchParam },
                                        {nameof(Booking.EventName), searchParam },
                                        {nameof(Booking.OrganizerName), searchParam }
                                    };

                        getBookings = await _bookingQueryRepository.GetByAsyncForPartialMatch(criteria, pageSize, pageNumber);
                        totalCount = await _bookingQueryRepository.GetCountAsync(criteria);
                    }

                    var bookingView = new List<BookingsViewModel>();
                    foreach (var booking in getBookings)
                    {
                        //var bookingDetails = await _bookingQueryRepository.FindByIdAsync(booking.Id);
                        var view = _mapper.Map<BookingsViewModel>(booking);

                        bookingView.Add(view);
                    }
                    return PaginatedResponse<List<BookingsViewModel>>
                        .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful,
                       bookingView, totalCount);
                }

                return PaginatedResponse<List<BookingsViewModel>>
                       .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);

            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return PaginatedResponse<List<BookingsViewModel>>
                    .Failure(ResponseMessages.InternalServerError, StatusCodes.GeneralError);
            }
        }

        public async Task<BaseResponse<ApprovalReviewViewModel>> ReviewBookingRequest(ApprovalReviewRequestModel request, ClaimsPrincipal userClaims)
        {
            try
            {
                var validationResult = await _bookingValidator.ValidateApprovalRequest(request);
                if (!validationResult.IsValid)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);
                var userName = userClaims.FindFirstValue(ClaimTypes.Name);

                if (userRole != UserCategory.Artiste.ToString())
                {
                    return BaseResponse<ApprovalReviewViewModel>
                      .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);
                }

                var booking = await _bookingQueryRepository.FindByIdAsync(Guid.Parse(request.Id));
                if (booking == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }
                if (booking.Status != ApprovalReview.Pending.ToString())
                {
                    return BaseResponse<ApprovalReviewViewModel>
                       .Failure(ResponseMessages.NotPendingApproval, StatusCodes.BadRequest);
                }

                var filter = new Dictionary<string, string>
                {
                    {nameof(Artiste.CreatedBy), userId}
                };

                var artiste = await _artisteQueryRepository.GetByDefaultAsync(filter);
                if (artiste == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (booking.ArtisteId != artiste.Id)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                      .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);
                }

                var organizerDetails = await _userService.FindByIdAsync(booking.EventOrganizerId.ToString());
                if (organizerDetails == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (request.ReviewStatus == ApprovalReview.Approved)
                {
                    booking.Status = ApprovalReview.Approved.ToString();
                    booking.Comment = request.Comment ?? "Approved";


                    var approvalDetails = $@"<strong>Section:</strong> Booking Request Approved for {artiste.Name}<br/>
                         <strong>Status:</strong> Approved <br/>
                         <strong>Comments:</strong> 
                             {booking.Comment}";


                    await _otpService.SendEmailAsync(organizerDetails.Email, "Request Approved", approvalDetails);
                }
                else if (request.ReviewStatus == ApprovalReview.Rejected)
                {
                    booking.Status = ApprovalReview.Rejected.ToString();
                    booking.LastUpdateDate = DateTime.UtcNow;
                    booking.Comment = request.Comment;

                    var rejectionDetails = $@"<strong>Section:</strong> Booking Request Rejected for {artiste.Name}<br/>
                                            <strong>Status:</strong> Rejected<br/>
                                            <strong>Comments:</strong> 
                                                {booking.Comment}";


                    await _otpService.SendEmailAsync(organizerDetails.Email, "Request Rejected", rejectionDetails);
                }

                await _bookingCommandRepository.UpdateAsync(booking);

                var view = _mapper.Map<ApprovalReviewViewModel>(request);

                return BaseResponse<ApprovalReviewViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, view);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return BaseResponse<ApprovalReviewViewModel>
                    .Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<ApprovalReviewViewModel>> ReviewCreatedArtisteProfileRequest(ApprovalReviewRequestModel request, ClaimsPrincipal userClaims)
        {
            try
            {
                var validationResult = await _bookingValidator.ValidateApprovalRequest(request);
                if (!validationResult.IsValid)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);
                var userName = userClaims.FindFirstValue(ClaimTypes.Name);

                if (userRole != UserCategory.Admin.ToString())
                {
                    return BaseResponse<ApprovalReviewViewModel>
                      .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);
                }

                var artisteProfile = await _artisteQueryRepository.FindByIdAsync(Guid.Parse(request.Id));
                if (artisteProfile == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }
                if (artisteProfile.AccountStatus != ApprovalReview.Pending.ToString())
                {
                    return BaseResponse<ApprovalReviewViewModel>
                       .Failure(ResponseMessages.NotPendingApproval, StatusCodes.BadRequest);
                }

                var artisteDetails = await _userService.FindByIdAsync(artisteProfile.CreatedBy.ToString());
                if (artisteDetails == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (request.ReviewStatus == ApprovalReview.Approved)
                {
                    artisteProfile.AccountStatus = ApprovalReview.Approved.ToString();
                    artisteProfile.Comment = request.Comment ?? "Approved";

                    var approvalDetails = $@"<strong>Section:</strong> Create Profile Request Approved<br/>
                                            <strong>Status:</strong> Approved <br/>
                                            <strong>Comments:</strong> 
                                                {artisteProfile.Comment}";

                    await _otpService.SendEmailAsync(artisteDetails.Email, "Request Approved", approvalDetails);

                }
                else if (request.ReviewStatus == ApprovalReview.Rejected)
                {
                    artisteProfile.AccountStatus = ApprovalReview.Rejected.ToString();
                    artisteProfile.LastUpdateDate = DateTime.UtcNow;
                    artisteProfile.Comment = request.Comment;

                    var rejectionDetails = $@"<strong>Section:</strong> Create Profile Request Rejected<br/>
                                            <strong>Status:</strong> Rejected<br/>
                                            <strong>Comments:</strong> 
                                                {request.Comment}";

                    await _otpService.SendEmailAsync(artisteDetails.Email, "Request Rejected", rejectionDetails);
                }

                artisteProfile.ReviewerName = userName;
                artisteProfile.ReviewerId = Guid.Parse(userId);
                await _artisteCommandRepository.UpdateAsync(artisteProfile);

                var view = _mapper.Map<ApprovalReviewViewModel>(request);

                return BaseResponse<ApprovalReviewViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, view);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return BaseResponse<ApprovalReviewViewModel>
                    .Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

        public async Task<BaseResponse<ApprovalReviewViewModel>> ReviewCreatedEventRequest(ApprovalReviewRequestModel request, ClaimsPrincipal userClaims)
        {
            try
            {
                var validationResult = await _bookingValidator.ValidateApprovalRequest(request);
                if (!validationResult.IsValid)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(string.Join(" | ", validationResult.Errors), StatusCodes.ModelValidationError);
                }

                var userId = userClaims.FindFirstValue(ClaimTypes.Sid);
                var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
                var userRole = userClaims.FindFirstValue(ClaimTypes.Role);
                var userName = userClaims.FindFirstValue(ClaimTypes.Name);

                if (userRole != UserCategory.Admin.ToString())
                {
                    return BaseResponse<ApprovalReviewViewModel>
                      .Failure(ResponseMessages.UnauthorizedAccess, StatusCodes.Forbidden);
                }

                var createdEvent = await _eventQueryRepository.FindByIdAsync(Guid.Parse(request.Id));
                if (createdEvent == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (createdEvent.EventStatus != ApprovalReview.Pending.ToString())
                {
                    return BaseResponse<ApprovalReviewViewModel>
                       .Failure(ResponseMessages.NotPendingApproval, StatusCodes.BadRequest);
                }

                var organizerDetails = await _userService.FindByIdAsync(createdEvent.EventOrganizerId.ToString());
                if (organizerDetails == null)
                {
                    return BaseResponse<ApprovalReviewViewModel>
                        .Failure(ResponseMessages.NoRecordFound, StatusCodes.NoRecordFound);
                }

                if (request.ReviewStatus == ApprovalReview.Approved)
                {
                    createdEvent.EventStatus = ApprovalReview.Approved.ToString();
                    createdEvent.Comment = request.Comment ?? "Approved";

                    var approvalDetails = $@"<strong>Section:</strong> Create Event Request Approved for {createdEvent.Name}<br/>
                                            <strong>Status:</strong> Approved <br/>
                                            <strong>Comments:</strong> 
                                                {createdEvent.Comment}";


                    await _otpService.SendEmailAsync(organizerDetails.Email, "Request Approved", approvalDetails);
                }
                else if (request.ReviewStatus == ApprovalReview.Rejected)
                {
                    createdEvent.EventStatus = ApprovalReview.Rejected.ToString();
                    createdEvent.LastUpdateDate = DateTime.UtcNow;
                    createdEvent.Comment = request.Comment;

                    var rejectionDetails = $@"<strong>Section:</strong> Create Event Request Rejected for {createdEvent.Name}<br/>
                                            <strong>Status:</strong> Rejected<br/>
                                            <strong>Comments:</strong> 
                                                {request.Comment}";


                    await _otpService.SendEmailAsync(organizerDetails.Email, "Request Rejected", rejectionDetails);
                }

                createdEvent.ReviewerName = userName;
                createdEvent.ReviewerId = Guid.Parse(userId);
                await _eventCommandRepository.UpdateAsync(createdEvent);

                var view = _mapper.Map<ApprovalReviewViewModel>(request);

                return BaseResponse<ApprovalReviewViewModel>
                    .Success(ResponseMessages.OperationSuccessful, StatusCodes.Successful, view);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message} \n StackTrace: {ex.StackTrace}");
                return BaseResponse<ApprovalReviewViewModel>
                    .Failure(ResponseMessages.GeneralError, StatusCodes.FatalError);
            }
        }

    }
}
