import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Property,
  CreatePropertyRequest,
  UpdatePropertyRequest,
  SellerProfile,
  BecomeSellerRequest,
  BecomeSellerResponse,
  UpdateSellerProfileRequest,
  SellerDirectoryProfile,
  SellerDirectoryQuery,
  Lead,
  ApiResponse,
  PagedResult,
} from '../../models';

@Injectable({
  providedIn: 'root',
})
export class SellerApiService {
  private apiUrl = `${environment.apiBaseUrl}/seller`;

  constructor(private http: HttpClient) {}

  // Properties
  getProperties(): Observable<ApiResponse<Property[]>> {
    return this.http.get<ApiResponse<Property[]>>(`${this.apiUrl}/properties`);
  }

  getProperty(id: string): Observable<ApiResponse<Property>> {
    return this.http.get<ApiResponse<Property>>(`${this.apiUrl}/properties/${id}`);
  }

  createProperty(property: CreatePropertyRequest): Observable<ApiResponse<Property>> {
    return this.http.post<ApiResponse<Property>>(`${this.apiUrl}/properties`, property);
  }

  updateProperty(id: string, property: UpdatePropertyRequest): Observable<ApiResponse<Property>> {
    return this.http.put<ApiResponse<Property>>(`${this.apiUrl}/properties/${id}`, property);
  }

  deleteProperty(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/properties/${id}`);
  }

  submitPropertyForApproval(id: string): Observable<ApiResponse<void>> {
    return this.http.patch<ApiResponse<void>>(`${this.apiUrl}/properties/${id}/submit`, {});
  }

  // Profile
  becomeSeller(request: BecomeSellerRequest): Observable<ApiResponse<BecomeSellerResponse>> {
    return this.http.post<ApiResponse<BecomeSellerResponse>>(
      `${this.apiUrl}/become-seller`,
      request,
    );
  }

  getProfile(): Observable<ApiResponse<SellerProfile>> {
    return this.http.get<ApiResponse<SellerProfile>>(`${this.apiUrl}/profile`);
  }

  updateProfile(request: UpdateSellerProfileRequest): Observable<ApiResponse<SellerProfile>> {
    return this.http.put<ApiResponse<SellerProfile>>(`${this.apiUrl}/profile`, request);
  }

  getDirectory(params?: SellerDirectoryQuery): Observable<ApiResponse<SellerDirectoryProfile[]>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          httpParams = httpParams.set(key, value);
        }
      });
    }

    return this.http.get<ApiResponse<SellerDirectoryProfile[]>>(`${this.apiUrl}/directory`, {
      params: httpParams,
    });
  }

  // Leads
  getLeads(): Observable<ApiResponse<Lead[]>> {
    return this.http.get<ApiResponse<Lead[]>>(`${this.apiUrl}/leads`);
  }

  getPropertyLeads(propertyId: string): Observable<ApiResponse<Lead[]>> {
    return this.http.get<ApiResponse<Lead[]>>(`${this.apiUrl}/properties/${propertyId}/leads`);
  }

  // Dashboard
  getDashboard(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/dashboard`);
  }
}
