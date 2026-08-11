import { requireAuth } from '../auth/authGuard.js';
import { credentialApi } from '../api/credentialApi.js';

// Ensure authentication
requireAuth();

// DOM references
const list = document.getElementById('list');
const form = document.querySelector('form');

// Load credentials and render them
async function load() {
    const rows = await credentialApi.all();
    list.innerHTML = rows
        .map(
            (x) => `
        <div class="card">
          <b>${x.title}</b>
          <div>${x.username}</div>
          <div>${x.password}</div>
        </div>
      `
        )
        .join('');
}

// Handle form submission
form.onsubmit = async (e) => {
    e.preventDefault();

    await credentialApi.create({
        title: form.title.value,
        username: form.username.value,
        password: form.password.value,
        website: form.website.value,
        notes: form.notes.value,
    });

    form.reset();
    load();
};

// Initial load
load();
