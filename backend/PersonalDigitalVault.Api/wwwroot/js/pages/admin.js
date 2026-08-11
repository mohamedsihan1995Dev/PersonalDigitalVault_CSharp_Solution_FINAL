import {
    requireAuth
} from '../auth/authGuard.js';

import {
    adminApi
} from '../api/adminApi.js';


// =========================================================
// AUTH CHECK
// =========================================================
//
// Admin page open pannumbodhu JWT token irukka check pannum.
//
// Token illaina:
// Login page-ku redirect aagum.
//
requireAuth();


// =========================================================
// HTML ELEMENTS
// =========================================================

// Dashboard statistics display panna area
const statsContainer =
    document.getElementById('stats');


// Registered users table body
const usersContainer =
    document.getElementById('users');


// Upload activity table body
const uploadsContainer =
    document.getElementById('uploads');


// Success / Error message box
const messageBox =
    document.getElementById('message');


// Registered users search box
const userSearch =
    document.getElementById('userSearch');


// =========================================================
// ALL USERS TEMPORARY STORAGE
// =========================================================
//
// Backend-lendhu varra full registered users list
// inga temporary-a store pannuvom.
//
// Search pannumbodhu API repeatedly call panna thevai illa.
//
let allUsers = [];


// =========================================================
// ESCAPE HTML
// =========================================================
//
// FUNCTION:
// Database-lendhu varra text safe-a HTML-la display pannum.
//
// INPUT:
// value -> Name / Email / File Name
//
// REASON:
// XSS attack avoid panna.
//
// OUTPUT:
// Safe HTML string.
//
function escapeHtml(value) {

    return String(value ?? '')
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#039;');
}


// =========================================================
// FILE SIZE FORMAT
// =========================================================
//
// FUNCTION:
// Database-la bytes-la irukkura file size-a
// readable format-ku convert pannum.
//
// EXAMPLE:
// 1024       -> 1 KB
// 1048576    -> 1 MB
//
// INPUT:
// bytes
//
// OUTPUT:
// B / KB / MB / GB
//
function formatFileSize(bytes) {

    const value =
        Number(bytes ?? 0);


    if (value <= 0) {

        return '0 B';
    }


    const units =
        [
            'B',
            'KB',
            'MB',
            'GB'
        ];


    const index =
        Math.min(
            Math.floor(
                Math.log(value) /
                Math.log(1024)
            ),
            units.length - 1
        );


    const size =
        value /
        Math.pow(1024, index);


    return `${size.toFixed(
        index === 0 ? 0 : 1
    )} ${units[index]}`;
}


// =========================================================
// DATE FORMAT
// =========================================================
//
// FUNCTION:
// Backend-lendhu varra date/time-a
// browser readable format-la display pannum.
//
// INPUT:
// ISO Date
//
// OUTPUT:
// Local date + time
//
function formatDate(value) {

    if (!value) {

        return '-';
    }


    const date =
        new Date(value);


    if (
        Number.isNaN(
            date.getTime()
        )
    ) {

        return '-';
    }


    return date.toLocaleString();
}


// =========================================================
// SHOW MESSAGE
// =========================================================
//
// FUNCTION:
// Enable / Disable success or error message show pannum.
//
// INPUT:
// text -> Message
// type -> success / error
//
// OUTPUT:
// Admin page-la notification show aagum.
//
function showMessage(
    text,
    type = 'success'
) {

    if (!messageBox) {

        return;
    }


    messageBox.hidden = false;


    messageBox.className =
        `admin-message ${type}`;


    messageBox.textContent =
        text;


    // 3 seconds appuram message hide pannum
    window.setTimeout(
        () => {

            messageBox.hidden = true;

        },
        3000
    );
}


// =========================================================
// LOAD ADMIN DASHBOARD
// =========================================================
//
// API:
// GET /api/admin/dashboard
//
// RETURN:
// Total Users
// Total Uploads
// Total Stored Files
// Recent Upload Metadata
//
async function loadDashboard() {

    const dashboard =
        await adminApi.dashboard();


    // =====================================================
    // DASHBOARD STATISTICS
    // =====================================================

    if (statsContainer) {

        statsContainer.innerHTML = `

            <article class="admin-stat-card">

                <div class="stat-label">
                    Total Users
                </div>

                <div class="stat-number">
                    ${Number(
            dashboard.totalUsers ?? 0
        )}
                </div>

                <div class="stat-description">
                    Registered accounts
                </div>

            </article>


            <article class="admin-stat-card">

                <div class="stat-label">
                    Total Uploads
                </div>

                <div class="stat-number">
                    ${Number(
            dashboard.totalUploads ?? 0
        )}
                </div>

                <div class="stat-description">
                    Files uploaded to the vault
                </div>

            </article>


            <article class="admin-stat-card">

                <div class="stat-label">
                    Total Stored Files
                </div>

                <div class="stat-number">
                    ${Number(
            dashboard.totalStoredFiles ?? 0
        )}
                </div>

                <div class="stat-description">
                    Currently stored files
                </div>

            </article>

        `;
    }


    // =====================================================
    // UPLOAD ACTIVITY
    // =====================================================

    if (!uploadsContainer) {

        return;
    }


    const uploads =
        dashboard.recentUploads ?? [];


    // Uploads illa na
    if (uploads.length === 0) {

        uploadsContainer.innerHTML = `

            <tr>

                <td colspan="4">
                    No upload activity found.
                </td>

            </tr>

        `;

        return;
    }


    // Upload metadata table display
    uploadsContainer.innerHTML =
        uploads
            .map(upload => `

                <tr>

                    <td>

                        ${escapeHtml(
                upload.uploadedBy
            )}

                    </td>


                    <td>

                        <div class="admin-file-name">

                            <div class="file-icon">
                                📄
                            </div>

                            <span>

                                ${escapeHtml(
                upload.fileName
            )}

                            </span>

                        </div>

                    </td>


                    <td>

                        ${formatFileSize(
                upload.fileSize
            )}

                    </td>


                    <td>

                        ${escapeHtml(
                formatDate(
                    upload.uploadedAt
                )
            )}

                    </td>

                </tr>

            `)
            .join('');
}


// =========================================================
// RENDER USERS
// =========================================================
//
// FUNCTION:
// Users list-a table rows-aa convert pannum.
//
// INPUT:
// users -> Display panna vendiya users
//
// USE:
// 1. Full registered users
// 2. Search result users
//
// OUTPUT:
// Users table update aagum.
//
function renderUsers(users) {

    if (!usersContainer) {

        return;
    }


    // Matching users illa na
    if (
        !users ||
        users.length === 0
    ) {

        usersContainer.innerHTML = `

            <tr>

                <td colspan="5">
                    No matching users found.
                </td>

            </tr>

        `;

        return;
    }


    usersContainer.innerHTML =
        users
            .map(user => {

                // =================================================
                // ROLE
                // =================================================

                const role =
                    String(
                        user.role ?? 'User'
                    )
                        .trim();


                const isAdmin =
                    role
                        .toLowerCase()
                    === 'admin';


                const roleClass =
                    isAdmin
                        ? 'role-admin'
                        : 'role-user';


                // =================================================
                // STATUS
                // =================================================

                const active =
                    Boolean(
                        user.isActive
                    );


                const statusText =
                    active
                        ? 'Active'
                        : 'Disabled';


                const statusClass =
                    active
                        ? 'status-active'
                        : 'status-disabled';


                // =================================================
                // ACTION
                // =================================================
                //
                // User Active-na:
                // nextStatus = false
                // Button = Disable
                //
                // User Disabled-na:
                // nextStatus = true
                // Button = Enable
                //

                const nextStatus =
                    !active;


                const actionText =
                    active
                        ? 'Disable'
                        : 'Enable';


                const actionClass =
                    active
                        ? 'action-disable'
                        : 'action-enable';


                return `

                    <tr>

                        <td>

                            ${escapeHtml(
                    user.fullName
                )}

                        </td>


                        <td>

                            ${escapeHtml(
                    user.email
                )}

                        </td>


                        <td>

                            <span
                                class="admin-badge ${roleClass}">

                                ${escapeHtml(
                    role
                )}

                            </span>

                        </td>


                        <td>

                            <span
                                class="admin-badge ${statusClass}">

                                ${statusText}

                            </span>

                        </td>


                        <td>

                            ${isAdmin
                        ?
                        `
                                    <span
                                        class="admin-protected">

                                        Protected

                                    </span>
                                    `
                        :
                        `
                                    <button
                                        type="button"
                                        class="admin-user-action ${actionClass}"
                                        data-user-status
                                        data-user-id="${Number(user.id)}"
                                        data-next-status="${nextStatus}">

                                        ${actionText}

                                    </button>
                                    `
                    }

                        </td>

                    </tr>

                `;

            })
            .join('');
}


// =========================================================
// FILTER / SEARCH USERS
// =========================================================
//
// FUNCTION:
// Search box text base panni users filter pannum.
//
// SEARCH SUPPORT:
// Full Name
// Email
// Role
// Status
//
// EXAMPLE:
// piru
// gmail
// admin
// user
// active
// disabled
//
function filterUsers() {

    // Search box HTML-la illa na
    // full users display pannum
    if (!userSearch) {

        renderUsers(allUsers);

        return;
    }


    // Admin type pannura text
    const keyword =
        String(
            userSearch.value ?? ''
        )
            .trim()
            .toLowerCase();


    // Search empty-na
    // full users list show pannum
    if (keyword === '') {

        renderUsers(allUsers);

        return;
    }


    // =====================================================
    // USERS FILTER
    // =====================================================

    const filteredUsers =
        allUsers.filter(user => {

            // Name
            const fullName =
                String(
                    user.fullName ?? ''
                )
                    .toLowerCase();


            // Email
            const email =
                String(
                    user.email ?? ''
                )
                    .toLowerCase();


            // Role
            const role =
                String(
                    user.role ?? ''
                )
                    .toLowerCase();


            // Status
            const status =
                user.isActive
                    ? 'active'
                    : 'disabled';


            // Any field match aana
            // user result-la varuvaar
            return (

                fullName.includes(keyword) ||

                email.includes(keyword) ||

                role.includes(keyword) ||

                status.includes(keyword)

            );

        });


    // Filtered users display pannum
    renderUsers(
        filteredUsers
    );
}


// =========================================================
// LOAD USER ACCOUNTS
// =========================================================
//
// API:
// GET /api/admin/users
//
// FUNCTION:
// Backend-lendhu registered users ellam edukkum.
//
// OUTPUT:
// allUsers-la store pannum.
// Search keyword base panni render pannum.
//
async function loadUsers() {

    const users =
        await adminApi.users();


    // API valid array return pannina
    // allUsers-la save pannum
    allUsers =
        Array.isArray(users)
            ? users
            : [];


    // Search box currently empty-na
    // full users varum.
    //
    // Search text already irundha
    // matching users mattum varum.
    filterUsers();
}


// =========================================================
// USER SEARCH EVENT
// =========================================================
//
// FUNCTION:
// Admin search box-la type pannumbodhu
// instant search pannum.
//
// IMPORTANT:
// Search pannumbodhu backend API call panna maatom.
// Existing allUsers list mattum filter pannuvom.
//
if (userSearch) {

    userSearch.addEventListener(
        'input',
        () => {

            filterUsers();

        }
    );
}


// =========================================================
// ENABLE / DISABLE USER
// =========================================================
//
// FUNCTION:
// User Action button click handle pannum.
//
// ENABLE:
// isActive = true
//
// DISABLE:
// isActive = false
//
// API:
// PUT /api/admin/users/{id}/status
//
if (usersContainer) {

    usersContainer.addEventListener(
        'click',
        async event => {

            // Click panna element-la
            // data-user-status irukka check pannum
            const button =
                event.target.closest(
                    '[data-user-status]'
                );


            // User status button illa na
            // function stop pannum
            if (!button) {

                return;
            }


            // =================================================
            // USER ID
            // =================================================

            const userId =
                Number(
                    String(
                        button.dataset.userId
                    )
                        .trim()
                );


            // Invalid user ID-na stop
            if (
                !Number.isInteger(userId) ||
                userId <= 0
            ) {

                showMessage(
                    'Invalid user ID.',
                    'error'
                );

                return;
            }


            // =================================================
            // NEW USER STATUS
            // =================================================
            //
            // Important:
            // HTML data attribute-la whitespace irundhaalum
            // trim() use panni correct boolean value edukkum.
            //

            const newStatus =
                String(
                    button.dataset.nextStatus
                )
                    .trim()
                    .toLowerCase()
                === 'true';


            console.log(
                'User status change:',
                {
                    userId,
                    newStatus
                }
            );


            // Multiple clicks avoid panna
            button.disabled = true;


            try {

                // =============================================
                // UPDATE STATUS API
                // =============================================

                await adminApi.setStatus(
                    userId,
                    newStatus
                );


                // =============================================
                // SUCCESS MESSAGE
                // =============================================

                showMessage(
                    newStatus
                        ? 'User enabled successfully.'
                        : 'User disabled successfully.',
                    'success'
                );


                // =============================================
                // REFRESH USERS
                // =============================================
                //
                // Status update aana appuram
                // fresh database data load pannuvom.
                //
                // Search text already irundha
                // filterUsers() atha preserve pannum.
                //

                await loadUsers();

            }
            catch (error) {

                console.error(
                    'Status update failed:',
                    error
                );


                showMessage(
                    error.message ||
                    'Unable to update user status.',
                    'error'
                );


                // API fail aana button thirumba enable pannum
                button.disabled = false;
            }
        }
    );
}


// =========================================================
// ADMIN NAVIGATION ACTIVE STATE
// =========================================================
//
// Overview
// Users
// Upload Activity
//
// Click panna active nav link highlight aagum.
//
const navLinks =
    document.querySelectorAll(
        '.admin-nav-link'
    );


navLinks.forEach(link => {

    link.addEventListener(
        'click',
        () => {

            // Ellaa nav links active remove
            navLinks.forEach(
                item => {

                    item.classList.remove(
                        'active'
                    );

                }
            );


            // Current click panna link active
            link.classList.add(
                'active'
            );

        }
    );

});


// =========================================================
// INITIAL ADMIN PAGE LOAD
// =========================================================
//
// Admin page open aana:
//
// 1. Dashboard statistics load
// 2. Upload activity load
// 3. Registered users load
//
// Rendu API calls parallel-a run aagum.
//
async function initialiseAdmin() {

    try {

        await Promise.all(
            [
                loadDashboard(),
                loadUsers()
            ]
        );

    }
    catch (error) {

        console.error(
            'Admin dashboard error:',
            error
        );


        showMessage(
            error.message ||
            'Unable to load admin dashboard.',
            'error'
        );


        const errorMessage =
            String(
                error.message ?? ''
            );


        // =====================================================
        // 403 FORBIDDEN
        // =====================================================
        //
        // Normal User admin page access panna try pannina
        // backend 403 return pannum.
        //
        if (
            errorMessage.includes(
                '403'
            )
        ) {

            window.location.replace(
                '/html/dashboard.html'
            );

            return;
        }


        // =====================================================
        // 401 UNAUTHORIZED
        // =====================================================
        //
        // Token expired / invalid / missing-na
        // login page-ku redirect pannum.
        //
        if (
            errorMessage.includes(
                '401'
            )
        ) {

            window.location.replace(
                '/html/login.html'
            );
        }
    }
}


// =========================================================
// START ADMIN PAGE
// =========================================================

initialiseAdmin();