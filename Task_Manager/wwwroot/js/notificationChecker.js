// Notification Checker - Periodically checks for events that should trigger notifications

class NotificationChecker {
    constructor(notificationService) {
        this.notificationService = notificationService;
        this.checkInterval = 3600000; // 1 hour in milliseconds
        this.timerId = null;
    }

    // Start periodic checking
    startChecking() {
        // Check immediately on startup
        this.checkDeadlines();
        
        // Then set interval for periodic checks
        this.timerId = setInterval(() => {
            this.checkDeadlines();
        }, this.checkInterval);
    }

    // Stop checking
    stopChecking() {
        if (this.timerId) {
            clearInterval(this.timerId);
            this.timerId = null;
        }
    }

    // Check all relevant deadlines
    async checkDeadlines() {
        try {
            // Fetch projects and tasks from API
            const projects = await this.fetchProjects();
            const tasks = await this.fetchTasks();
            
            // Check deadlines and generate notifications
            this.notificationService.checkProjectDeadlines(projects);
            this.notificationService.checkTaskDeadlines(tasks);
        } catch (error) {
            console.error('Error checking deadlines:', error);
        }
    }

    // Fetch projects from API
    async fetchProjects() {
        try {
            const response = await fetch('/api/projects');
            if (!response.ok) throw new Error('Failed to fetch projects');
            return await response.json();
        } catch (error) {
            console.error('Error fetching projects:', error);
            return [];
        }
    }

    // Fetch tasks from API
    async fetchTasks() {
        try {
            const response = await fetch('/api/tasks');
            if (!response.ok) throw new Error('Failed to fetch tasks');
            return await response.json();
        } catch (error) {
            console.error('Error fetching tasks:', error);
            return [];
        }
    }
}
