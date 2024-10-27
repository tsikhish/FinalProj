namespace Final.Services.ServiceHelper
{
    public class LoanPaymentService
    {
        public static int CalculateCompletedMonths(decimal paidAmount, decimal monthlyPayment)
        {
            return (int)(paidAmount / monthlyPayment);
        }

        public static decimal CalculateRemainPaymentForMonth(decimal remainMonthlyPayment, decimal paidAmount, decimal monthlyPayment)
        {
            return remainMonthlyPayment + paidAmount - monthlyPayment;
        }
    }

}
