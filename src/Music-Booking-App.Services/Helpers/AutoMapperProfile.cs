
using AutoMapper;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Models.RequestModels;
using Music_Booking_App.Models.ViewModels;

namespace Music_Booking_App.Services.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateTestRequestModel, Test>();
            CreateMap<UpdateTestRequestModel, Test>();
            CreateMap<Test, TestViewModel>();

            CreateMap<OTP, OtpViewModel>();
            CreateMap<SendOtpRequestModel, OTP>();

            CreateMap<SignUpRequestModel, SignUpViewModel>();
            CreateMap<User, SignUpViewModel>();

            CreateMap<User, LoginViewModel>();

            CreateMap<User, PasswordViewModel>();
            CreateMap<User, TeamMemberViewModel>();

            CreateMap<CreateArtisteRequestModel, Artiste>();
            CreateMap<CreateEventRequestModel, Event>();
            CreateMap<Artiste, ArtistesViewModel>();
            CreateMap<Artiste, ArtisteDetailViewModel>();
            CreateMap<Event, EventDetailsViewModel>();
            CreateMap<Event, EventsViewModel>();
        }
    }
}
