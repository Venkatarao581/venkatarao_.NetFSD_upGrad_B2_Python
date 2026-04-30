const DashboardService = (() => {

    /**
     * Fetches all dashboard data from the server in one API call.
     * Returns { total, active, inactive, departments, departmentBreakdown, recentEmployees }
     * @returns {Promise<DashboardSummaryDto>}
     */
    async function getSummary() {
        return await StorageService.getDashboard();
    }
    async function getDepartmentBreakdown() {
        const summary = await getSummary();
        return summary.departmentBreakdown;
    }

    async function getRecentEmployees(n = 5) {
        const summary = await getSummary();
        return summary.recentEmployees.slice(0, n);
    }

    return { getSummary, getDepartmentBreakdown, getRecentEmployees };

})();