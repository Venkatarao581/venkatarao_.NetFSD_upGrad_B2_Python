const UIService = (() => {

    // ── Employee Table ────────────────────────────────────────────────────────

    /**
     * Renders the employee table rows from a paged result.
     * @param {PagedResult} pagedResult - from StorageService.getAll()
     */
    function renderEmployeeTable(pagedResult) {
        const tbody = $("#employeeTableBody");
        tbody.empty();

        const employees = pagedResult.data || [];

        if (employees.length === 0) {
            tbody.append(`
                <tr>
                    <td colspan="10" class="text-center text-muted py-4">
                        <i class="bi bi-search me-2"></i>No employees found.
                    </td>
                </tr>`);
            return;
        }

        employees.forEach(emp => {
            const initials   = (emp.firstName[0] + emp.lastName[0]).toUpperCase();
            const statusBadge = emp.status === "Active"
                ? `<span class="badge bg-success">Active</span>`
                : `<span class="badge bg-danger">Inactive</span>`;
            const deptColor  = _deptColor(emp.department);
            const salary     = "₹" + Number(emp.salary).toLocaleString("en-IN");
            const joinDate   = new Date(emp.joinDate).toLocaleDateString("en-IN",
                { day: "2-digit", month: "short", year: "numeric" });

            // Admin-only buttons hidden for Viewer via applyRoleUI()
            tbody.append(`
                <tr>
                    <td>${emp.id}</td>
                    <td>
                        <div class="emp-avatar" style="background:${deptColor}">${initials}</div>
                    </td>
                    <td>${emp.firstName} ${emp.lastName}</td>
                    <td>${emp.email}</td>
                    <td><span class="badge" style="background:${deptColor}">${emp.department}</span></td>
                    <td>${emp.designation}</td>
                    <td>${salary}</td>
                    <td>${joinDate}</td>
                    <td>${statusBadge}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-info me-1 btn-view" data-id="${emp.id}" title="View">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-warning me-1 btn-edit admin-only" data-id="${emp.id}" title="Edit">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-delete admin-only" data-id="${emp.id}" title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>`);
        });

        // Show/hide admin buttons after render
        applyRoleUI();

        // Record count label
        const start = ((pagedResult.page - 1) * pagedResult.pageSize) + 1;
        const end   = Math.min(pagedResult.page * pagedResult.pageSize, pagedResult.totalCount);
        $("#recordCountLabel").text(
            `Showing ${start}–${end} of ${pagedResult.totalCount} employees`
        );
    }

    // ── Pagination ────────────────────────────────────────────────────────────

    /**
     * Renders Bootstrap pagination buttons below the employee table.
     * @param {PagedResult} pagedResult
     * @param {function} onPageChange - callback(pageNumber)
     */
    function renderPagination(pagedResult, onPageChange) {
        const container = $("#paginationContainer");
        container.empty();

        if (pagedResult.totalPages <= 1) return;

        const ul = $(`<ul class="pagination pagination-sm mb-0"></ul>`);

        // Prev button
        ul.append(`
            <li class="page-item ${pagedResult.hasPrevPage ? "" : "disabled"}">
                <a class="page-link" href="#" data-page="${pagedResult.page - 1}">
                    <i class="bi bi-chevron-left"></i>
                </a>
            </li>`);

        // Page number buttons (show max 5 around current)
        const start = Math.max(1, pagedResult.page - 2);
        const end   = Math.min(pagedResult.totalPages, pagedResult.page + 2);

        if (start > 1) ul.append(`<li class="page-item disabled"><span class="page-link">…</span></li>`);

        for (let p = start; p <= end; p++) {
            ul.append(`
                <li class="page-item ${p === pagedResult.page ? "active" : ""}">
                    <a class="page-link" href="#" data-page="${p}">${p}</a>
                </li>`);
        }

        if (end < pagedResult.totalPages)
            ul.append(`<li class="page-item disabled"><span class="page-link">…</span></li>`);

        // Next button
        ul.append(`
            <li class="page-item ${pagedResult.hasNextPage ? "" : "disabled"}">
                <a class="page-link" href="#" data-page="${pagedResult.page + 1}">
                    <i class="bi bi-chevron-right"></i>
                </a>
            </li>`);

        // Bind click
        ul.on("click", ".page-link", function (e) {
            e.preventDefault();
            const pg = parseInt($(this).data("page"));
            if (!isNaN(pg) && pg >= 1 && pg <= pagedResult.totalPages)
                onPageChange(pg);
        });

        container.append(ul);
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────

    /**
     * Renders the 4 KPI summary cards from DashboardSummaryDto.
     */
    function renderDashboardCards(summary) {
        $("#totalEmployees").text(summary.total);
        $("#activeEmployees").text(summary.active);
        $("#inactiveEmployees").text(summary.inactive);
        $("#totalDepartments").text(summary.departments);
    }

    /**
     * Renders the department breakdown table + CSS bar chart.
     */
    function renderDepartmentBreakdown(breakdown) {
        const tbody = $("#deptBreakdownBody");
        tbody.empty();

        if (!breakdown || breakdown.length === 0) {
            tbody.append(`<tr><td colspan="3" class="text-muted text-center">No data</td></tr>`);
            return;
        }

        breakdown.forEach(d => {
            const color = _deptColor(d.department);
            tbody.append(`
                <tr>
                    <td><span class="badge" style="background:${color}">${d.department}</span></td>
                    <td>${d.count}</td>
                    <td>
                        <div class="dept-bar-wrap">
                            <div class="dept-bar" style="width:${d.percentage}%;background:${color}"></div>
                            <span class="dept-bar-label">${d.percentage}%</span>
                        </div>
                    </td>
                </tr>`);
        });
    }

    /**
     * Renders the Recent Employees panel (last 5 added).
     */
    function renderRecentEmployees(employees) {
        const container = $("#recentEmployeesBody");
        container.empty();

        if (!employees || employees.length === 0) {
            container.append(`<p class="text-muted text-center">No employees yet.</p>`);
            return;
        }

        employees.forEach(emp => {
            const initials  = (emp.firstName[0] + emp.lastName[0]).toUpperCase();
            const color     = _deptColor(emp.department);
            const statusBadge = emp.status === "Active"
                ? `<span class="badge bg-success">Active</span>`
                : `<span class="badge bg-danger">Inactive</span>`;

            container.append(`
                <div class="recent-emp-row d-flex align-items-center mb-2">
                    <div class="emp-avatar me-2" style="background:${color};width:36px;height:36px;font-size:.75rem">${initials}</div>
                    <div class="flex-grow-1">
                        <div class="fw-semibold">${emp.firstName} ${emp.lastName}</div>
                        <small class="text-muted">${emp.designation}</small>
                    </div>
                    <div class="d-flex gap-1">
                        <span class="badge" style="background:${color}">${emp.department}</span>
                        ${statusBadge}
                    </div>
                </div>`);
        });
    }

    // ── Modals ────────────────────────────────────────────────────────────────

    /**
     * Shows a Bootstrap modal by ID.
     */
    function showModal(modalId) {
        const modal = new bootstrap.Modal(document.getElementById(modalId));
        modal.show();
    }

    /**
     * Hides a Bootstrap modal by ID.
     */
    function hideModal(modalId) {
        const modalEl = document.getElementById(modalId);
        const modal   = bootstrap.Modal.getInstance(modalEl);
        if (modal) modal.hide();
    }

    /**
     * Pre-populates the Add/Edit form with an employee's current data.
     */
    function populateForm(emp) {
        $("#empId").val(emp.id);
        $("#firstName").val(emp.firstName);
        $("#lastName").val(emp.lastName);
        $("#email").val(emp.email);
        $("#phone").val(emp.phone);
        $("#department").val(emp.department);
        $("#designation").val(emp.designation);
        $("#salary").val(emp.salary);
        $("#joinDate").val(emp.joinDate?.split("T")[0]);
        $("#status").val(emp.status);
    }

    /**
     * Populates the View Employee modal with read-only data.
     */
    function populateViewModal(emp) {
        const initials = (emp.firstName[0] + emp.lastName[0]).toUpperCase();
        const color    = _deptColor(emp.department);
        const salary   = "₹" + Number(emp.salary).toLocaleString("en-IN");
        const joinDate = new Date(emp.joinDate).toLocaleDateString("en-IN",
            { day: "2-digit", month: "short", year: "numeric" });

        $("#viewAvatar").css("background", color).text(initials);
        $("#viewName").text(`${emp.firstName} ${emp.lastName}`);
        $("#viewDept").text(emp.department);
        $("#viewEmail").text(emp.email);
        $("#viewPhone").text(emp.phone);
        $("#viewDesignation").text(emp.designation);
        $("#viewSalary").text(salary);
        $("#viewJoinDate").text(joinDate);
        $("#viewStatus")
            .text(emp.status)
            .removeClass("bg-success bg-danger")
            .addClass(emp.status === "Active" ? "bg-success" : "bg-danger");
    }

    /**
     * Clears the Add/Edit form and all inline error messages.
     */
    function clearForm() {
        $("#empId").val("");
        ["firstName","lastName","email","phone","department",
         "designation","salary","joinDate","status"].forEach(f => {
            $(`#${f}`).val("");
            $(`#${f}Error`).text("");
        });
    }

    // ── Toast ─────────────────────────────────────────────────────────────────

    /**
     * Shows a Bootstrap toast notification.
     * @param {string} message
     * @param {"success"|"danger"|"warning"|"info"} type
     */
    function showToast(message, type = "success") {
        const toastEl = $("#liveToast");
        toastEl.removeClass("text-bg-success text-bg-danger text-bg-warning text-bg-info")
               .addClass(`text-bg-${type}`);
        $("#toastMessage").text(message);
        const toast = new bootstrap.Toast(toastEl[0], { delay: 3000 });
        toast.show();
    }

    // ── Inline Errors ─────────────────────────────────────────────────────────

    /**
     * Displays field-level inline error messages below each input.
     * @param {object} errors - { fieldName: "message" }
     */
    function showInlineErrors(errors) {
        Object.entries(errors).forEach(([field, msg]) => {
            $(`#${field}Error`).text(msg);
        });
    }
    function clearForm_errors() {
        $("[id$='Error']").text("");
    }
    function applyRoleUI() {
        if (AuthService.isAdmin()) {
            $(".admin-only").show();
            $("#viewerNotice").hide();
        } else {
            $(".admin-only").hide();
            $("#viewerNotice").show();
        }

        const user = AuthService.getCurrentUser();
        if (user) {
            $("#navUsername").text(user.username);
            $("#navRoleBadge")
                .text(user.role)
                .removeClass("bg-primary bg-secondary")
                .addClass(user.role === "Admin" ? "bg-primary" : "bg-secondary");
        }
    }
    function setActiveNav(view) {
        $(".nav-link").removeClass("active");
        $(`#nav${view}`).addClass("active");
    }
    function _deptColor(dept) {
        const colors = {
            "Engineering": "#0d6efd",
            "Marketing":   "#fd7e14",
            "HR":          "#6f42c1",
            "Finance":     "#20c997",
            "Operations":  "#dc3545"
        };
        return colors[dept] || "#6c757d";
    }

    return {
        renderEmployeeTable,
        renderPagination,
        renderDashboardCards,
        renderDepartmentBreakdown,
        renderRecentEmployees,
        showModal,
        hideModal,
        populateForm,
        populateViewModal,
        clearForm,
        clearForm_errors,
        showToast,
        showInlineErrors,
        applyRoleUI,
        setActiveNav
    };

})();