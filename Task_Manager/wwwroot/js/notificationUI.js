// Notification UI - Handles the notification UI elements and interactions

class NotificationUI {
    constructor(notificationService) {
        this.notificationService = notificationService;
        this.isInitialized = false;
    }

    init() {
        if (this.isInitialized) return;
        
        // Set callback for notification updates
        this.notificationService.onNotificationsUpdated = (count) => {
            this.updateBadge(count);
        };

        // Add bell to navbar after DOM is loaded
        document.addEventListener('DOMContentLoaded', () => {
            this.createNotificationBell();
            this.updateBadge(this.notificationService.getUnreadCount());
            this.setupEventListeners();
            
            // REMOVE any test notifications here as well if they exist
        });

        this.isInitialized = true;
    }

    createNotificationBell() {
        // Find navbar-nav or similar container
        const navbarNav = document.querySelector('.navbar-nav');
        if (!navbarNav) return;

        // Create notification bell container
        const bellContainer = document.createElement('li');
        bellContainer.className = 'nav-item dropdown notification-bell-container';
        
        // Create bell element
        bellContainer.innerHTML = `
            <div class="notification-bell nav-link">
                <i class="fas fa-bell"></i>
                <span class="notification-badge">0</span>
                <div class="notification-panel">
                    <div class="notification-header">
                        <h6 class="notification-title">Notifications</h6>
                        <div class="notification-actions">
                            <button class="btn btn-sm btn-link mark-all-read">Mark all as read</button>
                            <button class="btn btn-sm btn-link clear-all">Clear all</button>
                        </div>
                    </div>
                    <ul class="notification-list"></ul>
                </div>
            </div>
        `;
        
        // Insert bell before last item (if it's a login/user menu)
        const lastNavItems = navbarNav.querySelectorAll('.nav-item');
        if (lastNavItems.length > 0) {
            navbarNav.insertBefore(bellContainer, lastNavItems[lastNavItems.length - 1]);
        } else {
            navbarNav.appendChild(bellContainer);
        }
    }

    updateBadge(count) {
        const badge = document.querySelector('.notification-badge');
        if (badge) {
            badge.textContent = count;
            badge.style.display = count > 0 ? 'block' : 'none';
        }
    }

    renderNotifications() {
        const notificationList = document.querySelector('.notification-list');
        if (!notificationList) return;

        const notifications = this.notificationService.getNotifications();
        
        if (notifications.length === 0) {
            notificationList.innerHTML = `
                <div class="notification-empty">
                    No notifications available
                </div>
            `;
            return;
        }

        notificationList.innerHTML = '';
        
        notifications.forEach(notification => {
            const timeDiff = this.getTimeDifference(notification.createdAt);
            const notificationItem = document.createElement('li');
            notificationItem.className = `notification-item ${notification.isRead ? '' : 'unread'}`;
            notificationItem.dataset.id = notification.id;
            
            notificationItem.innerHTML = `
                <div class="notification-item-header">
                    <span class="notification-item-title">${notification.title}</span>
                    <span class="notification-item-time">${timeDiff}</span>
                </div>
                <div class="notification-item-message">${notification.message}</div>
            `;
            
            notificationList.appendChild(notificationItem);
        });
    }

    getTimeDifference(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffInSeconds = Math.floor((now - date) / 1000);
        
        if (diffInSeconds < 60) return 'Just now';
        if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} min ago`;
        if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hr ago`;
        if (diffInSeconds < 604800) return `${Math.floor(diffInSeconds / 86400)} days ago`;
        
        return date.toLocaleDateString();
    }

    setupEventListeners() {
        // Toggle notification panel
        document.addEventListener('click', (e) => {
            const bell = document.querySelector('.notification-bell');
            const panel = document.querySelector('.notification-panel');
            if (!bell || !panel) return;

            // If clicking the bell, toggle the panel
            if (bell.contains(e.target)) {
                panel.classList.toggle('show');
                
                // If showing panel, render notifications
                if (panel.classList.contains('show')) {
                    this.renderNotifications();
                }
                
                e.stopPropagation();
            } 
            // If clicking outside, close the panel
            else if (!panel.contains(e.target)) {
                panel.classList.remove('show');
            }
        });

        // Handle notification item click
        document.addEventListener('click', (e) => {
            const item = e.target.closest('.notification-item');
            if (item) {
                const notificationId = item.dataset.id;
                this.notificationService.markAsRead(notificationId);
                item.classList.remove('unread');
            }
        });

        // Mark all as read
        document.addEventListener('click', (e) => {
            if (e.target.closest('.mark-all-read')) {
                this.notificationService.markAllAsRead();
                const items = document.querySelectorAll('.notification-item');
                items.forEach(item => item.classList.remove('unread'));
            }
        });

        // Clear all notifications
        document.addEventListener('click', (e) => {
            if (e.target.closest('.clear-all')) {
                this.notificationService.clearAll();
                this.renderNotifications();
            }
        });
    }
}
