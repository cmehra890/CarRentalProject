namespace CarRentalProject.Models
{
    public static class BookingStatus
    {
        private static string booked = "Booked";
        private static string notBooked = "NotBooked";
        private static string cancelled = "Cancelled";
        private static string upComming = "UpComing";
        private static string completed = "Completed";

        public static string Booked { get => booked; set => booked = value; }
        public static string NotBooked { get => notBooked; set => notBooked = value; }
        public static string Cancelled { get => cancelled; set => cancelled = value; }
        public static string UpComming { get => upComming; set => upComming = value; }
        public static string Completed { get => completed; set => completed = value; }
    }

    public static class PayementStatus
    {
        private static string paid = "Paid";
        private static string notPaid = "NotPaid";
        private static string partiallyPaid = "PartiallyPaid";
        private static string cancelled = "Cancelled";

        public static string Paid { get => paid; set => paid = value; }
        public static string NotPaid { get => notPaid; set => notPaid = value; }
        public static string PartiallyPaid { get => partiallyPaid; set => partiallyPaid = value; }
        public static string Cancelled { get => cancelled; set => cancelled = value; }
    }

    public static class PaymentType
    {
        private static string cash = "Cash";
        private static string card = "Card";
        private static string upi = "Upi";
        private static string netBanking = "NetBanking";
        private static string thirdPartyWallet = "ThirdPartyWallet";

        public static string Cash { get => cash; set => cash = value; }
        public static string Card { get => card; set => card = value; }
        public static string Upi { get => upi; set => upi = value; }
        public static string NetBanking { get => netBanking; set => netBanking = value; }
        public static string ThirdPartyWallet { get => thirdPartyWallet; set => thirdPartyWallet = value; }
    }

    public static class CarStatus
    {
        private static string available = "Available";
        private static string rented = "Rented";
        private static string underMaintainance = "UnderMaintainance";
        private static string removed = "Removed";

        public static string Available { get => available; set => available = value; }
        public static string Rented { get => rented; set => rented = value; }
        public static string UnderMaintainance { get => underMaintainance; set => underMaintainance = value; }
        public static string Removed { get => removed; set => removed = value; }
    }

}
