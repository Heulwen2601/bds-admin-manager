import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Property,
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  Conversation,
  AdminDashboard,
  ConsultantPerformance,
  ApiResponse,
  PagedResult,
} from '../../models';

@Injectable({
  providedIn: 'root',
})
export class AdminApiService {
  private apiUrl = `${environment.apiBaseUrl}`;

  constructor(private http: HttpClient) {}

  // Properties
  getProperties(): Observable<ApiResponse<PagedResult<Property>>> {
    return this.http.get<ApiResponse<PagedResult<Property>>>(`${this.apiUrl}/admin/properties`);
  }

  getProperty(id: string): Observable<ApiResponse<Property>> {
    return this.http.get<ApiResponse<Property>>(`${this.apiUrl}/admin/properties/${id}`);
  }

  approveProperty(id: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/admin/properties/${id}/approve`, {});
  }

  rejectProperty(id: string, reason: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/admin/properties/${id}/reject`, {
      reason,
    });
  }

  deleteProperty(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/admin/properties/${id}`);
  }

  // Categories
  getCategories(): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>(`${this.apiUrl}/categories`);
  }

  createCategory(category: CreateCategoryRequest): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>(`${this.apiUrl}/admin/categories`, category);
  }

  updateCategory(id: string, category: UpdateCategoryRequest): Observable<ApiResponse<Category>> {
    return this.http.put<ApiResponse<Category>>(`${this.apiUrl}/admin/categories/${id}`, category);
  }

  deleteCategory(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/admin/categories/${id}`);
  }

  // Conversations
  getConversations(): Observable<ApiResponse<Conversation[]>> {
    return this.http.get<ApiResponse<Conversation[]>>(`${this.apiUrl}/admin/conversations`);
  }

  // Dashboard
  getDashboard(): Observable<ApiResponse<AdminDashboard>> {
    return this.http.get<ApiResponse<AdminDashboard>>(`${this.apiUrl}/admin/dashboard`);
  }

  getConsultantPerformance(): Observable<ApiResponse<ConsultantPerformance[]>> {
    return this.http.get<ApiResponse<ConsultantPerformance[]>>(
      `${this.apiUrl}/admin/dashboard/consultant-performance`,
    );
  }
}
