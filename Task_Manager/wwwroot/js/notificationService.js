// Notification Service - Handles notification generation, storage, and management

class NotificationService {
    constructor() {
        this.storageKey = 'userNotifications';
        this.notifications = this.getNotifications();
        
        // Remove any test notifications that might exist in storage
        this.notifications = this.notifications.filter(n => n.type !== 'test');
        this.saveNotifications();
        
        this.onNotificationsUpdated = null; // Callback for UI updates
    }

    // Get all notifications from localStorage
    getNotifications() {
        const storedNotifications = localStorage.getItem(this.storageKey);
        return storedNotifications ? JSON.parse(storedNotifications) : [];
    }

    // Save notifications to localStorage
    saveNotifications() {
        localStorage.setItem(this.storageKey, JSON.stringify(this.notifications));
        if (this.onNotificationsUpdated) {
            this.onNotificationsUpdated(this.getUnreadCount());
        }
    }

    // Add a new notification
    addNotification(type, title, message, dueDate, relatedId) {
        // Check for duplicates
        const isDuplicate = this.notifications.some(n => 
            n.type === type && 
            n.relatedId === relatedId && 
            n.dueDate === dueDate
        );

        if (isDuplicate) return;

        const notification = {
            id: Date.now().toString(),
            type,
            title,
            message,
            dueDate,
            relatedId,
            createdAt: new Date().toISOString(),
            isRead: false
        };

        this.notifications.unshift(notification);
        this.saveNotifications();
        return notification;
    }

    // Mark a notification as read
    markAsRead(notificationId) {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification) {
            notification.isRead = true;
            this.saveNotifications();
        }
    }

    // Mark all notifications as read
    markAllAsRead() {
        this.notifications.forEach(n => n.isRead = true);
        this.saveNotifications();
    }

    // Get unread notifications count
    getUnreadCount() {
        return this.notifications.filter(n => !n.isRead).length;
    }

    // Get all unread notifications
    getUnreadNotifications() {
        return this.notifications.filter(n => !n.isRead);
    }

    // Check project deadlines and create notifications
    checkProjectDeadlines(projects) {
        const today = new Date();
        
        projects.forEach(project => {
            const dueDate = new Date(project.dueDate);
            const daysDiff = Math.floor((dueDate - today) / (1000 * 60 * 60 * 24));
            
            if (daysDiff === 3) {
                const formattedDate = dueDate.toLocaleDateString();
                this.addNotification(
                    'project',
                    'Project Deadline Approaching',
                    `⏰ Reminder: Project ${project.name} is due in 3 days (Due: ${formattedDate})`,
                    project.dueDate,
                    project.id
                );
            }
        });
    }

    // Check task deadlines and create notifications
    checkTaskDeadlines(tasks) {
        const today = new Date();
        
        tasks.forEach(task => {
            const dueDate = new Date(task.dueDate);
            const daysDiff = Math.floor((dueDate - today) / (1000 * 60 * 60 * 24));
            
            if (daysDiff === 1) {
                const formattedDate = dueDate.toLocaleDateString();
                this.addNotification(
                    'task',
                    'Task Due Tomorrow',
                    `📝 Task ${task.title} is due tomorrow (Due: ${formattedDate})`,
                    task.dueDate,
                    task.id
                );
            }
        });
    }

    // Delete a notification
    deleteNotification(notificationId) {
        this.notifications = this.notifications.filter(n => n.id !== notificationId);
        this.saveNotifications();
    }

    // Clear all notifications
    clearAll() {
        this.notifications = [];
        this.saveNotifications();
    }
}

// Create global instance
const notificationService = new NotificationService();
