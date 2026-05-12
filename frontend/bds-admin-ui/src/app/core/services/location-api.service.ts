import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Location, ApiResponse } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class LocationApiService {
  private apiUrl = `${environment.apiBaseUrl}/locations`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<Location[]>> {
    return this.http.get<ApiResponse<Location[]>>(this.apiUrl);
  }

  getById(id: string): Observable<ApiResponse<Location>> {
    return this.http.get<ApiResponse<Location>>(`${this.apiUrl}/${id}`);
  }

  getChildren(parentId: string): Observable<ApiResponse<Location[]>> {
    return this.http.get<ApiResponse<Location[]>>(`${this.apiUrl}/${parentId}/children`);
  }
}