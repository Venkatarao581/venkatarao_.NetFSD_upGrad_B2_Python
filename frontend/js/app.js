$(document).ready(function () {
    let _state = {
        search:     "",
        department: "",
        status:     "",
        sortBy:     "name",
        sortDir:    "asc",
        page:       1,
        pageSize:   PAGE_SIZE  // from config.js
    };

    let _searchDebounce = null;
    let _editingId      = null; 
    function showView(view) {
        $("#loginSection, #signupSection, #dashboardSection, #employeeSection, #mainNav").hide();

        if (view === "login")     { $("#loginSection").show(); }
        else if (view === "signup")    { $("#signupSection").show(); }
        else {
            $("#mainNav").show();
            if (view === "dashboard") {
                $("#dashboardSection").show();
                UIService.setActiveNav("Dashboard");
                loadDashboard();
            } else {
                $("#employeeSection").show();
                UIService.setActiveNav("Employees");
                loadEmployees();
            }
        }
    }
    showView("login");

    $("#goToSignup, #linkSignup").on("click", function (e) {
        e.preventDefault();
        showView("signup");
    });

    $("#goToLogin, #linkLogin").on("click", function (e) {
        e.preventDefault();
        showView("login");
    });

    $("#loginForm").on("submit", async function (e) {
        e.preventDefault();
        UIService.clearForm_errors();

        const username = $("#loginUsername").val().trim();
        const password = $("#loginPassword").val();

        const vErrors = ValidationService.validateAuthForm({ username, password });
        if (Object.keys(vErrors).length) {
            UIService.showInlineErrors({
                loginUsernameError: vErrors.username || "",
                loginPasswordError: vErrors.password || ""
            });
            return;
        }

        const result = await AuthService.login(username, password);
        if (!result.success) {
            $("#loginError").text(result.message || "Invalid credentials.").show();
            return;
        }

        $("#loginError").hide();
        UIService.applyRoleUI();
        showView("dashboard");
    });

    $("#signupForm").on("submit", async function (e) {
        e.preventDefault();
        UIService.clearForm_errors();

        const username        = $("#signupUsername").val().trim();
        const password        = $("#signupPassword").val();
        const confirmPassword = $("#signupConfirm").val();

        const vErrors = ValidationService.validateAuthForm({ username, password, confirmPassword });
        if (Object.keys(vErrors).length) {
            if (vErrors.username)        $("#signupUsernameError").text(vErrors.username);
            if (vErrors.password)        $("#signupPasswordError").text(vErrors.password);
            if (vErrors.confirmPassword) $("#signupConfirmError").text(vErrors.confirmPassword);
            return;
        }

        const result = await AuthService.signup(username, password);
        if (!result.success) {
            $("#signupUsernameError").text(result.message || "Registration failed.");
            return;
        }

        UIService.showToast("Account created! Please log in.", "success");
        showView("login");
    });
    $("#btnLogout").on("click", function () {
        AuthService.logout();
        showView("login");
    });

    $("#navDashboard").on("click", function (e) {
        e.preventDefault();
        showView("dashboard");
    });

    $("#navEmployees").on("click", function (e) {
        e.preventDefault();
        showView("employees");
    });


    async function loadDashboard() {
        try {
            const summary = await DashboardService.getSummary();
            UIService.renderDashboardCards(summary);
            UIService.renderDepartmentBreakdown(summary.departmentBreakdown);
            UIService.renderRecentEmployees(summary.recentEmployees);
        } catch (err) {
            UIService.showToast("Failed to load dashboard. Is the API running?", "danger");
        }
    }

    $("#btnDashAddEmployee").on("click", function () {
        openAddModal();
    });

    async function loadEmployees() {
        try {
            const result = await EmployeeService.getAll(_state);
            UIService.renderEmployeeTable(result);
            UIService.renderPagination(result, (page) => {
                _state.page = page;
                loadEmployees();
            });
        } catch (err) {
            if (err.status === 401) {
                UIService.showToast("Session expired. Please log in again.", "warning");
                AuthService.logout();
                showView("login");
            } else {
                UIService.showToast("Failed to load employees.", "danger");
            }
        }
    }


    $("#searchInput").on("input", function () {
        clearTimeout(_searchDebounce);
        _searchDebounce = setTimeout(() => {
            _state.search = $(this).val().trim();
            _state.page   = 1;
            loadEmployees();
        }, 350);
    });


    $("#deptFilter").on("change", function () {
        _state.department = $(this).val();
        _state.page       = 1;
        loadEmployees();
    });
    $(".btn-status-filter").on("click", function () {
        $(".btn-status-filter").removeClass("active");
        $(this).addClass("active");
        _state.status = $(this).data("status") || "";
        _state.page   = 1;
        loadEmployees();
    });
    $(document).on("click", ".sortable-col", function () {
        const field = $(this).data("sort");
        if (_state.sortBy === field) {
            _state.sortDir = _state.sortDir === "asc" ? "desc" : "asc";
        } else {
            _state.sortBy  = field;
            _state.sortDir = "asc";
        }
        _state.page = 1;

        // Update sort indicators
        $(".sortable-col").removeClass("sort-asc sort-desc");
        $(this).addClass(_state.sortDir === "asc" ? "sort-asc" : "sort-desc");

        loadEmployees();
    });
    $("#btnAddEmployee, #navBtnAddEmployee").on("click", function () {
        openAddModal();
    });

    function openAddModal() {
        _editingId = null;
        UIService.clearForm();
        $("#employeeModalTitle").text("Add Employee");
        $("#btnSaveEmployee").text("Save Employee");
        UIService.showModal("employeeModal");
    }

    // ── View Employee ─────────────────────────────────────────────────────────

    $(document).on("click", ".btn-view", async function () {
        const id  = $(this).data("id");
        const emp = await EmployeeService.getById(id);
        if (!emp) { UIService.showToast("Employee not found.", "danger"); return; }
        UIService.populateViewModal(emp);
        UIService.showModal("viewEmployeeModal");
    });

    // ── Edit Employee ─────────────────────────────────────────────────────────

    $(document).on("click", ".btn-edit", async function () {
        const id  = $(this).data("id");
        const emp = await EmployeeService.getById(id);
        if (!emp) { UIService.showToast("Employee not found.", "danger"); return; }

        _editingId = id;
        UIService.clearForm();
        UIService.populateForm(emp);
        $("#employeeModalTitle").text("Edit Employee");
        $("#btnSaveEmployee").text("Update Employee");
        UIService.showModal("employeeModal");
    });

    // ── Save Employee (Add / Edit) ────────────────────────────────────────────

    $("#btnSaveEmployee").on("click", async function () {
        UIService.clearForm_errors();

        const data = {
            firstName:   $("#firstName").val().trim(),
            lastName:    $("#lastName").val().trim(),
            email:       $("#email").val().trim(),
            phone:       $("#phone").val().trim(),
            department:  $("#department").val(),
            designation: $("#designation").val().trim(),
            salary:      parseFloat($("#salary").val()),
            joinDate:    $("#joinDate").val(),
            status:      $("#status").val()
        };

        // Client-side validation first
        const vErrors = ValidationService.validateEmployeeForm(data);
        if (Object.keys(vErrors).length) {
            UIService.showInlineErrors(vErrors);
            return;
        }

        try {
            if (_editingId) {
                await EmployeeService.update(_editingId, data);
                UIService.showToast("Employee updated successfully!", "success");
            } else {
                await EmployeeService.add(data);
                UIService.showToast("Employee added successfully!", "success");
            }

            UIService.hideModal("employeeModal");
            _state.page = 1;
            loadEmployees();
            loadDashboard();

        } catch (err) {
            const serverErrors = ValidationService.mapServerErrors(err);
            if (Object.keys(serverErrors).length) {
                UIService.showInlineErrors(serverErrors);
            } else {
                UIService.showToast(err.message || "Something went wrong.", "danger");
            }
        }
    });

    // ── Delete Employee ───────────────────────────────────────────────────────

    let _deleteId = null;

    $(document).on("click", ".btn-delete", async function () {
        const id  = $(this).data("id");
        const emp = await EmployeeService.getById(id);
        if (!emp) return;

        _deleteId = id;
        $("#deleteEmployeeName").text(`${emp.firstName} ${emp.lastName}`);
        UIService.showModal("deleteModal");
    });

    $("#btnConfirmDelete").on("click", async function () {
        if (!_deleteId) return;

        try {
            await EmployeeService.remove(_deleteId);
            UIService.hideModal("deleteModal");
            UIService.showToast("Employee deleted successfully.", "success");

            // If last record on page, go back one page
            if (_state.page > 1) _state.page--;
            loadEmployees();
            loadDashboard();
        } catch (err) {
            UIService.showToast(err.message || "Failed to delete employee.", "danger");
        }

        _deleteId = null;
    });

    // ── Populate Department Dropdown ──────────────────────────────────────────

    async function populateDeptFilter() {
        const depts = await EmployeeService.getDepartments();
        const sel   = $("#deptFilter");
        sel.find("option:not(:first)").remove();
        depts.forEach(d => sel.append(`<option value="${d}">${d}</option>`));
    }

    populateDeptFilter();

});