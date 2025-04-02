using Music_Booking_App.Models.Enums;

namespace Music_Booking_App.Models.RequestModels
{
    public class ApprovalReviewRequestModel
    {
        public string Id { get; set; }
        public ApprovalReview ReviewStatus { get; set; }  // Approved or Rejected
        public string Comment { get; set; }
    }
}
