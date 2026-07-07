using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// The +3 bonus: one end-to-end scenario of 10 chained steps that tells a full story —
// authenticate, build a whole league→clubs→player→match→user graph, rate the player, then
// read / update / delete the rating and prove it's gone. Each step builds on the ids from
// the previous ones. Everything created here is cleaned up at the end.
[Collection("api")]
public class EndToEndScenarioTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public EndToEndScenarioTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task FullRatingLifecycle_TenSteps()
    {
        var tag = ApiFixture.Tag();
        int leagueId = 0, homeId = 0, awayId = 0, playerId = 0, matchId = 0, userId = 0;

        try
        {
            // ── Step 1: authenticate — the fixture logged in and holds a JWT. ──
            Assert.NotEmpty(_fixture.Token);

            // ── Step 2: create a league. ──
            var league = await Api.PostAsync("/api/leagues", new() { DataObject = new { name = $"E2E League {tag}" } });
            Assert.Equal(201, league.Status);
            leagueId = await league.IdAsync();

            // ── Step 3: create the home club in that league. ──
            var home = await Api.PostAsync("/api/clubs", new() { DataObject = new { name = $"E2E Home {tag}", foundedDate = "1990-01-01", leagueId } });
            Assert.Equal(201, home.Status);
            homeId = await home.IdAsync();

            // ── Step 4: create the away club in that league. ──
            var away = await Api.PostAsync("/api/clubs", new() { DataObject = new { name = $"E2E Away {tag}", foundedDate = "1991-01-01", leagueId } });
            Assert.Equal(201, away.Status);
            awayId = await away.IdAsync();

            // ── Step 5: add a player to the home club. ──
            var player = await Api.PostAsync("/api/players", new()
            {
                DataObject = new { firstName = "E2E", lastName = $"Player{tag}", dateOfBirth = "1998-05-03", position = 3, nationality = "Testland", clubId = homeId }
            });
            Assert.Equal(201, player.Status);
            playerId = await player.IdAsync();

            // ── Step 6: create a match between the two clubs. ──
            var match = await Api.PostAsync("/api/matches", new()
            {
                DataObject = new { leagueId, homeTeamId = homeId, awayTeamId = awayId, date = "2026-05-10T18:00:00", homeGoals = 2, awayGoals = 1 }
            });
            Assert.Equal(201, match.Status);
            matchId = await match.IdAsync();

            // ── Step 7: create a rating author (user). ──
            var user = await Api.PostAsync("/api/users", new() { DataObject = new { firstName = "E2E", lastName = $"Author{tag}", email = $"e2e{tag}@example.com" } });
            Assert.Equal(201, user.Status);
            userId = await user.IdAsync();

            // ── Step 8: rate the player in that match, then read it back. ──
            var rating = await Api.PostAsync("/api/ratings", new() { DataObject = new { playerId, matchId, userId, score = 8, comment = "Great game" } });
            Assert.Equal(201, rating.Status);
            int ratingId = await rating.IdAsync();

            var read = await Api.GetAsync($"/api/ratings/{ratingId}");
            Assert.Equal(200, read.Status);
            Assert.Equal(8, (await read.JsonElementAsync()).GetProperty("score").GetInt32());

            // ── Step 9: update the rating's score and confirm the change persisted. ──
            var update = await Api.PutAsync($"/api/ratings/{ratingId}", new() { DataObject = new { playerId, matchId, userId, score = 10, comment = "Upgraded to a 10" } });
            Assert.Equal(200, update.Status);
            Assert.Equal(10, (await update.JsonElementAsync()).GetProperty("score").GetInt32());

            // ── Step 10: delete the rating, then prove it's gone (404). ──
            var delete = await Api.DeleteAsync($"/api/ratings/{ratingId}");
            Assert.Equal(204, delete.Status);

            var gone = await Api.GetAsync($"/api/ratings/{ratingId}");
            Assert.Equal(404, gone.Status);
        }
        finally
        {
            // Tear the graph down in reverse dependency order (best effort).
            if (matchId  != 0) await _fixture.TryDeleteAsync($"/api/matches/{matchId}");
            if (playerId != 0) await _fixture.TryDeleteAsync($"/api/players/{playerId}");
            if (userId   != 0) await _fixture.TryDeleteAsync($"/api/users/{userId}");
            if (homeId   != 0) await _fixture.TryDeleteAsync($"/api/clubs/{homeId}");
            if (awayId   != 0) await _fixture.TryDeleteAsync($"/api/clubs/{awayId}");
            if (leagueId != 0) await _fixture.TryDeleteAsync($"/api/leagues/{leagueId}");
        }
    }
}
