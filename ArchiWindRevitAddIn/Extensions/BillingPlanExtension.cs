using ArchiwindRevitAddIn.Api.Models;

namespace ArchiWindRevitAddIn.Extensions
{
    public static class BillingPlanExtension
    {
        public static string HumanName(this BillingPlan billingPlan)
        {
            return billingPlan.Name switch
            {
                BillingPlan_name.Basic => "Basic",
                BillingPlan_name.Basic_ppc => "Basic (pay per credit)",
                BillingPlan_name.Professional => "Professional",
                BillingPlan_name.Explorer => "Explorer",
                BillingPlan_name.Enterprise => "Enterprise",
                _ => throw new NotImplementedException($"{billingPlan.Name} not covered"),
            };
        }

        public static int DraftCredits(this BillingPlan billingPlan)
        {
            if (billingPlan.DraftCredits?.String == "inf")
            {
                return int.MaxValue;
            }

            return billingPlan.DraftCredits?.Integer ?? 0;
        }

        public static int DetailedCredits(this BillingPlan billingPlan)
        {
            if (billingPlan.DetailedCredits?.String == "inf")
            {
                return int.MaxValue;
            }

            return billingPlan.DetailedCredits?.Integer ?? 0;
        }

        public static string DraftCreditsString(this BillingPlan billingPlan)
        {
            return CreditsToString(billingPlan.DraftCredits());
        }

        public static string DetailedCreditsString(this BillingPlan billingPlan)
        {
            return CreditsToString(billingPlan.DetailedCredits());
        }

        private static string CreditsToString(int credits)
        {
            if (credits == int.MaxValue) { return "unlimited"; }

            return credits.ToString();
        }
    }
}
