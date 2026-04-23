namespace RPS.Challenge.Web.ViewModels {
    public class TournamentFileViewModel {
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
        public string Description { get; set; }
        public string DescriptionCssClass { get; set; }
    }

    public class TournamentFileDescriptionViewModel {
        public string Message { get; set; }
        public string CssClass { get; set; }
    }
}

