namespace LiveScores.Domain.Entities
{
    public record Match(string HomeTeam, string AwayTeam)
    {
        public Guid Id { get; init; } = new Guid();

        public byte HomeTeamScore { get; private set; } = 0;

        public byte AwayTeamScore { get; private set; } = 0;

        public void UpdateScore(byte homeTeamScore, byte awayTeamScore)
        {
            HomeTeamScore = homeTeamScore;
            AwayTeamScore = awayTeamScore;
        }

        public virtual bool Equals(Match? other)
        {
            if (other is null)
            {
                return false;
            }

            return Id.Equals(other.Id)
                   && HomeTeam.Equals(other.HomeTeam)
                   && AwayTeam.Equals(other.AwayTeam)
                   && HomeTeamScore.Equals(other.HomeTeamScore)
                   && AwayTeamScore.Equals(other.AwayTeamScore);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, AwayTeam, HomeTeam) ;
        }
    }
}
