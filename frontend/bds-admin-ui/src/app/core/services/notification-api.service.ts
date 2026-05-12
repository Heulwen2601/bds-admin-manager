import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Notification, NotificationCount, ApiResponse } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class NotificationApiService {
  private apiUrl = `${environment.apiBaseUrl}/notifications`;

  constructor(private http: HttpClient) {}

  getNotifications(page: number = 1, pageSize: number = 20): Observable<ApiResponse<Notification[]>> {
    return this.http.get<ApiResponse<Notification[]>>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }

  markAsRead(id: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/${id}/read`, {});
  }

  markAllAsRead(): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/read-all`, {});
  }

  getUnreadCount(): Observable<ApiResponse<NotificationCount>> {
    return this.http.get<ApiResponse<NotificationCount>>(`${this.apiUrl}/unread-count`);
  }
}