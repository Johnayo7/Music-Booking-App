

namespace Music_Booking_App.Data.Helpers
{
    public static class CustomQueries
    {

        public const string GetById = "SELECT * FROM #table WHERE \"Id\" = @ID and \"IsDeleted\" is false";

        public const string DeleteById = "DELETE FROM #table WHERE \"Id\" = @ID";

        public const string ProcessDailyAutoDepositsQuery = @"
        SELECT *
        FROM #table
        WHERE
        ""IsEnabled"" = true
        AND
            (
                lower(""AutoDebitFrequency"") = 'daily' OR
                (
                    lower(""AutoDebitFrequency"") = 'weekly' AND
                    lower(""DayOfWeek"") = lower(to_char(current_date, 'Day'))
                ) OR
                (
                    lower(""AutoDebitFrequency"") = 'monthly' AND
                    CAST(""DayOfMonth"" AS TEXT) = to_char(current_date, 'DD')
                )
            ) AND
            CAST(""StartDate"" AS DATE) <= current_date AND
            CAST(""EndDate"" AS DATE) >= current_date AND
            ""AutoDebitConfigurations"".""SavingsAccountNumber"" NOT IN (
                SELECT DISTINCT ""SavingsAccountNumber""
                FROM ""Deposits""
                WHERE
                    CAST(""CreationDate"" AS DATE) = current_date AND
                    ""DepositType"" = 'Auto'
            );";

        public static string SelectPendingDailyInterestAccruals(string date)
        {
            return $@"UPDATE public.""InterestAccruals"" AS ia
                SET ""Status"" = 'Processing'
                FROM public.""SavingsAccounts"" AS sa
                JOIN public.""SavingsProducts"" AS sp ON sa.""ProductId""::uuid = sp.""Id""
                WHERE ia.""AccountNumber"" = sa.""SavingsAccountNumber""
                AND ia.""Status"" = 'Pending'
                AND sp.""HasTenure"" = TRUE
                AND sa.""MaturityDate""::DATE = '{date}';
                SELECT 
                    ia.""AccountNumber"",
                    EXTRACT(YEAR FROM ia.""FinancialDate"") AS ""Year"",
                    EXTRACT(MONTH FROM ia.""FinancialDate"") AS ""Month"",
                    SUM(ia.""InterestDue"") AS ""TotalInterestDue""
                FROM public.""InterestAccruals"" AS ia
                JOIN public.""SavingsAccounts"" AS sa ON ia.""AccountNumber"" = sa.""SavingsAccountNumber""
                JOIN public.""SavingsProducts"" AS sp ON sa.""ProductId""::uuid = sp.""Id""
                WHERE ia.""Status"" = 'Processing'
                AND sp.""HasTenure"" = TRUE
                AND sa.""MaturityDate""::DATE = '{date}'
                GROUP BY 
                    ia.""AccountNumber"", 
                    EXTRACT(YEAR FROM ia.""FinancialDate""), 
                    EXTRACT(MONTH FROM ia.""FinancialDate"");";
        }

        public static string UpdateSuccessfulInterestAccrualsPayment(string year, int month, string accountNumber)
        {
            return $@"UPDATE public.""InterestAccruals""
                    SET ""Status"" = 'Success'
                    WHERE ""Status"" = 'Processing'
                    AND EXTRACT(YEAR FROM ""FinancialDate"") = '{year}'
                    AND EXTRACT(MONTH FROM ""FinancialDate"") = '{month}'
                    AND ""AccountNumber"" = '{accountNumber}';";
        }

        public static string SelectInterestPaymentsOnEarlyWithdrawalFromTenuredAccount(string savingsAccountNumber)
        {
            return $@"UPDATE public.""InterestAccruals""
            SET ""Status"" = 'Processing'
            WHERE ""Status"" = 'Pending'
            AND ""AccountNumber"" = '{savingsAccountNumber}';
            SELECT
                public.""InterestAccruals"".""AccountNumber"",
                EXTRACT(YEAR FROM public.""InterestAccruals"".""FinancialDate"") AS ""Year"",
                EXTRACT(MONTH FROM public.""InterestAccruals"".""FinancialDate"") AS ""Month"",
                SUM(public.""InterestAccruals"".""InterestDue"") AS ""TotalInterestDue""
            FROM
                public.""InterestAccruals""
            WHERE
                public.""InterestAccruals"".""Status"" = 'Processing'
            AND 
                public.""InterestAccruals"".""AccountNumber"" = '{savingsAccountNumber}'
            GROUP BY
                public.""InterestAccruals"".""AccountNumber"",
                EXTRACT(YEAR FROM public.""InterestAccruals"".""FinancialDate""),
                EXTRACT(MONTH FROM public.""InterestAccruals"".""FinancialDate"");";
        }

        public static string SelectPendingMonthlyInterestAccruals(int year, int month)
        {
            return $@"
                    UPDATE public.""InterestAccruals"" AS ia
                    SET ""Status"" = 'Processing'
                    FROM public.""SavingsAccounts"" AS sa
                    JOIN public.""SavingsProducts"" AS sp ON sa.""ProductId""::text = sp.""Id""::text
                    WHERE ia.""Status"" = 'Pending'
                    AND EXTRACT(YEAR FROM ia.""FinancialDate"") = '{year}'
                    AND EXTRACT(MONTH FROM ia.""FinancialDate"") = '{month}'
                    AND sa.""SavingsAccountNumber"" = ia.""AccountNumber""
                    AND sp.""HasTenure"" = FALSE;

                    SELECT ia.""AccountNumber"", 
                        EXTRACT(YEAR FROM ia.""FinancialDate"") AS ""Year"", 
                        EXTRACT(MONTH FROM ia.""FinancialDate"") AS ""Month"",
                        SUM(ia.""InterestDue"") AS ""TotalInterestDue""
                    FROM public.""InterestAccruals"" AS ia
                    JOIN public.""SavingsAccounts"" AS sa ON ia.""AccountNumber"" = sa.""SavingsAccountNumber""
                    JOIN public.""SavingsProducts"" AS sp ON sa.""ProductId""::uuid = sp.""Id""
                    WHERE ia.""Status"" = 'Processing'
                    AND sp.""HasTenure"" = FALSE
                    AND EXTRACT(YEAR FROM ia.""FinancialDate"") = '{year}'
                    AND EXTRACT(MONTH FROM ia.""FinancialDate"") = '{month}'
                    GROUP BY ia.""AccountNumber"", 
                        EXTRACT(YEAR FROM ia.""FinancialDate""), 
                        EXTRACT(MONTH FROM ia.""FinancialDate"");";
        }

        public static string ValidateSavingsBucket(string bucketName, string userId)
        {
            return $"SELECT * FROM public.\"SavingsAccounts\" WHERE UPPER(\"BucketName\") = '{bucketName}' AND \"UserId\" = '{userId}' AND \"IsDeleted\" = 'false' ORDER BY \"CreationDate\" LIMIT 1;";
        }
    }
}
