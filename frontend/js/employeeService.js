

const EmployeeService = (() => {

    /**
     * Returns a paginated, filtered, sorted list of employees from the API.
     * @param {object} params - { search, department, status, sortBy, sortDir, page, pageSize }
     * @returns {Promise<PagedResult>}
     */
    async function getAll(params = {}) {
        return await StorageService.getAll(params);
    }

    /**
     * Returns a single employee by ID from the API.
     * @param {number} id
     * @returns {Promise<EmployeeResponseDto|null>}
     */
    async function getById(id) {
        try {
            return await StorageService.getById(id);
        } catch {
            return null;
        }
    }

    /**
     * Creates a new employee via API.
     * @param {object} data - employee form data
     * @returns {Promise<EmployeeResponseDto>}
     * @throws on duplicate email (409) or validation error (400)
     */
    async function add(data) {
        return await StorageService.add(data);
    }

    /**
     * Updates an existing employee via API.
     * @param {number} id
     * @param {object} data - updated employee form data
     * @returns {Promise<EmployeeResponseDto>}
     * @throws on not found (404) or email conflict (409)
     */
    async function update(id, data) {
        return await StorageService.update(id, data);
    }

    /**
     * Deletes an employee by ID via API.
     * @param {number} id
     * @returns {Promise<{message: string}>}
     * @throws on not found (404)
     */
    async function remove(id) {
        return await StorageService.remove(id);
    }

    /**
     * Returns unique department values from a full employee list.
     * Used to populate the department filter dropdown.
     * @returns {Promise<string[]>}
     */
    async function getDepartments() {
        try {
            const result = await StorageService.getAll({ pageSize: 100 });
            const depts = [...new Set(result.data.map(e => e.department))].sort();
            return depts;
        } catch {
            return ["Engineering", "Marketing", "HR", "Finance", "Operations"];
        }
    }

    /**
     * Maps a server-side 409/400 error response to inline field error messages.
     * @param {Error} err - error thrown by StorageService
     * @returns {object} - field-keyed error messages e.g. { email: "..." }
     */
    function mapServerErrors(err) {
        const errors = {};
        if (err.status === 409) {
            errors.email = err.message || "This email is already in use.";
        } else if (err.status === 400 && err.data?.errors) {
            Object.entries(err.data.errors).forEach(([field, msgs]) => {
                errors[field.toLowerCase()] = Array.isArray(msgs) ? msgs[0] : msgs;
            });
        }
        return errors;
    }

    return { getAll, getById, add, update, remove, getDepartments, mapServerErrors };

})();