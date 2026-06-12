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

function field(item, camelName) {
  if (!item) {
    return undefined;
  }

  const pascalName = capitalize(camelName);
  return item[camelName] ?? item[pascalName];
}

function itemId(item) {
  return field(item, 'id');
}

function setStatus(message, kind = 'loading') {
  const element = document.getElementById('apiStatus');
  if (!element) {
    return;
  }

  element.textContent = message;
  element.dataset.kind = kind;
}

function setAuthStatus(message, kind = 'loading') {
  const element = document.getElementById('authStatus');
  if (!element) {
    return;
  }

  element.textContent = message;
  element.dataset.kind = kind;
}

async function postJson(url, data) {
  return sendJson(url, 'POST', data);
}

async function sendJson(url, method, data) {
  const options = {
    method,
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    }
  };

  if (data !== undefined) {
    options.body = JSON.stringify(data);
  }

  const response = await fetch(url, options);

  if (!response.ok) {
    let message = `Request failed with status ${response.status}.`;
    const text = await response.text();

    try {
      const error = text ? JSON.parse(text) : null;
      message = Array.isArray(error) ? error.join(' ') : error?.message ?? JSON.stringify(error);
    } catch {
      message = text || message;
    }

    throw new Error(message);
  }

  const text = await response.text();
  return text ? JSON.parse(text) : {};
}

function setAdminStatus(id, message, kind = 'loading') {
  const element = document.getElementById(id);
  if (!element) {
    return;
  }

  element.textContent = message;
  element.dataset.kind = kind;
}

function getPayload(form, numberFields = []) {
  const payload = Object.fromEntries(new FormData(form).entries());
  numberFields.forEach(fieldName => {
    payload[fieldName] = Number(payload[fieldName]);
  });
  return payload;
}

function fillSelect(id, items, getLabel, selectedValue) {
  const select = document.getElementById(id);
  if (!select) {
    return;
  }

  select.innerHTML = items.map(item => {
    const value = itemId(item);
    const selected = String(value) === String(selectedValue) ? ' selected' : '';
    return `<option value="${value}"${selected}>${escapeHtml(getLabel(item))}</option>`;
  }).join('');
}

function toLocalInputValue(value) {
  if (!value) {
    return '';
  }

  const date = new Date(value);
  date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
  return date.toISOString().slice(0, 16);
}

function toUtcIsoFromLocalInput(value) {
  return new Date(value).toISOString();
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

function renderClubs(clubs, standings = []) {
  const grid = document.getElementById('clubGrid');
  if (!grid) {
    return;
  }

  if (clubs.length === 0) {
    grid.innerHTML = '<div class="empty-state">No clubs match this search.</div>';
    return;
  }

  window.leagueState = window.leagueState || {};
  window.leagueState.clubs = clubs;

  const standingsByClub = new Map(standings.map(row => [field(row, 'club'), row]));

  grid.innerHTML = clubs.map(club => `
    <article class="club-card">
      <div class="club-card-top">
        <div class="club-code">${escapeHtml(field(club, 'shortCode'))}</div>
        ${renderClubRank(standingsByClub.get(field(club, 'name')))}
      </div>
      <h3>${escapeHtml(field(club, 'name'))}</h3>
      <p>${escapeHtml(field(club, 'city'))}</p>
      <span>${escapeHtml(field(field(club, 'stadium'), 'name') ?? 'No stadium')}</span>
      <div class="admin-actions">
        <button type="button" data-edit-club="${itemId(club)}">Edit</button>
        <button type="button" class="danger-button" data-delete-club="${itemId(club)}">Delete</button>
      </div>
    </article>`).join('');
}

function renderClubRank(standing) {
  if (!standing) {
    return '<span class="club-rank">No table data</span>';
  }

  return `<span class="club-rank">#${standing.position} · ${standing.points} pts</span>`;
}

function renderMatches(matches) {
  const list = document.getElementById('matchesList');
  if (!list) {
    return;
  }

  window.leagueState = window.leagueState || {};
  window.leagueState.matches = matches;

  if (matches.length === 0) {
    list.innerHTML = '<div class="empty-state">No matches found for this filter.</div>';
    return;
  }

  const visibleMatches = list.classList.contains('full-list') ? matches : matches.slice(0, 6);

  list.innerHTML = visibleMatches.map(match => {
    const homeGoals = field(match, 'homeGoals');
    const awayGoals = field(match, 'awayGoals');
    const homeClub = field(match, 'homeClub');
    const awayClub = field(match, 'awayClub');
    const isUpcoming = homeGoals === null || homeGoals === undefined || awayGoals === null || awayGoals === undefined;
    const score = isUpcoming
      ? 'vs'
      : `${homeGoals} - ${awayGoals}`;
    const date = new Date(field(match, 'kickoffUtc')).toLocaleDateString(undefined, {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });

    return `
      <div class="result-row">
        <span>${escapeHtml(field(homeClub, 'name') ?? 'Home')}</span>
        <strong>${score}</strong>
        <span>${escapeHtml(field(awayClub, 'name') ?? 'Away')}</span>
        <small>
          <span class="match-badge ${isUpcoming ? 'match-upcoming' : 'match-played'}">${isUpcoming ? 'Upcoming' : 'Played'}</span>
          ${date}
        </small>
        <div class="admin-actions result-actions">
          <button type="button" data-edit-match="${itemId(match)}">Edit</button>
          <button type="button" data-result-match="${itemId(match)}">Result</button>
          <button type="button" class="danger-button" data-delete-match="${itemId(match)}">Delete</button>
        </div>
      </div>`;
  }).join('');

  fillResultMatchSelect(matches);
}

function getPlayerStats(player) {
  return player.stats ?? player.Stats ?? {};
}

function renderPlayers(players) {
  const body = document.getElementById('playersBody');
  if (!body) {
    return;
  }

  window.leagueState = window.leagueState || {};
  window.leagueState.players = players;

  if (players.length === 0) {
    body.innerHTML = '<tr><td colspan="9" class="empty-table">No players match this search.</td></tr>';
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
        <td class="table-actions">
          <button type="button" data-edit-player="${itemId(player)}">Edit</button>
          <button type="button" class="danger-button" data-delete-player="${itemId(player)}">Delete</button>
        </td>
      </tr>`;
  }).join('');
}

function renderStadiums(stadiums) {
  const body = document.getElementById('stadiumsBody');
  if (!body) {
    return;
  }

  window.leagueState = window.leagueState || {};
  window.leagueState.stadiums = stadiums;

  if (stadiums.length === 0) {
    body.innerHTML = '<tr><td colspan="5" class="empty-table">No stadiums found.</td></tr>';
    return;
  }

  body.innerHTML = stadiums.map((stadium, index) => `
    <tr>
      <td>${index + 1}</td>
      <td class="club-cell">${escapeHtml(field(stadium, 'name'))}</td>
      <td>${escapeHtml(field(stadium, 'city'))}</td>
      <td>${Number(field(stadium, 'capacity')).toLocaleString()}</td>
      <td class="table-actions">
        <button type="button" data-edit-stadium="${itemId(stadium)}">Edit</button>
        <button type="button" class="danger-button" data-delete-stadium="${itemId(stadium)}">Delete</button>
      </td>
    </tr>`).join('');
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
  const [clubs, standings] = await Promise.all([
    getJson(buildUrl('/api/clubs', { search })),
    getJson('/api/league/standings')
  ]);
  renderClubs(clubs, standings);
  await loadStadiumOptions();
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
  await loadClubOptions();
  setStatus('Players loaded', 'ok');
}

async function loadStadiums() {
  setStatus('Loading stadiums');
  const stadiums = await getJson('/api/stadiums');
  window.leagueState = window.leagueState || {};
  window.leagueState.stadiums = stadiums;
  renderStadiums(stadiums);
  setStatus('Stadiums loaded', 'ok');
}

async function loadClubOptions() {
  if (!hasElement('playerClubId') && !hasElement('matchHomeClubId') && !hasElement('matchAwayClubId')) {
    return;
  }

  const clubs = window.leagueState?.clubs ?? await getJson('/api/clubs');
  window.leagueState = window.leagueState || {};
  window.leagueState.clubs = clubs;
  fillSelect('playerClubId', clubs, club => field(club, 'name'));
  fillSelect('matchHomeClubId', clubs, club => field(club, 'name'));
  fillSelect('matchAwayClubId', clubs, club => field(club, 'name'));
}

async function loadStadiumOptions() {
  if (!hasElement('clubStadiumId')) {
    return;
  }

  const stadiums = window.leagueState?.stadiums ?? await getJson('/api/stadiums');
  window.leagueState = window.leagueState || {};
  window.leagueState.stadiums = stadiums;
  fillSelect('clubStadiumId', stadiums, stadium => `${field(stadium, 'name')} - ${field(stadium, 'city')}`);
}

function fillResultMatchSelect(matches = window.leagueState?.matches ?? []) {
  fillSelect('resultMatchId', matches, match => {
    const home = field(field(match, 'homeClub'), 'name') ?? 'Home';
    const away = field(field(match, 'awayClub'), 'name') ?? 'Away';
    return `${home} vs ${away}`;
  });
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
    hasElement('playersBody') ? loadPlayers() : Promise.resolve(),
    hasElement('stadiumsBody') ? loadStadiums() : Promise.resolve(),
    hasElement('matchHomeClubId') ? loadClubOptions() : Promise.resolve()
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

function resetClubForm() {
  document.getElementById('clubForm')?.reset();
  setText('clubFormTitle', 'Add Club');
  const id = document.getElementById('clubId');
  if (id) {
    id.value = '';
  }
}

function resetPlayerForm() {
  document.getElementById('playerForm')?.reset();
  setText('playerFormTitle', 'Add Player');
  const id = document.getElementById('playerId');
  if (id) {
    id.value = '';
  }
}

function resetStadiumForm() {
  document.getElementById('stadiumForm')?.reset();
  setText('stadiumFormTitle', 'Add Stadium');
  const id = document.getElementById('stadiumId');
  if (id) {
    id.value = '';
  }
}

function resetMatchForm() {
  document.getElementById('matchForm')?.reset();
  setText('matchFormTitle', 'Add Match');
  const id = document.getElementById('matchId');
  if (id) {
    id.value = '';
  }
}

function editClub(id) {
  const club = window.leagueState?.clubs?.find(item => String(itemId(item)) === String(id));
  if (!club) {
    return;
  }

  setText('clubFormTitle', 'Edit Club');
  document.getElementById('clubId').value = itemId(club);
  document.getElementById('clubName').value = field(club, 'name') ?? '';
  document.getElementById('clubShortCode').value = field(club, 'shortCode') ?? '';
  document.getElementById('clubCity').value = field(club, 'city') ?? '';
  document.getElementById('clubFoundedYear').value = field(club, 'foundedYear') ?? '';
  document.getElementById('clubStadiumId').value = field(club, 'stadiumId') ?? itemId(field(club, 'stadium')) ?? '';
  document.getElementById('clubForm')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function editPlayer(id) {
  const player = window.leagueState?.players?.find(item => String(itemId(item)) === String(id));
  if (!player) {
    return;
  }

  setText('playerFormTitle', 'Edit Player');
  document.getElementById('playerId').value = itemId(player);
  document.getElementById('playerFullName').value = field(player, 'fullName') ?? '';
  document.getElementById('playerPosition').value = field(player, 'position') ?? '';
  document.getElementById('playerShirtNumber').value = field(player, 'shirtNumber') ?? '';
  document.getElementById('playerClubId').value = field(player, 'clubId') ?? '';
  document.getElementById('playerForm')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function editStadium(id) {
  const stadium = window.leagueState?.stadiums?.find(item => String(itemId(item)) === String(id));
  if (!stadium) {
    return;
  }

  setText('stadiumFormTitle', 'Edit Stadium');
  document.getElementById('stadiumId').value = itemId(stadium);
  document.getElementById('stadiumName').value = field(stadium, 'name') ?? '';
  document.getElementById('stadiumCity').value = field(stadium, 'city') ?? '';
  document.getElementById('stadiumCapacity').value = field(stadium, 'capacity') ?? '';
  document.getElementById('stadiumForm')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function editMatch(id) {
  const match = window.leagueState?.matches?.find(item => String(itemId(item)) === String(id));
  if (!match) {
    return;
  }

  setText('matchFormTitle', 'Edit Match');
  document.getElementById('matchId').value = itemId(match);
  document.getElementById('matchHomeClubId').value = field(match, 'homeClubId') ?? itemId(field(match, 'homeClub')) ?? '';
  document.getElementById('matchAwayClubId').value = field(match, 'awayClubId') ?? itemId(field(match, 'awayClub')) ?? '';
  document.getElementById('matchKickoffUtc').value = toLocalInputValue(field(match, 'kickoffUtc'));
  document.getElementById('matchForm')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

function fillResultForm(id) {
  const match = window.leagueState?.matches?.find(item => String(itemId(item)) === String(id));
  if (!match) {
    return;
  }

  document.getElementById('resultMatchId').value = itemId(match);
  document.getElementById('resultHomeGoals').value = field(match, 'homeGoals') ?? '';
  document.getElementById('resultAwayGoals').value = field(match, 'awayGoals') ?? '';
  document.getElementById('resultForm')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

async function deleteResource(url, statusId, reload, label) {
  if (!confirm(`Delete this ${label}?`)) {
    return;
  }

  setAdminStatus(statusId, `Deleting ${label}`);

  try {
    await sendJson(url, 'DELETE');
    setAdminStatus(statusId, `${capitalize(label)} deleted`, 'ok');
    await reload();
  } catch (error) {
    console.error(error);
    setAdminStatus(statusId, error.message || `Could not delete ${label}`, 'error');
  }
}

document.addEventListener('click', event => {
  const target = event.target.closest('button');
  if (!target) {
    return;
  }

  if (target.dataset.editClub) {
    editClub(target.dataset.editClub);
  } else if (target.dataset.deleteClub) {
    deleteResource(`/api/clubs/${target.dataset.deleteClub}`, 'clubAdminStatus', loadClubs, 'club');
  } else if (target.dataset.editPlayer) {
    editPlayer(target.dataset.editPlayer);
  } else if (target.dataset.deletePlayer) {
    deleteResource(`/api/players/${target.dataset.deletePlayer}`, 'playerAdminStatus', loadPlayers, 'player');
  } else if (target.dataset.editStadium) {
    editStadium(target.dataset.editStadium);
  } else if (target.dataset.deleteStadium) {
    deleteResource(`/api/stadiums/${target.dataset.deleteStadium}`, 'stadiumAdminStatus', loadStadiums, 'stadium');
  } else if (target.dataset.editMatch) {
    editMatch(target.dataset.editMatch);
  } else if (target.dataset.resultMatch) {
    fillResultForm(target.dataset.resultMatch);
  } else if (target.dataset.deleteMatch) {
    deleteResource(`/api/matches/${target.dataset.deleteMatch}`, 'matchAdminStatus', loadMatches, 'match');
  }
});

document.getElementById('clubResetButton')?.addEventListener('click', resetClubForm);
document.getElementById('playerResetButton')?.addEventListener('click', resetPlayerForm);
document.getElementById('stadiumResetButton')?.addEventListener('click', resetStadiumForm);
document.getElementById('matchResetButton')?.addEventListener('click', resetMatchForm);

document.getElementById('clubForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  const id = document.getElementById('clubId').value;
  const payload = getPayload(event.currentTarget, ['foundedYear', 'stadiumId']);
  const method = id ? 'PUT' : 'POST';
  const url = id ? `/api/clubs/${id}` : '/api/clubs';
  setAdminStatus('clubAdminStatus', id ? 'Updating club' : 'Creating club');

  try {
    await sendJson(url, method, payload);
    setAdminStatus('clubAdminStatus', id ? 'Club updated' : 'Club created', 'ok');
    resetClubForm();
    await loadClubs();
  } catch (error) {
    console.error(error);
    setAdminStatus('clubAdminStatus', error.message || 'Could not save club', 'error');
  }
});

document.getElementById('playerForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  const id = document.getElementById('playerId').value;
  const payload = getPayload(event.currentTarget, ['shirtNumber', 'clubId']);
  const method = id ? 'PUT' : 'POST';
  const url = id ? `/api/players/${id}` : '/api/players';
  setAdminStatus('playerAdminStatus', id ? 'Updating player' : 'Creating player');

  try {
    await sendJson(url, method, payload);
    setAdminStatus('playerAdminStatus', id ? 'Player updated' : 'Player created', 'ok');
    resetPlayerForm();
    await loadPlayers();
  } catch (error) {
    console.error(error);
    setAdminStatus('playerAdminStatus', error.message || 'Could not save player', 'error');
  }
});

document.getElementById('stadiumForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  const id = document.getElementById('stadiumId').value;
  const payload = getPayload(event.currentTarget, ['capacity']);
  const method = id ? 'PUT' : 'POST';
  const url = id ? `/api/stadiums/${id}` : '/api/stadiums';
  setAdminStatus('stadiumAdminStatus', id ? 'Updating stadium' : 'Creating stadium');

  try {
    await sendJson(url, method, payload);
    setAdminStatus('stadiumAdminStatus', id ? 'Stadium updated' : 'Stadium created', 'ok');
    resetStadiumForm();
    await loadStadiums();
  } catch (error) {
    console.error(error);
    setAdminStatus('stadiumAdminStatus', error.message || 'Could not save stadium', 'error');
  }
});

document.getElementById('matchForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  const id = document.getElementById('matchId').value;
  const payload = getPayload(event.currentTarget, ['homeClubId', 'awayClubId']);
  payload.kickoffUtc = toUtcIsoFromLocalInput(payload.kickoffUtc);
  const method = id ? 'PUT' : 'POST';
  const url = id ? `/api/matches/${id}` : '/api/matches';
  setAdminStatus('matchAdminStatus', id ? 'Updating match' : 'Creating match');

  try {
    await sendJson(url, method, payload);
    setAdminStatus('matchAdminStatus', id ? 'Match updated' : 'Match created', 'ok');
    resetMatchForm();
    await loadMatches();
  } catch (error) {
    console.error(error);
    setAdminStatus('matchAdminStatus', error.message || 'Could not save match', 'error');
  }
});

document.getElementById('resultForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  const payload = getPayload(event.currentTarget, ['matchId', 'homeGoals', 'awayGoals']);
  const matchId = payload.matchId;
  delete payload.matchId;
  setAdminStatus('resultAdminStatus', 'Saving result');

  try {
    await sendJson(`/api/matches/${matchId}/result`, 'PATCH', payload);
    setAdminStatus('resultAdminStatus', 'Result saved', 'ok');
    event.currentTarget.reset();
    await loadMatches();
    if (hasElement('standingsBody')) {
      await loadStandings();
    }
  } catch (error) {
    console.error(error);
    setAdminStatus('resultAdminStatus', error.message || 'Could not save result', 'error');
  }
});

function getFormPayload(form) {
  return Object.fromEntries(new FormData(form).entries());
}

document.getElementById('loginForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  setAuthStatus('Signing in');

  try {
    await postJson('/api/account/login', getFormPayload(event.currentTarget));
    setAuthStatus('Logged in', 'ok');
    window.location.href = '/Account';
  } catch (error) {
    console.error(error);
    setAuthStatus(error.message || 'Login failed', 'error');
  }
});

document.getElementById('registerForm')?.addEventListener('submit', async event => {
  event.preventDefault();
  setAuthStatus('Creating account');

  try {
    await postJson('/api/account/register', getFormPayload(event.currentTarget));
    setAuthStatus('Account created. You can log in now.', 'ok');
    window.setTimeout(() => {
      window.location.href = '/Login';
    }, 900);
  } catch (error) {
    console.error(error);
    setAuthStatus(error.message || 'Registration failed', 'error');
  }
});

async function loadProfile() {
  if (!hasElement('profileEmail')) {
    return;
  }

  try {
    const profile = await getJson('/api/account/profile');
    setText('profileEmail', profile.email);
    setText('profileFullName', profile.fullName);
    setText('profileFavoriteClub', profile.favoriteClub || 'Not selected');
    setAuthStatus('Profile loaded', 'ok');
  } catch (error) {
    console.error(error);
    setText('profileEmail', 'Not signed in');
    setText('profileFullName', 'Not signed in');
    setText('profileFavoriteClub', 'Not signed in');
    setAuthStatus('Please log in first', 'error');
  }
}

document.getElementById('logoutButton')?.addEventListener('click', async () => {
  setAuthStatus('Signing out');

  try {
    await postJson('/api/account/logout', {});
    setAuthStatus('Logged out', 'ok');
    window.location.href = '/Login';
  } catch (error) {
    console.error(error);
    setAuthStatus(error.message || 'Logout failed', 'error');
  }
});

loadDashboard().catch(showApiError);
loadProfile().catch(showApiError);
