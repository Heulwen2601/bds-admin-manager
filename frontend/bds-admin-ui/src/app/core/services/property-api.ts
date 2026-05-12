import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Property, CreatePropertyRequest, UpdatePropertyRequest, PropertyQueryParams, PropertyImage, ApiResponse, PagedResult } from '../../models';

@Injectable({
  providedIn: 'root'
})
export class PropertyApiService {
  private apiUrl = `${environment.apiBaseUrl}/properties`;

  constructor(private http: HttpClient) {}

  getAll(params?: PropertyQueryParams): Observable<ApiResponse<PagedResult<Property>>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key as keyof PropertyQueryParams] !== undefined) {
          httpParams = httpParams.set(key, params[key as keyof PropertyQueryParams]!.toString());
        }
      });
    }
    return this.http.get<ApiResponse<PagedResult<Property>>>(this.apiUrl, { params: httpParams });
  }

  getById(id: string): Observable<ApiResponse<Property>> {
    return this.http.get<ApiResponse<Property>>(`${this.apiUrl}/${id}`);
  }

  search(params: PropertyQueryParams): Observable<ApiResponse<PagedResult<Property>>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key as keyof PropertyQueryParams] !== undefined) {
          httpParams = httpParams.set(key, params[key as keyof PropertyQueryParams]!.toString());
        }
      });
    }
    return this.http.get<ApiResponse<PagedResult<Property>>>(`${this.apiUrl}/search`, { params: httpParams });
  }

  getImages(propertyId: string): Observable<ApiResponse<PropertyImage[]>> {
    return this.http.get<ApiResponse<PropertyImage[]>>(`${this.apiUrl}/${propertyId}/images`);
  }

  uploadImage(propertyId: string, file: File): Observable<ApiResponse<PropertyImage>> {
    const formData = new FormData();
    formData.append('image', file);
    return this.http.post<ApiResponse<PropertyImage>>(`${this.apiUrl}/${propertyId}/images`, formData);
  }

  deleteImage(propertyId: string, imageId: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${propertyId}/images/${imageId}`);
  }

  setPrimaryImage(propertyId: string, imageId: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/${propertyId}/images/${imageId}/primary`, {});
  }

  submitLead(propertyId: string, lead: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/${propertyId}/leads`, lead);
  }
}