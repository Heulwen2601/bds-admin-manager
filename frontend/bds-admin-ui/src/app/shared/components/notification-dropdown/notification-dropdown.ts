import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { NotificationService } from '../../../core/services/notification.service';
import { NotificationApiService } from '../../../core/services/notification-api.service';
import { Notification } from '../../../models';

@Component({
  selector: 'app-notification-dropdown',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-dropdown.html',
  styleUrl: './notification-dropdown.scss',
})
export class NotificationDropdownComponent implements OnInit, OnDestroy {
  private notificationService = inject(NotificationService);
  private notificationApi = inject(NotificationApiService);
  private router = inject(Router);

  notifications: Notification[] = [];
  unreadCount = 0;
  isOpen = false;
  private subscriptions: Subscription[] = [];

  ngOnInit() {
    this.subscriptions.push(
      this.notificationService.notifications$.subscribe((notifications) => {
        this.notifications = notifications;
      }),
      this.notificationService.unreadCount$.subscribe((count) => {
        this.unreadCount = count;
      }),
    );

    // Load initial notifications
    this.loadNotifications();
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  markAsRead(id: string) {
    this.notificationApi.markAsRead(id).subscribe(() => {
      // Update local state
      this.notifications = this.notifications.map((n) =>
        n.id === id ? { ...n, isRead: true } : n,
      );
      this.unreadCount = Math.max(0, this.unreadCount - 1);
    });
  }

  markAllAsRead() {
    this.notificationApi.markAllAsRead().subscribe(() => {
      this.notifications = this.notifications.map((n) => ({ ...n, isRead: true }));
      this.unreadCount = 0;
    });
  }

  viewAll() {
    this.isOpen = false;
    this.router.navigate(['/notifications']);
  }

  private loadNotifications() {
    this.notificationApi.getNotifications(1, 10).subscribe((response) => {
      if (response.success) {
        this.notifications = response.data;
        this.notificationService.updateNotifications(response.data);
      }
    });

    this.notificationApi.getUnreadCount().subscribe((response) => {
      if (response.success) {
        this.unreadCount = response.data.unreadCount;
        this.notificationService.updateUnreadCount(response.data.unreadCount);
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString('vi-VN', { dateStyle: 'short', timeStyle: 'short' });
  }
}
