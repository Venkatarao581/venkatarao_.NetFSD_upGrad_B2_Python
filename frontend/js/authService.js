

const AuthService = (() => {

    // In-memory session — lost on page refresh (correct secure behaviour)
    let _session = null; // { username, role, token }

    /**
     * Attempts login via API.
     * On success, stores session in memory.
     * @returns {{ success, message, username?, role?, token? }}
     */
    async function login(username, password) {
        try {
            const data = await StorageService.login({ username, password });
            _session = { username: data.username, role: data.role, token: data.token };
            return { success: true, username: data.username, role: data.role };
        } catch (err) {
            return { success: false, message: err.message || "Invalid credentials." };
        }
    }

    /**
     * Attempts registration via API.
     * On success, does NOT auto-login — redirects user to login page.
     * @returns {{ success, message }}
     */
    async function signup(username, password, role = "Viewer") {
        try {
            await StorageService.register({ username, password, role });
            return { success: true };
        } catch (err) {
            return { success: false, message: err.message || "Registration failed." };
        }
    }

    /**
     * Clears the in-memory session (logout).
     */
    function logout() {
        _session = null;
    }

    /**
     * Returns true if a valid session exists.
     */
    function isLoggedIn() {
        return _session !== null;
    }

    /**
     * Returns the stored JWT token string, or null.
     * Called by StorageService._headers() to attach Authorization header.
     */
    function getToken() {
        return _session?.token ?? null;
    }

    /**
     * Returns the current logged-in user object { username, role, token }.
     */
    function getCurrentUser() {
        return _session;
    }

    /**
     * Returns true if the current user has the Admin role.
     */
    function isAdmin() {
        return _session?.role === "Admin";
    }

    return { login, signup, logout, isLoggedIn, getToken, getCurrentUser, isAdmin };

})();