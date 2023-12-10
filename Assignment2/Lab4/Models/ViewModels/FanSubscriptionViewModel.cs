namespace Lab4.Models.ViewModels
{
    public class FanSubscriptionViewModel
    {
        public Fan Fan { get; set; }
        public IEnumerable<SportClubSubscriptionViewModel> Subscriptions { get; set; }
        public string SelectedFanId { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

}