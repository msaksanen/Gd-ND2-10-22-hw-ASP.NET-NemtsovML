namespace MedContactWebApi.Models.Responses
{
    /// <summary>
    /// Model for returning text response from api
    /// </summary>
    public class TextResponse
    {
        /// <summary>
        /// string? Response Message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// bool? Response isMatched
        /// to requirements
        /// </summary>
        public bool? isMatched { get; set; }

        /// <summary>
        /// int? Response MatchResult
        /// to requirements
        /// </summary>
        public int? MatchResult { get; set; }
    }
}
