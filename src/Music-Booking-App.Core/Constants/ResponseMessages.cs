

namespace Music_Booking_App.Core.Constants
{
    public class ResponseMessages
    {
        // General
        public const string GeneralError =
            "An error occured while processing this request. Kindly refer to the status code.";
        public const string OperationSuccessful = "Operation successful.";
        public const string NoKybRecord = "No Kyb stage found this organization, start with creating an organization";
        public const string KybComplete = "KybStages have been completed";
        public const string OperationUnsuccessful = "Operation unsuccessful.";
        public const string WrongCredentials = "Wrong email or password";
        public const string Deactivated = "This account has been deactivated";


        public const string OperationProcessing = "Operation processing.";
        //otp
        public const string OtpAlreadyUsed = "OTP has already been used";
        public const string TimeoutOrExpired = "The session has expired, kindly try again";
        public const string AccountLocked = "Your account is locked. Please try again after 10mins.";
        public const string InvalidAttempt = "Invalid login attempt";
        public const string InvalidToken = "Invalid token";

        //verify email
        public const string VerifyEmail = "Email not yet verified, verify then try again";
        // Validation
        public const string UnAuthorized = "UnAuthorized User";
        public static string NoMessageFound = "No message found.";


        public const string InternalServerError = "Server Error, try again.";
        public const string UnauthorizedOrganizationAccess = "You do not have authorization to create an organization with the specified credentials.";
        public const string UnauthorizedAccess = "You do not have permission to access this resource.";

        public const string DuplicateKeyMessage = "duplicate key value";
        public const string DuplicateAccountNumber =
            "Duplicate account number found. Kindly confirm that an account of same category is not being created for this customer.";
        public const string DuplicateRequestReference =
            "Duplicate request reference found. Kindly retry with a unique request reference.";
        public const string DuplicateAccountRecord =
            "A duplicate record exists for the account to be created. Kindly review the customer information provided.";
        public const string DuplicateName = "Duplicate name found. Kindly retry with a different name";
        public const string DuplicateRecord = "Duplicate record. Kindly try again";
        public const string InitialDepositLessThan = "Initial deposit amount is lower than the minimum amount for this product.";
        public const string TargetNotAllowed = "Goal amount is not allowed for this product.";
        public const string TenureNotAllowed = "Tenure is not allowed for this product.";
        public const string InsufficientFunds = "You have insufficient funds to carry out this transaction.";
        public const string SavingAccountDoesNotExist = "Savings account does not exist.";
        public const string SpendAndStashConfigAlreadyExists = "This account already has an active spend and stash configuration.";
        public const string SpendAndStashConfigDoesNotExists = "No Spend and Stash configuration found for this account.";
        public const string NoProductFound = "Invalid product ID.";
        public const string InvalidAccountBalance = "Invalid account balance data.";
        public const string SavingProductNotFound = "Savings product not found.";
        public const string NoRecordFound = "No record found.";
        public const string NotPendingApproval = "The request cannot be processed as it is not currently pending approval.";
        public const string DeserializationToNull = "Deserialization resulting in null.";
        //Postings
        public const string ErrorCallingPostings = "Error calling Postings Engine.";
        public const string DuplicateAutoDebitConfiguration = "An auto debit configuration already exist for the savings account.";
        public const string InvalidGuid = "Id must be a valid Guid";
        public const string InvalidAccountNumber = "Invalid account number.";
        public const string PartialWithdrawalNotAllowed = "Partial withdrawals are not allowed for accounts with a tenure.";
        public const string DuplicateBucketName = "The bucket name you provided is already in use.";
        public const string UnableToRetrieveSavingsAccountBalance = "Savings account balance cannot be retrieved at this time";
        public const string InvalidNameMessage = "Name cannot be empty or consist only of whitespace.";
        public const string AccountBalanceTooLowForTransaction = "Your account balance is too low to complete this transaction.";

        public static string InitialDepositLessThanMinimum(decimal minimumAmount)
        {
            var formattedAmount = $"N{minimumAmount:N}";
            return $"The initial deposit amount is lower than the minimum amount ({formattedAmount}) for this product.";
        }

        public static string UnableToRetrieveAccountBalance(string accountNumber)
        {
            return $"Unable to Retrieve Account Balance for {accountNumber}";
        }
    }
}
