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

function escapeHtml(value) {
  return String(value ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

function setStatus(message, kind = 'loading') {
  const element = document.getElementById('apiStatus');
  if (!element) {
    return;
  }

  element.textContent = message;
  element.dataset.kind = kind;
}

function getClubName(value) {
  return value?.name ?? value?.Name ?? value ?? '';
}

function getMatchResultForClub(match, clubName) {
  const homeClub = getClubName(match.homeClub ?? match.HomeClub);
  const awayClub = getClubName(match.awayClub ?? match.AwayClub);
  const homeGoals = match.homeGoals ?? match.HomeGoals;
  const awayGoals = match.awayGoals ?? match.AwayGoals;

  if (homeGoals === null || homeGoals === undefined || awayGoals === null || awayGoals === undefined) {
    return null;
  }

  if (homeClub === clubName) {
    return homeGoals > awayGoals ? 'W' : homeGoals === awayGoals ? 'D' : 'L';
  }

  if (awayClub === clubName) {
    return awayGoals > homeGoals ? 'W' : awayGoals === homeGoals ? 'D' : 'L';
  }

  return null;
}

function getForm(clubName, matches) {
  return matches
    .filter(match => getMatchResultForClub(match, clubName) !== null)
    .sort((first, second) => new Date(second.kickoffUtc ?? second.KickoffUtc) - new Date(first.kickoffUtc ?? first.KickoffUtc))
    .slice(0, 5)
    .map(match => getMatchResultForClub(match, clubName));
}

function getStandingZone(position) {
  if (position <= 4) {
    return 'zone-champions';
  }

  if (position <= 7) {
    return 'zone-europe';
  }

  if (position >= 18) {
    return 'zone-relegation';
  }

  return '';
}

function renderForm(form) {
  if (!form || form.length === 0) {
    return '<span class="form-empty">-</span>';
  }

  return form.map(result => `<span class="form-pill form-${result.toLowerCase()}">${result}</span>`).join('');
}

function renderStandings(rows, matches = []) {
  const body = document.getElementById('standingsBody');
  if (!body) {
    return;
  }

  body.innerHTML = rows.map(row => `
    <tr>
      <td>${row.position}</td>
      <td class="club-cell">${escapeHtml(row.club)}</td>
      <td>${row.played}</td>
      <td>${row.wins}</td>
      <td>${row.draws}</td>
      <td>${row.losses}</td>
      <td>${row.goalsFor}</td>
      <td>${row.goalsAgainst}</td>
      <td>${row.goalDifference}</td>
      <td class="points">${row.points}</td>
      <td class="form-cell ${getStandingZone(row.position)}">${renderForm(getForm(row.club, matches))}</td>
    </tr>`).join('');
}

function renderClubs(clubs) {
  const grid = document.getElementById('clubGrid');
  if (!grid) {
    return;
  }

  if (clubs.length === 0) {
    grid.innerHTML = '<div class="empty-state">No clubs match this search.</div>';
    return;
  }

  grid.innerHTML = clubs.map(club => `
    <article class="club-card">
      <div class="club-code">${escapeHtml(club.shortCode)}</div>
      <h3>${escapeHtml(club.name)}</h3>
      <p>${escapeHtml(club.city)}</p>
      <span>${escapeHtml(club.stadium?.name ?? 'No stadium')}</span>
    </article>`).join('');
}

function renderMatches(matches) {
  const list = document.getElementById('matchesList');
  if (!list) {
    return;
  }

  if (matches.length === 0) {
    list.innerHTML = '<div class="empty-state">No matches found for this filter.</div>';
    return;
  }

  const visibleMatches = list.classList.contains('full-list') ? matches : matches.slice(0, 6);

  list.innerHTML = visibleMatches.map(match => {
    const isUpcoming = match.homeGoals === null || match.awayGoals === null;
    const score = isUpcoming
      ? 'vs'
      : `${match.homeGoals} - ${match.awayGoals}`;
    const date = new Date(match.kickoffUtc).toLocaleDateString(undefined, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });

    return `
      <div class="result-row">
        <span>${escapeHtml(match.homeClub?.name ?? 'Home')}</span>
        <strong>${score}</strong>
        <span>${escapeHtml(match.awayClub?.name ?? 'Away')}</span>
        <small>
          <span class="match-badge ${isUpcoming ? 'match-upcoming' : 'match-played'}">${isUpcoming ? 'Upcoming' : 'Played'}</span>
          ${date}
        </small>
      </div>`;
  }).join('');
}

function getPlayerStats(player) {
  return player.stats ?? player.Stats ?? {};
}

function renderPlayers(players) {
  const body = document.getElementById('playersBody');
  if (!body) {
    return;
  }

  if (players.length === 0) {
    body.innerHTML = '<tr><td colspan="8" class="empty-table">No players match this search.</td></tr>';
    return;
  }

  const maxGoals = Math.max(...players.map(player => Number(getPlayerStats(player).goals ?? getPlayerStats(player).Goals ?? 0)), 1);

  body.innerHTML = players.map((player, index) => {
    const stats = getPlayerStats(player);
    const goals = Number(stats.goals ?? stats.Goals ?? 0);
    const goalPercent = Math.round((goals / maxGoals) * 100);

    return `
      <tr>
        <td>${index + 1}</td>
        <td class="club-cell">${escapeHtml(player.fullName ?? player.FullName)}</td>
        <td>${escapeHtml(player.club ?? player.Club ?? 'No club')}</td>
        <td>${escapeHtml(player.position ?? player.Position)}</td>
        <td>${stats.appearances ?? stats.Appearances ?? 0}</td>
        <td class="points"><span class="stat-bar" style="--stat-width:${goalPercent}%">${goals}</span></td>
        <td>${stats.assists ?? stats.Assists ?? 0}</td>
        <td>${stats.rating ?? stats.Rating ?? 0}</td>
      </tr>`;
  }).join('');
}

function hasElement(id) {
  return document.getElementById(id) !== null;
}

function buildUrl(path, params) {
  const url = new URL(path, window.location.origin);
  Object.entries(params).forEach(([key, value]) => {
    if (value !== '') {
      url.searchParams.set(key, value);
    }
  });
  return url.pathname + url.search;
}

async function loadSummary() {
  const summary = await getJson('/api/league/summary');

  setText('clubsMetric', summary.clubs);
  setText('playersMetric', summary.players);
  setText('matchesMetric', summary.matches);
  setText('leaderMetric', summary.leader);
}

async function loadStandings() {
  const [standings, matches] = await Promise.all([
    getJson('/api/league/standings'),
    getJson('/api/matches')
  ]);
  renderStandings(standings, matches);
}

async function loadClubs() {
  setStatus('Loading clubs');
  const search = document.getElementById('clubSearch')?.value.trim() ?? '';
  const clubs = await getJson(buildUrl('/api/clubs', { search }));
  renderClubs(clubs);
  setStatus('Clubs loaded', 'ok');
}

async function loadMatches() {
  setStatus('Loading matches');
  const played = document.getElementById('matchFilter')?.value ?? '';
  const matches = await getJson(buildUrl('/api/matches', { played }));
  renderMatches(matches);
  setStatus('Matches loaded', 'ok');
}

async function loadPlayers() {
  setStatus('Loading players');
  const players = await getJson('/api/players');
  const search = document.getElementById('playerSearch')?.value.trim().toLowerCase() ?? '';
  const sort = document.getElementById('playerSort')?.value ?? 'goals';

  const filteredPlayers = players.filter(player => {
    const fullName = String(player.fullName ?? player.FullName ?? '').toLowerCase();
    const club = String(player.club ?? player.Club ?? '').toLowerCase();
    const position = String(player.position ?? player.Position ?? '').toLowerCase();
    return fullName.includes(search) || club.includes(search) || position.includes(search);
  });

  filteredPlayers.sort((first, second) => {
    const firstStats = getPlayerStats(first);
    const secondStats = getPlayerStats(second);

    if (sort === 'name') {
      return String(first.fullName ?? first.FullName).localeCompare(String(second.fullName ?? second.FullName));
    }

    const statKey = sort === 'appearances'
      ? 'appearances'
      : sort;
    const firstValue = Number(firstStats[statKey] ?? firstStats[capitalize(statKey)] ?? 0);
    const secondValue = Number(secondStats[statKey] ?? secondStats[capitalize(statKey)] ?? 0);
    return secondValue - firstValue;
  });

  renderPlayers(filteredPlayers);
  updatePlayerMetrics(filteredPlayers);
  setStatus('Players loaded', 'ok');
}

function capitalize(value) {
  return value.charAt(0).toUpperCase() + value.slice(1);
}

function updatePlayerMetrics(players) {
  setText('playerCountMetric', players.length);

  const byGoals = [...players].sort((first, second) => Number(getPlayerStats(second).goals ?? getPlayerStats(second).Goals ?? 0) - Number(getPlayerStats(first).goals ?? getPlayerStats(first).Goals ?? 0))[0];
  const byAssists = [...players].sort((first, second) => Number(getPlayerStats(second).assists ?? getPlayerStats(second).Assists ?? 0) - Number(getPlayerStats(first).assists ?? getPlayerStats(first).Assists ?? 0))[0];
  const byRating = [...players].sort((first, second) => Number(getPlayerStats(second).rating ?? getPlayerStats(second).Rating ?? 0) - Number(getPlayerStats(first).rating ?? getPlayerStats(first).Rating ?? 0))[0];

  setText('topScorerMetric', byGoals ? `${byGoals.fullName ?? byGoals.FullName} (${getPlayerStats(byGoals).goals ?? getPlayerStats(byGoals).Goals})` : 'No data');
  setText('topAssistsMetric', byAssists ? `${byAssists.fullName ?? byAssists.FullName} (${getPlayerStats(byAssists).assists ?? getPlayerStats(byAssists).Assists})` : 'No data');
  setText('topRatingMetric', byRating ? `${byRating.fullName ?? byRating.FullName} (${getPlayerStats(byRating).rating ?? getPlayerStats(byRating).Rating})` : 'No data');
}

async function loadDashboard() {
  setStatus('Loading API data');
  await Promise.all([
    hasElement('clubsMetric') ? loadSummary() : Promise.resolve(),
    hasElement('standingsBody') ? loadStandings() : Promise.resolve(),
    hasElement('clubGrid') ? loadClubs() : Promise.resolve(),
    hasElement('matchesList') ? loadMatches() : Promise.resolve(),
    hasElement('playersBody') ? loadPlayers() : Promise.resolve()
  ]);
  setStatus('API data loaded', 'ok');
}

let searchTimer;

document.getElementById('clubSearch')?.addEventListener('input', () => {
  window.clearTimeout(searchTimer);
  searchTimer = window.setTimeout(() => {
    loadClubs().catch(showApiError);
  }, 250);
});

document.getElementById('matchFilter')?.addEventListener('change', () => {
  loadMatches().catch(showApiError);
});

document.getElementById('playerSearch')?.addEventListener('input', () => {
  window.clearTimeout(searchTimer);
  searchTimer = window.setTimeout(() => {
    loadPlayers().catch(showApiError);
  }, 250);
});

document.getElementById('playerSort')?.addEventListener('change', () => {
  loadPlayers().catch(showApiError);
});

function showApiError(error) {
  console.error(error);
  setText('leaderMetric', 'API unavailable');
  setStatus('API unavailable. Check SQL Server LocalDB and migrations.', 'error');
}

loadDashboard().catch(showApiError);
