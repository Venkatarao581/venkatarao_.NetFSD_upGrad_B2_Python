const StorageService = (() => {

    // ── Private helpers ───────────────────────────────────────────────────────

    /**
     * Builds fetch headers. Attaches JWT Bearer token if available.
     * @param {boolean} withAuth - whether to include Authorization header
     */
    function _headers(withAuth = true) {
        const headers = { "Content-Type": "application/json" };
        if (withAuth) {
            const token = AuthService.getToken();
            if (token) headers["Authorization"] = `Bearer ${token}`;
        }
        return headers;
    }
    async function _handleResponse(res) {
        const data = await res.json().catch(() => ({}));
        if (!res.ok) {
            const msg = data.message || data.title || `HTTP ${res.status}`;
            const err = new Error(msg);
            err.status = res.status;
            err.data   = data;
            throw err;
        }
        return data;
    }  
    async function login({ username, password }) {
        const res = await fetch(`${API_BASE_URL}/auth/login`, {
            method:  "POST",
            headers: _headers(false),
            body:    JSON.stringify({ username, password })
        });
        return _handleResponse(res);
    }
    async function register({ username, password, role = "Viewer" }) {
        const res = await fetch(`${API_BASE_URL}/auth/register`, {
            method:  "POST",
            headers: _headers(false),
            body:    JSON.stringify({ username, password, role })
        });
        return _handleResponse(res);
    }
    async function getAll(params = {}) {
        const qs = new URLSearchParams();
        if (params.search)     qs.set("search",     params.search);
        if (params.department) qs.set("department",  params.department);
        if (params.status)     qs.set("status",      params.status);
        if (params.sortBy)     qs.set("sortBy",      params.sortBy);
        if (params.sortDir)    qs.set("sortDir",     params.sortDir);
        if (params.page)       qs.set("page",        params.page);
        if (params.pageSize)   qs.set("pageSize",    params.pageSize);

        const res = await fetch(`${API_BASE_URL}/employees?${qs}`, {
            headers: _headers()
        });
        return _handleResponse(res);
    }
    async function getById(id) {
        const res = await fetch(`${API_BASE_URL}/employees/${id}`, {
            headers: _headers()
        });
        return _handleResponse(res);
    }
    async function add(employeeData) {
        const res = await fetch(`${API_BASE_URL}/employees`, {
            method:  "POST",
            headers: _headers(),
            body:    JSON.stringify(employeeData)
        });
        return _handleResponse(res);
    }
    async function update(id, employeeData) {
        const res = await fetch(`${API_BASE_URL}/employees/${id}`, {
            method:  "PUT",
            headers: _headers(),
            body:    JSON.stringify(employeeData)
        });
        return _handleResponse(res);
    }

    async function remove(id) {
        const res = await fetch(`${API_BASE_URL}/employees/${id}`, {
            method:  "DELETE",
            headers: _headers()
        });
        return _handleResponse(res);
    }
    async function getDashboard() {
        const res = await fetch(`${API_BASE_URL}/employees/dashboard`, {
            headers: _headers()
        });
        return _handleResponse(res);
    }

    // ── Public API ────────────────────────────────────────────────────────────
    return { login, register, getAll, getById, add, update, remove, getDashboard };

})();