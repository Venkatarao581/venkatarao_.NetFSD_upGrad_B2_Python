const ValidationService = (() => {

    /**
     * Validates the employee Add/Edit form.
     * @param {object} data - form field values
     * @returns {object} - { fieldName: "error message" } — empty object means valid
     */
    function validateEmployeeForm(data) {
        const errors = {};

        if (!data.firstName?.trim())        errors.firstName   = "First name is required.";
        if (!data.lastName?.trim())         errors.lastName    = "Last name is required.";

        if (!data.email?.trim()) {
            errors.email = "Email is required.";
        } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
            errors.email = "Please enter a valid email address.";
        }

        if (!data.phone?.trim()) {
            errors.phone = "Phone number is required.";
        } else if (!/^\d{10}$/.test(data.phone.trim())) {
            errors.phone = "Phone must be exactly 10 digits.";
        }

        if (!data.department)               errors.department  = "Please select a department.";
        if (!data.designation?.trim())      errors.designation = "Designation is required.";

        if (!data.salary && data.salary !== 0) {
            errors.salary = "Salary is required.";
        } else if (isNaN(data.salary) || Number(data.salary) <= 0) {
            errors.salary = "Salary must be a positive number.";
        }

        if (!data.joinDate)                 errors.joinDate    = "Join date is required.";
        if (!data.status)                   errors.status      = "Please select a status.";

        return errors;
    }

    /**
     * Validates the Auth (Login / Signup) form.
     * @param {object} data - { username, password, confirmPassword? }
     * @returns {object} - field-keyed error messages
     */
    function validateAuthForm(data) {
        const errors = {};

        if (!data.username?.trim())
            errors.username = "Username is required.";

        if (!data.password?.trim()) {
            errors.password = "Password is required.";
        } else if (data.password.length < 6) {
            errors.password = "Password must be at least 6 characters.";
        }

        // confirmPassword only present on signup form
        if (data.confirmPassword !== undefined) {
            if (!data.confirmPassword?.trim()) {
                errors.confirmPassword = "Please confirm your password.";
            } else if (data.password !== data.confirmPassword) {
                errors.confirmPassword = "Passwords do not match.";
            }
        }

        return errors;
    }

    /**
     * Translates API 409 Conflict / 400 Bad Request errors to field error objects.
     * @param {Error} err - thrown by StorageService (has .status and .data)
     * @returns {object} - { fieldName: "message" }
     */
    function mapServerErrors(err) {
        const errors = {};
        if (err.status === 409) {
            errors.email    = err.message || "This email is already in use.";
            errors.username = err.message || "This username is already taken.";
        } else if (err.status === 400 && err.data?.errors) {
            Object.entries(err.data.errors).forEach(([field, msgs]) => {
                const key = field.charAt(0).toLowerCase() + field.slice(1);
                errors[key] = Array.isArray(msgs) ? msgs[0] : msgs;
            });
        }
        return errors;
    }

    return { validateEmployeeForm, validateAuthForm, mapServerErrors };

})();