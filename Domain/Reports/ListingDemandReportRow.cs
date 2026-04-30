namespace rent_for_students.Domain.Reports
{
    // PROMPT v1.7: Read model for sp_GenerateListingDemandReport cursor result
    public class ListingDemandReportRow
    {
        public Guid ListingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public decimal PricePerMonth { get; set; }
        public int TotalApplications { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public DateTime? LastApplicationDate { get; set; }
        public string DemandLevel { get; set; } = string.Empty;
    }
}
