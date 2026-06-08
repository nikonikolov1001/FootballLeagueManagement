async function getJson(url) {
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Request failed: ${url}`);
  }

  return response.json();
}

function setText(id, value) {
  const element = document.getElementById(id);
  if (element) {
    element.textContent = value;
  }
}

function renderStandings(rows) {
  const body = document.getElementById('standingsBody');
  if (!body) {
    return;
  }

  body.innerHTML = rows.map(row => `
    <tr>
      <td>${row.position}</td>
      <td class="club-cell">${row.club}</td>
      <td>${row.played}</td>
      <td>${row.wins}</td>
      <td>${row.draws}</td>
      <td>${row.losses}</td>
      <td>${row.goalsFor}</td>
      <td>${row.goalsAgainst}</td>
      <td>${row.goalDifference}</td>
      <td class="points">${row.points}</td>
    </tr>`).join('');
}

function renderClubs(clubs) {
  const grid = document.getElementById('clubGrid');
  if (!grid) {
    return;
  }

  grid.innerHTML = clubs.map(club => `
    <article class="club-card">
      <div class="club-code">${club.shortCode}</div>
      <h3>${club.name}</h3>
      <p>${club.city}</p>
      <span>${club.stadium?.name ?? 'No stadium'}</span>
    </article>`).join('');
}

function renderMatches(matches) {
  const list = document.getElementById('matchesList');
  if (!list) {
    return;
  }

  list.innerHTML = matches.slice(0, 6).map(match => {
    const score = match.homeGoals === null || match.awayGoals === null
      ? 'vs'
      : `${match.homeGoals} - ${match.awayGoals}`;

    return `
      <div class="result-row">
        <span>${match.homeClub?.name ?? 'Home'}</span>
        <strong>${score}</strong>
        <span>${match.awayClub?.name ?? 'Away'}</span>
      </div>`;
  }).join('');
}

async function loadDashboard() {
  const [summary, standings, clubs, matches] = await Promise.all([
    getJson('/api/league/summary'),
    getJson('/api/league/standings'),
    getJson('/api/clubs'),
    getJson('/api/matches')
  ]);

  setText('clubsMetric', summary.clubs);
  setText('playersMetric', summary.players);
  setText('matchesMetric', summary.matches);
  setText('leaderMetric', summary.leader);
  renderStandings(standings);
  renderClubs(clubs);
  renderMatches(matches);
}

loadDashboard().catch(error => {
  console.error(error);
  setText('leaderMetric', 'API unavailable');
});
