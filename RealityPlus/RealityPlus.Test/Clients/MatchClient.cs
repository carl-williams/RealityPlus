namespace RealityPlus.Test.Clients
{
    internal class MatchClient: BaseClient
    {
        protected override string BaseUrl => "http://localhost:9999";

        public async Task<IEnumerable<string>> Match(Guid sessionId, bool ranked)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("SessionId", sessionId.ToString());

            var url = ranked ? "rank" : "quick";
            var response = await Get<IEnumerable<string>>(url, headers);
            if (response == null)
            {
                return Enumerable.Empty<string>();
            }
            return response;
        }
    }
}
